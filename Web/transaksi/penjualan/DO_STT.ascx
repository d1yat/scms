<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DO_STT.ascx.cs" Inherits="transaksi_penjualan_DO_STT" %>
<%@ Register Src="DO_STTCtrl.ascx" TagName="DO_STTCtrl" TagPrefix="uc" %>
<%@ Register Src="DO_STTCtrlPrint.ascx" TagName="DO_STTCtrlPrint" TagPrefix="uc" %>

<script type="text/javascript">
    var prepareCommandsParentDoStt = function(record, toolbar) {
        var accp = toolbar.items.get(0); // accept button

        var isSubmitDO = false;

        if (!Ext.isEmpty(record)) {
            isSubmitDO = record.get('l_sent');
        }

        if (isSubmitDO) {
            accp.setVisible(false);
        }
        else {
            accp.setVisible(true);
        }
    }

    var submitDODataToFJDoSTT = function(rec, dm) {
        if (Ext.isEmpty(rec)) {
            return;
        }

        ShowConfirm('Kirim ?', 'Apakah anda yakin ingin memproses nomor ini ?',
        function(btn) {
            if (btn == 'yes') {
                dm.SubmitMethod(rec.get('c_dono'));
            }
        });
    }

    var voidPLDataFromStoreDoStt = function(rec, dm) {
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

                      dm.DeleteMethod(rec.get('c_dono'), txt);
                  }
              });
            }
        });
    }
</script>

<ext:Hidden ID="hfGdg" runat="server" />
<ext:Hidden ID="hfMode" runat="server" />
<ext:Hidden ID="hfDONo" runat="server" />
<ext:Hidden ID="hfStoreID" runat="server" />
<ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
        <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
            <TopBar>
                <ext:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <ext:Button ID="btnAddNew" runat="server" Text="Tambah" Icon="Add">
                            <DirectEvents>
                                <Click OnEvent="btnAddNew_OnClick">
                                    <EventMask ShowMask="true" />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                        <ext:ToolbarSeparator />
                        <ext:Button ID="btnPrintPL" runat="server" Text="Cetak" Icon="Printer">
                            <DirectEvents>
                                <Click OnEvent="btnPrintPL_OnClick">
                                    <EventMask ShowMask="true" />
                                </Click>
                            </DirectEvents>
                        </ext:Button>
                        <ext:ToolbarSeparator />
                        <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
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
                    <Listeners>
                        <Command Handler="if(command == 'Delete') { voidPLDataFromStoreDoStt(record, #{DirectMethods}); } else if(command == 'Submit') { submitDODataToFJDoSTT(record, #{DirectMethods}); }" />
                    </Listeners>
                    <DirectEvents>
                        <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                            <EventMask ShowMask="true" />
                            <ExtraParams>
                                <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                                <ext:Parameter Name="Parameter" Value="c_dono" />
                                <ext:Parameter Name="PrimaryID" Value="record.data.c_dono" Mode="Raw" />
                            </ExtraParams>
                        </Command>
                    </DirectEvents>
                    <SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                    </SelectionModel>
                    <Store>
                        <ext:Store ID="storeGridDOStt" runat="server" SkinID="OriginalExtStore" RemoteSort="true">
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
                                <ext:Parameter Name="model" Value="0009" />
                                <ext:Parameter Name="parameters" Value="[['c_dono', paramValueGetter(#{txNoFltr}) + '%', ''],
                                  ['d_dodate = @0', paramRawValueGetter(#{txDODate}) , 'System.DateTime'],
                                  ['c_cusno = @0', paramValueGetter(#{txCustomer}) , 'System.String'],
                                  ['c_type = @0', paramValueGetter(#{cbViaHdr}) , 'System.String'],
                                  ['c_plno' , paramValueGetter(#{txNoPLFltr}) + '%',''],
                                  ['c_expno' , paramValueGetter(#{txNoExpFltr}) + '%',''],
                                  ['c_gdg = @0', paramValueGetter(#{cbGudang}) , 'System.Char'],
                                  ['@contains.c_entry.Contains(@0)', paramValueGetter(#{txNipFltr}),'System.String'],
                                  ['c_pin' , paramValueGetter(#{txPin}) + '%',''],
                                  ['l_sent = @0', paramValueGetter(#{sbCekSend}) , 'System.Boolean']]"                           
                                  Mode="Raw" />
                            </BaseParams>
                            <Reader>
                                <ext:JsonReader IDProperty="c_dono" Root="d.records" SuccessProperty="d.success"
                                    TotalProperty="d.totalRows">
                                    <Fields>
                                        <ext:RecordField Name="c_dono" />
                                        <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                                        <ext:RecordField Name="v_gdgdesc" />
                                        <ext:RecordField Name="v_cunam" />
                                        <ext:RecordField Name="v_ket" />
                                        <ext:RecordField Name="c_plno" />
                                        <ext:RecordField Name="c_expno" />
                                        <ext:RecordField Name="c_entry" />
                                        <ext:RecordField Name="c_pin" />
                                        <ext:RecordField Name="l_print" Type="Boolean" />
                                        <ext:RecordField Name="l_sent" Type="Boolean" />
                                    </Fields>
                                </ext:JsonReader>
                            </Reader>
                            <SortInfo Field="c_dono" Direction="DESC" />
                        </ext:Store>
                    </Store>
                    <ColumnModel>
                        <Columns>
                            <ext:CommandColumn Width="50" Resizable="false">
                                <Commands>
                                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                                </Commands>
                                <%--<PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfDONo}.getValue());" />--%>
                            </ext:CommandColumn>
                            <ext:Column ColumnID="c_dono" DataIndex="c_dono" Header="Nomor DO" Hideable="false" />
                            <ext:DateColumn ColumnID="d_dodate" DataIndex="d_dodate" Header="Tanggal" Format="dd-MM-yyyy" />
                            <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="120" />
                            <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="150" />
                            <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="VIA" Width="80" />
                            <ext:Column ColumnID="c_plno" DataIndex="c_plno" Header="No PL" />
                            <%--<ext:Column ColumnID="c_pin" DataIndex="c_pin" Header="Pin" />
              <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="No Ekspedisi" Width="100" />--%>
              <%--<ext:CheckColumn ColumnID="l_confirm" DataIndex="l_confirm" Header="Confirm" />--%>
                <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="No. Expedisi" />
                <ext:Column ColumnID="c_entry" DataIndex="c_entry" Header="N I P" />
               <ext:Column ColumnID="c_pin" DataIndex="c_pin" Header="P I N" />
               <ext:CheckColumn ColumnID="l_print" DataIndex="l_print" Header="Cetak" Width="50" />                
               <ext:CommandColumn ColumnID="l_sent" DataIndex="l_sent" Header="Kirim" Width="50">
                    <Commands>
                      <ext:GridCommand CommandName="Submit" Icon="Accept" ToolTip-Title="" ToolTip-Text="Kirim DO" />
                    </Commands>
                  <PrepareToolbar Handler="prepareCommandsParentDoStt(record, toolbar);" />
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
                            <Click Handler="clearFilterGridHeader(#{GridMain}, #{txEXPFltr},#{txDODate},#{cbGudang},#{txCustomer},#{cbViaHdr},#{txNoPLFltr},#{txNoEksFltr},#{txNoExpFltr}, #{txNipFltr}, #{txPin},#{sbCekSend});reloadFilterGrid(#{gridMain});"
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
                        <ext:DateField ID="txDODate" runat="server" EnableKeyEvents="true" Format="dd-MM-Y" AllowBlank="true">
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:DateField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:ComboBox ID="cbGudang" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                          Width="300" AllowBlank="true" TypeAhead="false" ForceSelection="false">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="true" />
                          </CustomConfig>
                          <Store>
                            <ext:Store runat="server" RemotePaging="false">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="allQuery" Value="true" />
                                <ext:Parameter Name="model" Value="2031" />
                                <ext:Parameter Name="parameters" Value="[['@contains.v_gdgdesc.Contains(@0) || @contains.c_gdg.Contains(@0)', paramTextGetter(#{cbGudang}), '']]"
                                  Mode="Raw" />
                                <ext:Parameter Name="sort" Value="c_gdg" />
                                <ext:Parameter Name="dir" Value="ASC" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                                  TotalProperty="d.totalRows">
                                  <Fields>
                                    <ext:RecordField Name="c_gdg" />
                                    <ext:RecordField Name="v_gdgdesc" />
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                            </ext:Store>
                          </Store>
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:ComboBox>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:ComboBox ID="txCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                          Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                          AllowBlank="true" ForceSelection="false">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="true" />
                          </CustomConfig>
                          <Store>
                            <ext:Store runat="server">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="start" Value="={0}" />
                                <ext:Parameter Name="limit" Value="={10}" />
                                <ext:Parameter Name="model" Value="2011" />
                                <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                    ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{txCustomer}), '']]"
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
                                                <ext:ComboBox ID="cbViaHdr" runat="server" DisplayField="v_ket" ValueField="c_type"
                                                    Width="250" AllowBlank="true" TypeAhead="false" ForceSelection="false">
                                                    <CustomConfig>
                                                        <ext:ConfigItem Name="allowBlank" Value="true" />
                                                    </CustomConfig>
                                                    <Store>
                                                        <ext:Store runat="server" RemotePaging="false">
                                                            <Proxy>
                                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                                    CallbackParam="soaScmsCallback" />
                                                            </Proxy>
                                                            <BaseParams>
                                                                <ext:Parameter Name="allQuery" Value="true" />
                                                                <ext:Parameter Name="model" Value="2001" />
                                                                <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                  ['c_notrans = @0', '02', ''],
                                  ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaHdr}), '']]" Mode="Raw" />
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
                                                    <Listeners>
                                                        <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:TextField ID="txNoPLFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                        <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:TextField ID="txNoExpFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                        <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:TextField ID="txNipFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                        <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:TextField ID="txPin" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                        <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Component>
                                        </ext:HeaderColumn>
                    <ext:HeaderColumn />                                        
                     <ext:HeaderColumn>
                        <Component>
                          <ext:SelectBox ID="sbCekSend" runat="server" ListWidth="100">
                            <Items>
                              <ext:ListItem Text="&nbsp;" Value="" />
                              <ext:ListItem Text="Sudah Terkirim" Value="true" />
                              <ext:ListItem Text="Belum Terkirim" Value="false" />
                            </Items>
                            <Listeners>
                              <Select handler="reloadFilterGrid(#{GridMain})" buffer="100" delay="100" />
                            </Listeners>
                          </ext:SelectBox>
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
<uc:DO_STTCtrl runat="server" ID="DO_STTCtrl1" />
<uc:DO_STTCtrlPrint runat="server" ID="DO_STTCtrlPrint1" />
