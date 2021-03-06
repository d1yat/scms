<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="ReceiveNoteGudang.aspx.cs" Inherits="transaksi_penerimaan_ReceiveNoteGudang" %>

<%@ Register Src="ReceiveNoteGudangCtrl.ascx" TagName="ReceiveNoteGudangCtrl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
//    var voidPODataFromStore = function(rec) {
//      if (Ext.isEmpty(rec)) {
//        return;
//      }

//      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
//        function(btn) {
//          if (btn == 'yes') {
//            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
//              function(btnP, txt) {
//                if (btnP == 'ok') {
//                  if (txt.trim().length < 1) {
//                    txt = 'Kesalahan pemakai.';
//                  }

//                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_gdg'), rec.get('c_pono'), txt);
//                }
//              });
//          }
//        });
//    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfTypeName" runat="server" />
  <ext:Hidden ID="hfMode" runat="server" />
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
              <ext:Button ID="btnPrintRNG" runat="server" Text="Cetak" Icon="Printer" />
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
            <%--<Listeners>
              <Command Handler="if(command == 'Delete') { voidPODataFromStore(record); }" />
            </Listeners>--%>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_sjno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_sjno" Mode="Raw" />
                  <ext:Parameter Name="GudangID" Value="record.data.c_gdg" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridRNG" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0042" />
                  <ext:Parameter Name="parameters" Value="[['c_gdg2 = @0', paramValueGetter(#{cbGudangToFltr}), 'System.Char'],
                    ['d_sjdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_sjno', paramValueGetter(#{txPOFltr}) + '%', ''],
                    ['v_ket', paramValueGetter(#{txKeteranganFltr}) + '%' , ''],
                    ['c_gdg = @0', paramValueGetter(#{cbGudangFltr}) , 'System.Char'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                    <Fields>
                      <ext:RecordField Name="c_gdg" />
                      <ext:RecordField Name="v_gdgFromDesc" />
                      <ext:RecordField Name="c_gdg2" />
                      <ext:RecordField Name="v_gdgToDesc" />
                      <ext:RecordField Name="c_sjno" />
                      <ext:RecordField Name="d_sjdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="c_pin" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="l_status" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="d_sjdate" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />--%>
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="v_gdgFromDesc" DataIndex="v_gdgFromDesc" Header="Gudang Asal" />
                <ext:Column ColumnID="c_sjno" DataIndex="c_sjno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_sjdate" DataIndex="d_sjdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_gdgToDesc" DataIndex="v_gdgToDesc" Header="Gudang Tujuan" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" Width="400" />
                <ext:Column ColumnID="c_pin" DataIndex="c_pin" Header="PIN" Width="75" />
                <ext:CheckColumn ColumnID="l_status" DataIndex="l_status" Header="Status"  Width="75" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{cbGudangFltr}, #{txPOFltr}, #{txDateFltr}, #{cbGudangToFltr}, #{txKeteranganFltr});reloadFilterGrid(#{gridMain});"
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
                                  <ext:Parameter Name="parameters" Value="[['@contains.v_gdgdesc.Contains(@0) || @contains.c_gdg.Contains(@0)', paramTextGetter(#{cbGudangFltr}), '']]" Mode="Raw" />
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
                          <ext:TextField ID="txPOFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                          <ext:ComboBox ID="cbGudangToFltr" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
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
                                  <ext:Parameter Name="parameters" Value="[['@contains.v_gdgdesc.Contains(@0) || @contains.c_gdg.Contains(@0)', paramTextGetter(#{cbGudangToFltr}), '']]" Mode="Raw" />
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
                          <ext:TextField ID="txKeteranganFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
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
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  <asp:PlaceHolder ID="phCtrl" runat="server" />
  <uc1:ReceiveNoteGudangCtrl ID="ReceiveNoteGudangCtrl1" runat="server" />
</asp:Content>
