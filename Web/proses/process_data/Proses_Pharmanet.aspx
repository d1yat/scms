<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
    CodeFile="Proses_Pharmanet.aspx.cs" Inherits="transaksi_proses_pharmanet" %>

    <%@ Register Src="Proses_PharmanetCtrl.ascx" TagName="Proses_PharmanetCtrl" TagPrefix="uc" %>

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

                      Ext.net.DirectMethods.DeleteMethod(rec.get('c_po_outlet'), txt);
                  }
              });
            }
        });
        }

        var afterEditDataConfirm = function(e, cb) {
            var qty = e.record.get('n_qty');

            if (e.field == 'Status') {
            //if (e.field == 'v_jenisSP') {
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
                    r.set('Status', 'Accepted');
                });
                chkDeclined.setValue(false);
            }
            else {
                store.each(function(r) {
                     r.set('Status', 'Pending');
                    
                });
            }
        }

        var setDefaultDecl = function(chkDefault, store, chkAccepted) {
            var chk = chkDefault.getValue();

            if (chk) {
                store.each(function(r) {
                       r.set('Status', 'Rejected');
                });
                chkAccepted.setValue(false);
            }
            else {
                store.each(function(r) {
                         r.set('Status', 'Pending');
                    
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
                           <%--<ext:ToolbarSeparator />       --%>                 
                        
                            <%--<ext:ToolbarSeparator />--%>
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
                                                                  
                                                                                   
                                    <ext:ComboBox ID="cbCustomerHdr" runat="server"  FieldLabel="Cabang" DisplayField="v_cunam" 
                                         ValueField="c_cusno" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" 
                                         MinChars="3" AllowBlank="true"  ForceSelection="false">
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
                                                  ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]" Mode="Raw" />
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
                                            <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                                            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                          </Triggers>
                                          
                                                                                  
                                          <Listeners>
                                            <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                            <Change Handler="reloadFilterGrid(#{gridMain});" />
                                             <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                                          </Listeners>
                                        </ext:ComboBox>
                                        


                                    <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" ColumnWidth=".3"
                                        Layout="Form">
                                        <Items>
                                        </Items>
                                    </ext:Panel>

                                                                 
                                    
                                     <ext:ComboBox ID="cbStatusHdr" runat="server"  FieldLabel="Jenis Status" DisplayField="v_ket" 
                                         ValueField="c_type" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" 
                                         MinChars="3" AllowBlank="true"  ForceSelection="false">
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
                                                <ext:Parameter Name="model" Value="2001" />
                                                <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                                                                ['c_notrans = @0', '009', '']]" Mode="Raw" />
                                               
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
                                          <Template ID="Template2" runat="server">
                                            <Html>
                                            <table cellpading="0" cellspacing="1" style="width: 400px">
                                                <tr><td class="body-panel">Kode</td><td class="body-panel">Keterangan</td></tr>
                                                <tpl for="."><tr class="search-item">
                                                    <td>{c_type}</td><td>{v_ket}</td>
                                                </tr></tpl>
                                                </table>
                                            </Html>
                                          </Template>
                                          <Triggers>
                                            <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                                            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                          </Triggers>
                                          
                                                                                  
                                          <Listeners>
                                            <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                            <Change Handler="reloadFilterGrid(#{gridMain});" />
                                             <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                                          </Listeners>
                                        </ext:ComboBox>

                                
                                                                       
                                  
                                </Items>
                            </ext:FormPanel>
                        </North>
                        <Center>
                            <ext:Panel ID="Panel2" runat="server" Layout="FitLayout">
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>
                                            <%--<ext:Checkbox runat="server" ID="chkAllAcc" FieldLabel="Set All Accepted ">
                                                <Listeners>
                                                    <Check Handler="setDefaultAcc(this, #{gridMain}.getStore(),#{chkAllDec});" />
                                                </Listeners>
                                            </ext:Checkbox>--%>
                                            <%--<ext:ToolbarSeparator />--%>
                                           <%-- <ext:Checkbox runat="server" ID="chkAllDec" FieldLabel="Set All Rejected ">
                                                <Listeners>
                                                    <Check Handler="setDefaultDecl(this, #{gridMain}.getStore(),#{chkAllAcc});" />
                                                </Listeners>
                                            </ext:Checkbox>--%>
                                            <%--<ext:ToolbarSeparator />--%>
                                          
                                       
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>
                                <Items>
                                    <ext:GridPanel ID="gridMain" runat="server">
                                        <LoadMask ShowMask="true" />
                                        <Listeners>
                                            <Command Handler="if(command == 'Delete') { voidSPDataFromStore(record); }" />
                                        </Listeners>
                                        
                                                                               
                                        <%--hafizh--%>
                                        <DirectEvents>
                                          <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                                            <EventMask ShowMask="true" />
                                            <ExtraParams>
                                              <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                                              <ext:Parameter Name="Parameter" Value="c_po_outlet" />
                                              <ext:Parameter Name="PrimaryID" Value="record.data.c_po_outlet" Mode="Raw" />
                                            </ExtraParams>
                                          </Command>
                                        </DirectEvents>
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
                                                    <ext:Parameter Name="model" Value="0024" />
                                                    <ext:Parameter Name="parameters" Value="[
                                                           ['c_plphar = @0', paramValueGetter(#{txPLFltr}) , 'System.String'],
                                                           ['c_cusno = @0', paramValueGetter(#{cbCustomerHdr}) , 'System.String'],
                                                           ['c_nosup = @0', '00165' , 'System.String'],
                                                           ['Status = @0', paramValueGetter(#{cbStatusHdr}) , 'System.String']
                                                            
                                                            ]" Mode="Raw" />
                                                </BaseParams>
                                                <Reader>
                                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                        <Fields>
                                                            <ext:RecordField Name="c_nosup" />
                                                            <ext:RecordField Name="v_nama" />
                                                            <ext:RecordField Name="c_dono_detail" />
                                                            <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                                                            <ext:RecordField Name="c_po_outlet" />
                                                            <ext:RecordField Name="c_plphar" />
                                                            <ext:RecordField Name="c_cusno" />
                                                            <ext:RecordField Name="v_cunam" />
                                                            <ext:RecordField Name="c_type" />
                                                            <ext:RecordField Name="Status" />
                                                            <ext:RecordField Name="v_ket" />
                                                            <ext:RecordField Name="v_status" />
                                                            
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <SortInfo Field="c_po_outlet" Direction="DESC" />
                                            </ext:Store>
                                        </Store>
                                        <ColumnModel>
                                            <Columns>
                                                <ext:CommandColumn Width="25" Resizable="false">
                                                    <Commands>
                                                        <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                                                        <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                                                    </Commands>
                                                </ext:CommandColumn>
                                                <ext:Column ColumnID="c_nosup" DataIndex="c_nosup" Header="Nomor Supplier" Width="80" />
                                                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama Supplier" Width="150" />
                                                <ext:Column ColumnID="c_dono_detail" DataIndex="c_dono_detail" Header="Nomor DO" Width="80" />
                                                <ext:DateColumn ColumnID="d_dodate" DataIndex="d_dodate" Header="Tanggal DO" Format="dd-MM-yyyy"
                                                    Width="80" />
                                                <ext:Column ColumnID="c_po_outlet" DataIndex="c_po_outlet" Header="PO Outlet" />
                                                <ext:Column ColumnID="c_plphar" DataIndex="c_plphar" Header="PL Pharmanet" Width="80" />
                                                <ext:Column ColumnID="c_cusno" DataIndex="c_cusno" Header="Kode Cabang" Width="80" />
                                                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Nama Cabang" Width="150" />
                                                <ext:Column ColumnID="v_status" DataIndex="v_status" Header="Jenis Status" Width="150" />
                                                                                                
                                               
                                               
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
                                                                            <Click Handler="clearFilterGridHeader(#{gridMain}, #{txSPFltr},#{txPLFltr},#{cbPrincipalFltr},#{txDateFltr});reloadFilterGrid(#{gridMain});"
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
                                                              
                                                               <%--Principle--%>
                                                               
                                                                <Component>
                                                                  <ext:ComboBox ID="cbPrincipalFltr" runat="server" DisplayField="v_nama" ValueField="c_nosup"
                                                                    Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350" MinChars="3"
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
                                                                          <ext:Parameter Name="model" Value="2021" />
                                                                          <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                                                                      ['l_hide = @0', false, 'System.Boolean'],
                                                                                      ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalFltr}), '']]"
                                                                            Mode="Raw" />
                                                                          <ext:Parameter Name="sort" Value="v_nama" />
                                                                          <ext:Parameter Name="dir" Value="ASC" />
                                                                        </BaseParams>
                                                                        <Reader>
                                                                          <ext:JsonReader IDProperty="c_nosup" Root="d.records" SuccessProperty="d.success"
                                                                            TotalProperty="d.totalRows">
                                                                            <Fields>
                                                                              <ext:RecordField Name="c_nosup" />
                                                                              <ext:RecordField Name="v_nama" />
                                                                            </Fields>
                                                                          </ext:JsonReader>
                                                                        </Reader>
                                                                      </ext:Store>
                                                                    </Store>
                                                                    <Template ID="Template6" runat="server">
                                                                      <Html>
                                                                      <table cellpading="0" cellspacing="0" style="width: 350px">
                                                                        <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                                                                        <tpl for="."><tr class="search-item">
                                                                            <td>{c_nosup}</td><td>{v_nama}</td>
                                                                        </tr></tpl>
                                                                        </table>
                                                                      </Html>
                                                                    </Template>
                                                                    <Listeners>
                                                                     <%-- <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />--%>
                                                                       <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                                                    </Listeners>
                                                                  </ext:ComboBox>
                                                                </Component>
                                                             </ext:HeaderColumn>
                                                             <ext:HeaderColumn />
                                                             <ext:HeaderColumn />
                                                             <ext:HeaderColumn />
                                                              <ext:HeaderColumn>
                                                              <%--Nomor PL--%>
                                                                <Component>
                                                                    <ext:TextField ID="txPLFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                        <Listeners>
                                                                            <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                                                        </Listeners>
                                                                    </ext:TextField>
                                                                </Component>
                                                            </ext:HeaderColumn>
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
   <uc:Proses_PharmanetCtrl ID="Proses_PharmanetCtrl2" runat="server" />

</asp:Content>
