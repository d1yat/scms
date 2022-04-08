<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="PackingList.aspx.cs" Inherits="transaction_sales_PackingList" %>

<%@ Register Src="PackingListCtrl.ascx" TagName="PackingListCtrl" TagPrefix="uc" %>
<%@ Register Src="PackingListPrintCtrl.ascx" TagName="PackingListPrintCtrl" TagPrefix="uc" %>
<%@ Register Src="PackingListAutoCtrl.ascx" TagName="PackingListAutoCtrl" TagPrefix="uc" %>
<%@ Register Src="PackingListConfirmCtrl.ascx" TagName="PackingListConfirmCtrl" TagPrefix="uc" %>
<%@ Register Src="PackingListMasterBox.ascx" TagName="PackingListMasterBox" TagPrefix="uc" %>
<%@ Register Src="PackingListAutoGeneratorCtrl.ascx" TagName="PackingListAutoGeneratorCtrl" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidPLDataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_plno'), txt);
                }
              });
          }
        });
    }
    var prepareCommandsPage = function(toolbar, valX) {
      var vd = toolbar.items.get(1); // void button

      var isHideConfirm = (valX == 'CF' ? false : true);

      vd.setVisible(isHideConfirm);      
    } 
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfGdg" runat="server" />
  <ext:Hidden ID="hfMode" runat="server" />
  <ext:Hidden ID="hfType" runat="server" />
  
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
              <ext:Checkbox ID="ChkPrint" runat="server" BoxLabel="Sudah Di Print">
              </ext:Checkbox>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel ID="gridMain" runat="server">
            <LoadMask ShowMask="true" />
            <Listeners>
              <%--<Command Handler="if(command == 'Delete') { voidPLDataFromStore(record); }" />--%>
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_plno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_plno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridPL" runat="server" SkinID="OriginalExtStore" RemoteSort="true">
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
                  <ext:Parameter Name="model" Value="0001" />
                  <ext:Parameter Name="parameters" Value="[['c_plno', paramValueGetter(#{txPLFltr}) + '%', ''],
                    ['d_pldate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_gdg = @0', #{hfGdg}.getValue(), 'System.Char'],
                    ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                    ['c_nosup = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String'],
                    ['c_type = @0', paramValueGetter(#{hfType}) , 'System.String'],
                    ['@contains.c_entry.Contains(@0)', paramValueGetter(#{txNipFltr}) , 'System.String'],
                    ['@contains.v_entry.Contains(@0)', paramValueGetter(#{txUserFltr}) , 'System.String'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean'],
                    ['l_print = @0', paramValueGetter(#{ChkPrint}), 'System.Boolean']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_plno">
                    <Fields>
                      <ext:RecordField Name="c_plno" />
                      <ext:RecordField Name="d_pldate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="v_cunam" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="c_entry" />
                      <ext:RecordField Name="v_entry" />
                      <ext:RecordField Name="v_ket_type" />
                      <ext:RecordField Name="l_print" Type="Boolean" />
                      <ext:RecordField Name="l_confirm" Type="Boolean" />
                      <ext:RecordField Name="l_do" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_plno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                  </Commands>
                  <PrepareToolbar Handler="prepareCommandsPage(toolbar, #{hfMode}.getValue());" />
                </ext:CommandColumn>
                <ext:Column ColumnID="c_plno" DataIndex="c_plno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_pldate" DataIndex="d_pldate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" />
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" />
                <ext:CheckColumn ColumnID="l_print" DataIndex="l_print" Header="Cetak" Width="50" />
                <ext:CheckColumn ColumnID="l_confirm" DataIndex="l_confirm" Header="Konfirmasi" Width="75" />
                <ext:CheckColumn ColumnID="l_do" DataIndex="l_do" Header="DO" Width="25" />
                <ext:Column ColumnID="v_ket_type" DataIndex="v_ket_type" Header="Tipe" />
                <ext:Column ColumnID="c_entry" DataIndex="c_entry" Header="N I P" />
                <ext:Column ColumnID="v_entry" DataIndex="v_entry" Header="User" Hidden="true" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txPLFltr}, #{txDateFltr}, #{cbCustomerFltr}, #{cbSuplierFltr}, #{txNipFltr}, #{txUserFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txPLFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
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
                                    ['l_cabang = @0', true, 'System.Boolean'],
                                    ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), 'System.String']]"
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
                            <Template runat="server">
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
                          <ext:ComboBox ID="cbSuplierFltr" runat="server" DisplayField="v_nama" ValueField="c_nosup"
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
                                  <ext:Parameter Name="model" Value="2021" />
                                  <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                ['l_hide = @0', false, 'System.Boolean'],
                                ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplierFltr}), 'System.String']]" Mode="Raw" />
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
                            <Template runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="0" style="width: 400px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                              <tpl for="."><tr class="search-item">
                                  <td>{c_nosup}</td><td>{v_nama}</td>
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
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn >
                        <%--<Component>
                        
                        <ext:ComboBox ID="cbTipeHdr" runat="server" DisplayField="v_ket" ValueField="c_type" 
                          Width="150">
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
                                                  ['c_notrans = @0', '15', ''],
                                                  ['c_type != @0', '02', ''],
                                                  ['c_type != @0', '05', '']]" Mode="Raw" />
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
                        </Component>--%>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNipFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txUserFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
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
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  <uc:PackingListCtrl ID="PackingListCtrl1" runat="server" />
  <uc:PackingListPrintCtrl ID="PackingListPrintCtrl1" runat="server" />
  <uc:PackingListAutoCtrl ID="PackingListAutoCtrl1" runat="server" />
  <uc:PackingListConfirmCtrl ID="PackingListConfirmCtrl1" runat="server" />
  <uc:PackingListMasterBox ID="PackingListMasterBox" runat="server" />
  <uc:PackingListAutoGeneratorCtrl ID="PackingListAutoGeneratorCtrl" runat="server" />
</asp:Content>
