<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaRCCtrl.ascx.cs" Inherits="transaksi_wp_SerahTerimaRCCtrl" %>

<script type="text/javascript">
    var storeToDetailGridTransport = function(frm, grid, transNo) {
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


    var prepareCommandsDetilBanget = function(rec, toolbar) {
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
            vd.setVisible(false);
            rec.set('l_modified', true);            
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
                <ext:FormPanel ID="frmHeaders" runat="server" Border="false" Padding="10" Height="160">
                    <Items>
                        <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Penerima">
                            <Items>
                                <ext:TextField ID="txNipPenerima" runat="server" AllowBlank="false" Width="100" MaxLength="10">
                                    <DirectEvents>
                                        <SpecialKey OnEvent="OnEvenCheckUser" Buffer="250" Delay="250">
                                            <ExtraParams>
                                                <ext:Parameter Name="NipPenerima" Value="#{txNipPenerima}.getValue()" Mode="Raw" />
                                            </ExtraParams>
                                        </SpecialKey>
                                    </DirectEvents>
                                </ext:TextField>
                                <ext:Label ID="Label2" runat="server" Text="-" />
                                <ext:TextField ID="txNamePenerima" runat="server" AllowBlank="false" Disabled="true"
                                    Width="200" />
                            </Items>
                        </ext:CompositeField>
                        <ext:ComboBox ID="cbEksHdr" runat="server" FieldLabel="Ekspedisi" DisplayField="v_ket"
                            ValueField="c_exp" WListWidth="275" MinChars="3" AllowBlank="false" ItemSelector="tr.search-item"
                            ForceSelection="false" ListWidth="275" PageSize="10">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="false" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store3" runat="server">
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
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="5002" />
                                  <ext:Parameter Name="parameters" Value="[['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbEksHdr}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_ket" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                                    TotalProperty="d.totalRows">
                                    <Fields>
                                      <ext:RecordField Name="c_exp" />
                                      <ext:RecordField Name="v_ket" />
                                    </Fields>
                                  </ext:JsonReader>
                                </Reader>
                              </ext:Store>
                            </Store>
                            <Template ID="Template2" runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="1" style="width: 400px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                              <tpl for="."><tr class="search-item">
                                  <td>{c_exp}</td><td>{v_ket}</td>
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
                        <ext:TextField ID="txNoResiHdr" runat="server" FieldLabel="No Resi" AllowBlank="false" />                  
                        <ext:NumberField ID="txKoli" runat="server" FieldLabel="Koli" AllowBlank="false"
                              AllowNegative="false" MinValue="-1" />
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
                                        <ext:TextField ID="txPBBR" runat="server" AllowBlank="false" FieldLabel="No.PBB/R">
                                            <DirectEvents>
                                                <SpecialKey OnEvent="Submit_scane" Buffer="500" Delay="500">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="NO" Value="#{txPBBR}.getValue()" Mode="Raw" />
                                                        <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                    </ExtraParams>
                                                </SpecialKey>
                                            </DirectEvents>
                                        </ext:TextField>
                                        <%--<ext:ComboBox ID="cbNoDtl" runat="server" FieldLabel="No.DO/SJ" ItemSelector="tr.search-item"
                                            DisplayField="noTrans" ValueField="noTrans" MinChars="3" PageSize="10" ListWidth="300"
                                            AllowBlank="false" ForceSelection="false">
                                            <CustomConfig>
                                                <ext:ConfigItem Name="allowBlank" Value="false" />
                                            </CustomConfig>
                                            <Store>
                                                <ext:Store ID="Store7" runat="server" AutoLoad="false">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="10" />
                                                        <ext:Parameter Name="model" Value="0306b" />
                                                        <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                                                            ['@contains.noTrans.Contains(@0)', paramTextGetter(#{cbNoDtl}), '']]" Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="noTrans" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="noTrans" Root="d.records" SuccessProperty="d.success"
                                                            TotalProperty="d.totalRows">
                                                            <Fields>
                                                                <ext:RecordField Name="noTrans" />
                                                                <ext:RecordField Name="dTrans" Type="Date" DateFormat="M$" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <Template ID="Template4" runat="server">
                                                <Html>
                                                <table cellpading="0" cellspacing="1" style="width: 300px">
                                                <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td></tr>
                                                <tpl for="."><tr class="search-item">
                                                <td>{noTrans}</td><td>{dTrans:this.formatDate}</td>
                                                </tr></tpl>
                                                </table>
                                                </Html>
                                                <Functions>
                                                    <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                                                </Functions>
                                            </Template>
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                            </Listeners>
                                            <DirectEvents>
                                                <SpecialKey OnEvent="Submit_scane" Buffer="250" Delay="250">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="NO" Value="#{cbNoDtl}.getText()" Mode="Raw" />
                                                        <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                    </ExtraParams>
                                                </SpecialKey>
                                            </DirectEvents>
                                        </ext:ComboBox>--%>
                                        <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                                            Icon="Add">
                                            <Listeners>
                                                <Click Handler="storeToDetailGridTransport(#{frmpnlDetailEntry}, #{gridDetail}, #{txPBBR});" />
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
                                <ext:Panel ID="Panel1" runat="server" ColumnWidth="0.40" Layout="FitLayout">
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
                                                                <%--<ext:RecordField Name="n_koli" Type="Float" />
                                                                <ext:RecordField Name="n_berat" Type="Float" />--%>
                                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                                                
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
                                                        <PrepareToolbar Handler="prepareCommandsDetilBanget(record, toolbar);" />
                                                    </ext:CommandColumn>
                                                    <ext:Column DataIndex="c_no" Header="Kode" Width="150" />
                                                    <%--<ext:NumberColumn DataIndex="n_koli" Header="Koli" Format="0.000,00/i" Width="75" >
                                                      <Editor>
                                                        <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                                                      </Editor>
                                                    </ext:NumberColumn>
                                                    <ext:NumberColumn DataIndex="n_berat" Header="Berat" Format="0.000,00/i" Width="75" >
                                                      <Editor>
                                                        <ext:NumberField ID="NumberField2" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                                                      </Editor>
                                                    </ext:NumberColumn>--%>
                                                    <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                                </Columns>
                                            </ColumnModel>
                                            <Listeners>
                                                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                                            </Listeners>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                                <ext:Panel ID="Panel2" runat="server" ColumnWidth="0.60" Layout="FitLayout">
                                    <Items>
                                        <ext:GridPanel ID="gridDetail2" runat="server">
                                          <LoadMask ShowMask="true" />
                                          <SelectionModel>
                                            <ext:RowSelectionModel SingleSelect="true" />
                                          </SelectionModel>
                                          <Store>
                                            <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false">
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
                                                    <ext:RecordField Name="C_ITENO" />
                                                    <ext:RecordField Name="C_ITNAM" />
                                                    <ext:RecordField Name="C_BATCH" />
                                                    <ext:RecordField Name="N_QTY" />
                                                    <ext:RecordField Name="C_NOREF" />
                                                  </Fields>
                                                </ext:JsonReader>
                                              </Reader>
                                            </ext:Store>
                                          </Store>
                                          <ColumnModel>
                                            <Columns>
                                             <%-- <ext:CommandColumn Width="0">
                                                <Commands>
                                                  <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                                                  <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                                                </Commands>
                                                <PrepareToolbar Handler="prepareCommandsProcess(record, toolbar, #{hfRCNoProcess}.getValue());" />
                                              </ext:CommandColumn>--%>
                                              <ext:Column DataIndex="C_ITENO" Header="Kode" Width="50" />
                                              <ext:Column DataIndex="C_ITNAM" Header="Nama Barang" Width="250" />
                                              <ext:Column DataIndex="C_BATCH" Header="Batch " />
                                              <ext:Column DataIndex="C_NOREF" Header="No. DO" />
                                              <ext:Column DataIndex="N_QTY" Header="Qty" />
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