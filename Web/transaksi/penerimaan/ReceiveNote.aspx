<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
    CodeFile="ReceiveNote.aspx.cs" Inherits="transaksi_penerimaan_ReceiveNote" %>

<%@ Register Src="RNPembelianCtrl.ascx" TagName="RNPembelianCtrl" TagPrefix="uc1" %>
<%@ Register Src="RNKhususCtrl.ascx" TagName="RNKhususCtrl" TagPrefix="uc2" %>
<%@ Register Src="RNClaimCtrl.ascx" TagName="RNClaimCtrl" TagPrefix="uc3" %>
<%@ Register Src="RNRetur.ascx" TagName="RNRetur" TagPrefix="uc4" %>
<%@ Register Src="RNRepack.ascx" TagName="RNRepack" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

    <script type="text/javascript">
    var voidRNDataFromStore = function(rec, hid) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_gdg'), rec.get('c_rnno'), hid.getValue(), txt);
                }
              });
          }
        });
    }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <ext:Hidden ID="hfType" runat="server" />
    <ext:Hidden ID="hfTypeName" runat="server" />
    <ext:Hidden ID="hfMode" runat="server" />
    <ext:Hidden ID="hfRNKhusus" runat="server" />
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
                            <%--<ext:Button ID="btnPrintRN" runat="server" Text="Cetak" Icon="Printer" />
              <ext:ToolbarSeparator />--%>
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
                            <Command Handler="if(command == 'Delete') { voidRNDataFromStore(record, #{hfType}); }" />
                        </Listeners>
                        <DirectEvents>
                            <Command OnEvent="gridMainCommand" Before="if((command != 'Select') && (command != 'Submit')) { return false; }">
                                <EventMask ShowMask="true" />
                                <ExtraParams>
                                    <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                                    <ext:Parameter Name="Parameter" Value="c_rnno" />
                                    <ext:Parameter Name="PrimaryID" Value="record.data.c_rnno" Mode="Raw" />
                                    <ext:Parameter Name="GudangID" Value="record.data.c_gdg" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <Store>
                            <ext:Store ID="storeGridRN" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                                    <ext:Parameter Name="model" Value="0026" />
                                    <ext:Parameter Name="parameters" Value="[['c_dono', paramValueGetter(#{txDOFltr}) + '%', ''],
                    ['d_rndate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_rnno', paramValueGetter(#{txRNFltr}) + '%', ''],
                    ['c_from = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String'],
                    ['c_gdg = @0', paramValueGetter(#{cbGudangFltr}) , 'System.Char'],
                    ['d_dodate = @0', paramRawValueGetter(#{txDateDOFltr}) , 'System.DateTime'],
                    ['c_typern = @0', paramRawValueGetter(#{hfType}) , 'System.String'],
                    ['l_typern', paramRawValueGetter(#{hfRNKhusus}) , 'System.Boolean'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean'],
                    ['c_entry', paramValueGetter(#{txNipFltr}) + '%', '']]" Mode="Raw" />
                                </BaseParams>
                                <Reader>
                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                                        IDProperty="c_rnno">
                                        <Fields>
                                            <ext:RecordField Name="c_gdg" />
                                            <ext:RecordField Name="v_gdgdesc" />
                                            <ext:RecordField Name="c_rnno" />
                                            <ext:RecordField Name="d_rndate" Type="Date" DateFormat="M$" />
                                            <ext:RecordField Name="c_dono" />
                                            <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                                            <ext:RecordField Name="v_nama" />
                                            <ext:RecordField Name="l_float" Type="Boolean" />
                                            <ext:RecordField Name="l_print" Type="Boolean" />
                                            <ext:RecordField Name="l_status" Type="Boolean" />
                                            <ext:RecordField Name="c_entry" />
                                            <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />                                            
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="c_rnno" Direction="DESC" />
                            </ext:Store>
                        </Store>
                        <ColumnModel>
                            <Columns>
                                <ext:CommandColumn Width="50" Resizable="false">
                                    <Commands>
                                        <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                                        <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                                    </Commands>
                                </ext:CommandColumn>
                                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" />
                                <ext:Column ColumnID="c_rnno" DataIndex="c_rnno" Header="Nomor" Hideable="false" />
                                <ext:DateColumn ColumnID="d_rndate" DataIndex="d_rndate" Header="Tanggal" Format="dd-MM-yyyy" />
                                <ext:Column ColumnID="c_dono" DataIndex="c_dono" Header="Nomor Delivery" />
                                <ext:DateColumn ColumnID="d_dodate" DataIndex="d_dodate" Header="Tanggal Delivery"
                                    Format="dd-MM-yyyy" />
                                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" />
                                <ext:Column ColumnID="c_entry" DataIndex="c_entry" Header="NIP" />
                                <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Waktu Pembuatan RN" Format="dd-MM-yyyy HH:mm:ss" Width="150"/>                                
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
                                                            <Click Handler="clearFilterGridHeader(#{gridMain}, #{cbGudangFltr}, #{txRNFltr}, #{txDateFltr}, #{txDOFltr}, #{cbSuplierFltr}, #{txDateDOFltr});reloadFilterGrid(#{gridMain});"
                                                                Buffer="300" Delay="300" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:ComboBox ID="cbGudangFltr" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                                                        Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250" MinChars="3"
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
                                                                    <ext:Parameter Name="model" Value="2031" />
                                                                    <ext:Parameter Name="parameters" Value="[['c_gdg != @0', '2', 'System.Char']]" Mode="Raw" />
                                                                    <ext:Parameter Name="sort" Value="v_gdgdesc" />
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
                                                        <Template runat="server">
                                                            <Html>
                                                            <table cellpading="0" cellspacing="1" style="width: 250px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                              <tpl for="."><tr class="search-item">
                              <td>{c_gdg}</td><td>{v_gdgdesc}</td>
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
                                                    <ext:TextField ID="txRNFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:TextField ID="txDOFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                        <Listeners>
                                                            <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:DateField ID="txDateDOFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                                                        AllowBlank="true">
                                                        <Listeners>
                                                            <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                                        </Listeners>
                                                    </ext:DateField>
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
                                                                    <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                    ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbSuplierFltr}), '']]" Mode="Raw" />
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
                                                            <table cellpading="0" cellspacing="1" style="width: 400px">
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
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:TextField ID="txNipFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                        <Listeners>
                                                            <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                                        </Listeners>
                                                    </ext:TextField>
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
    <asp:PlaceHolder ID="phCtrl" runat="server" />
</asp:Content>
