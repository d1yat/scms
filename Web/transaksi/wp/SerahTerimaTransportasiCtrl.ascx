<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaTransportasiCtrl.ascx.cs" Inherits="transaksi_wp_SerahTerimaTransportasiCtrl" %>

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

    var recalc = function(store) {
        var txKoli = Ext.getCmp('<%= txKoli.ClientID %>');
        var txReceh = Ext.getCmp('<%= txReceh.ClientID %>');
            

        var totalKarton = 0,
        totalReceh = 0;

        store.each(function(r) {
            totalKarton += r.get('n_karton');
            totalReceh += r.get('n_receh');
        });

        if (!Ext.isEmpty(txKoli)) {
            txKoli.setValue(myFormatNumber(totalKarton));
        }
        if (!Ext.isEmpty(txReceh)) {
            txReceh.setValue(myFormatNumber(totalReceh));
        }
    }

    var beforeEditDataConfirm = function(e, hfPrevKarton, hfPrevReceh) {
        var oldval = e.record.get('n_karton');
        var oldReceh = e.record.get('n_receh');

        hfPrevKarton.setValue(oldval);
        hfPrevReceh.setValue(oldReceh);
    }

    var onGridAfterEdit = function(e, store, hfPrevKarton, hfPrevReceh, store2) {
        var totalQty = 0;

        store2.each(function(r) {
            totalQty += r.get('n_qty');
        });

        if (e.field == 'n_karton') {
            var karton = e.record.get('n_karton');
            if (karton == 0) {
                e.record.set('n_receh', totalQty);
            } 
        }

        var valedit = e.record.get('n_karton');
        var valreceh = e.record.get('n_receh');
        if (hfPrevKarton != valedit) {
            e.record.set('l_modifiedkoli', true);
        }
        if (hfPrevReceh != valreceh) {
            e.record.set('l_modifiedkoli', true);
        }
        recalc(store);
    }
</script>

<ext:Window runat="server" ID="wndIDUser" Title="Validasi User" Height="520" Width="800"
    Hidden="true" MinHeight="480" MinWidth="600" Layout="Fit" Maximizable="true">
    <Content>
        <ext:Hidden ID="hfGudang" runat="server" />
        <ext:Hidden ID="hfGudangDesc" runat="server" />
        <ext:Hidden ID="hfSTNo" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
        <ext:Hidden ID="hfPrevReceh" runat="server" />
        <ext:Hidden ID="hfPrevKarton" runat="server" />
    </Content>
    <Items>
        <ext:BorderLayout ID="bllayout" runat="server">
            <North MinHeight="250" MaxHeight="250" Collapsible="false">
                <ext:FormPanel ID="frmHeaders" runat="server" Border="false" Padding="10" Height="180" Layout="Column">
                  <Items>
                    <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                        <Items>
                            <%--<ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Penerima">
                                <Items>
                                    <ext:TextField ID="txNipPenerima" runat="server" AllowBlank="false" Width="100" MaxLength="10"
                                        Disabled="true" />
                                    <ext:Label ID="Label3" runat="server" Text="-" />
                                    <ext:TextField ID="txNamePenerima" runat="server" AllowBlank="false" Disabled="true"
                                        Width="200" />
                                </Items>
                            </ext:CompositeField>--%>
                            <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Petugas Serah Terima" LabelWidth="500">
                                <Items>
                                    <ext:TextField ID="txNipPenyerah" runat="server" AllowBlank="false" Width="150" MaxLength="10">
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
                                        Width="150" />
                                    <%--<ext:Label ID="lblPassword" runat="server" Text="- Kata Kunci :" />
                                    <ext:TextField ID="txPassword" runat="server" AllowBlank="false" InputType="Password" />--%>
                                </Items>
                            </ext:CompositeField>
                            <ext:ComboBox ID="cbCustomer" runat="server" FieldLabel="Tujuan" DisplayField="v_cunam"
                                ValueField="c_cusno" Width="150" ItemSelector="tr.search-item" PageSize="10"
                                ListWidth="300" MinChars="3" AllowBlank="false" ForceSelection="false">
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
                                            <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), '']]" Mode="Raw" />
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
                                <Template ID="Template3" runat="server">
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
                            <ext:NumberField ID="txKoli" runat="server" FieldLabel="Koli Utuh" AllowBlank="false"
                                  AllowNegative="false" MinValue="-1" Disabled="true" />
                        </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                        <Items>
                        <%--<ext:ComboBox ID="cbDusReceh" runat="server" FieldLabel="Dus Receh" AllowBlank="true">
                                <CustomConfig>
                                    <ext:ConfigItem Name="allowBlank" Value="false" />
                                </CustomConfig>
                                <Store>
                                    <ext:Store ID="Store1" runat="server" SkinID="OriginalExtStore">
                                        <Proxy>
                                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                CallbackParam="soaScmsCallback" />
                                        </Proxy>
                                        <BaseParams>
                                            <ext:Parameter Name="start" Value="={0}" />
                                            <ext:Parameter Name="model" Value="2012" />
                                            <ext:Parameter Name="dir" Value="ASC" />
                                        </BaseParams>
                                        <Reader>
                                            <ext:JsonReader IDProperty="kd_dus" Root="d.records" SuccessProperty="d.success"
                                                TotalProperty="d.totalRows">
                                                <Fields>
                                                    <ext:RecordField Name="v_nama_dus" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                    </ext:Store>
                                </Store>
                                <Template ID="Template1" runat="server">
                                    <Html>
                                    <table cellpading="0" cellspacing="1" style="width: 400px">
                                <tpl for="."><tr class="search-item">
                                    <tr>
                                        <td>{v_nama_dus}</td>
                                    </tr>
                                </tpl>
                                </table>
                                    </Html>
                                </Template>
                                <Triggers>
                                    <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                </Triggers>
                                <Listeners>
                                    <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                </Listeners>

                        </ext:ComboBox>--%>
                        <ext:NumberField ID="txKoliReceh" runat="server" FieldLabel="Koli Receh" AllowBlank="false"
                              AllowNegative="false" MinValue="-1" Disabled="false" />
                        <ext:NumberField ID="txReceh" runat="server" FieldLabel="Qty Receh" AllowBlank="false"
                              AllowNegative="false" MinValue="-1" Disabled="true" />  
                        <ext:NumberField ID="txBerat" runat="server" FieldLabel="Berat (Kg)" AllowBlank="false"
                              AllowNegative="false" MinValue="0" />
                        <ext:NumberField ID="txVolume" runat="server" FieldLabel="Volume (Meter Kubik)" AllowBlank="false"
                              AllowNegative="false" DecimalPrecision="5" />
                              </Items>
                    </ext:Panel>
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
                                        <ext:TextField ID="cbNoDtl" runat="server" AllowBlank="false" FieldLabel="No.DO/SJ">
                                            <DirectEvents>
                                                <SpecialKey OnEvent="Submit_scane" Buffer="250" Delay="250">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="NO" Value="#{cbNoDtl}.getValue()" Mode="Raw" />
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
                                        <%--<ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                                            Icon="Add">
                                            <Listeners>
                                                <Click Handler="storeToDetailGridTransport(#{frmpnlDetailEntry}, #{gridDetail}, #{cbNoDtl});" />
                                            </Listeners>
                                        </ext:Button>--%>
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
                                                        <ext:Parameter Name="model" Value="0319" />
                                                        <ext:Parameter Name="sort" Value="" />
                                                        <ext:Parameter Name="dir" Value="" />
                                                        <ext:Parameter Name="parameters" Value="[['c_nodoc = @0', #{hfSTNo}.getValue(), 'System.String']]"
                                                            Mode="Raw" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                            <Fields>
                                                                <ext:RecordField Name="c_no" />
                                                                <ext:RecordField Name="n_karton" Type="Float" />
                                                                <ext:RecordField Name="n_receh" Type="Float" />
                                                                <ext:RecordField Name="n_hiddenkarton" Type="Float" />
                                                                <ext:RecordField Name="n_hiddenreceh" Type="Float" />
                                                                <ext:RecordField Name="c_type_editkoli" />
                                                                <ext:RecordField Name="v_ket_editkoli" />
                                                                <%--<ext:RecordField Name="n_koli" Type="Float" />
                                                                <ext:RecordField Name="n_berat" Type="Float" />--%>
                                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                                                <ext:RecordField Name="l_modifiedkoli" Type="Boolean" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                    <Listeners>
                                                        <Load Handler="recalc(#{gridDetail}.getStore());" />
                                                        <Add Handler ="recalc(#{gridDetail}.getStore());" />
                                                        <Remove Handler ="recalc(#{gridDetail}.getStore());" />
                                                    </Listeners>
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
                                                    <ext:Column DataIndex="c_no" Header="Kode" Width="100" />
                                                    <ext:NumberColumn DataIndex="n_karton" Header="Koli" Format="0.000,00/i" Width="75">
                                                    <Editor>
                                                      <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                                        DecimalPrecision="2" MinValue="0" >
                                                      </ext:NumberField>
                                                    </Editor>
                                                    </ext:NumberColumn>
                                                    <ext:NumberColumn DataIndex="n_receh" Header="Receh" Format="0.000,00/i" Width="75">
                                                        <Editor>
                                                          <ext:NumberField ID="NumberField2" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                                            DecimalPrecision="2" MinValue="0" />
                                                        </Editor>
                                                    </ext:NumberColumn>
                                                    <ext:Column DataIndex="v_ket_editkoli" Header="Alasan Edit" Width="150">
                                                      <Editor>
                                                        <ext:ComboBox ID="cbTypeDcGrd" runat="server" DisplayField="v_ket" ValueField="c_type"
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
                                                              <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                                              ['c_notrans = @0', '66', '']]" Mode="Raw" />
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
                                                <BeforeEdit Handler="beforeEditDataConfirm(e, #{hfPrevKarton}, #{hfPrevReceh});" />
                                                <AfterEdit Handler="onGridAfterEdit(e, #{gridDetail}.getStore(), #{hfPrevKarton}.getValue(), #{hfPrevReceh}.getValue(),#{GridDetail2}.getStore());" />
                                            </Listeners>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                                <ext:Panel ID="Panel2" runat="server" ColumnWidth="0.60" Layout="FitLayout">
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
                                                                <ext:RecordField Name="v_cunam" />                                                                
                                                                <ext:RecordField Name="c_spno" />
                                                                <ext:RecordField Name="c_batch" />
                                                                <ext:RecordField Name="n_qty" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <ColumnModel ID="ctl345">
                                                <Columns>
                                                    <ext:CommandColumn Width="25">
                                                    </ext:CommandColumn>
                                                    <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                                                    <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="150" />
                                                    <ext:Column DataIndex="v_cunam" Header="Cabang" />                                                                                                        
                                                    <ext:Column DataIndex="c_spno" Header="Nomor" />
                                                    <ext:Column DataIndex="c_batch" Header="Batch " />
                                                    <ext:Column DataIndex="n_qty" Header="Qty" />
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