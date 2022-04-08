<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="PurchaseOrder.aspx.cs" Inherits="transaksi_pembelian_PurchaseOrder" %>

<%@ Register Src="PurchaseOrderCtrl.ascx" TagName="PurchaseOrderCtrl" TagPrefix="uc" %>
<%@ Register Src="PurchaseOrderPrintCtrl.ascx" TagName="PurchaseOrderPrintCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidPODataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_gdg'), rec.get('c_pono'), txt);
                }
              });
          }
        });
    }
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
              <ext:Button ID="btnPrintPO" runat="server" Text="Cetak" Icon="Printer">
                <DirectEvents>
                  <Click OnEvent="btnPrintPO_OnClick">
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
              <Command Handler="if(command == 'Delete') { voidPODataFromStore(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if((command != 'Select') && (command != 'Submit')) { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_pono" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_pono" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridPO" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0017" />
                  <ext:Parameter Name="parameters" Value="[['c_orno', paramValueGetter(#{txORFltr}) + '%', ''],
                    ['d_podate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_pono', paramValueGetter(#{txPOFltr}) + '%', ''],
                    ['c_nosup = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String'],
                    ['c_gdg = @0', paramValueGetter(#{cbGudangFltr}) , 'System.Char'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_pono">
                    <Fields>
                      <ext:RecordField Name="c_gdg" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="c_pono" />
                      <ext:RecordField Name="d_podate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="c_orno" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="l_revisi" Type="Boolean" />
                      <ext:RecordField Name="l_print" Type="Boolean" />
                      <ext:RecordField Name="l_send" Type="Boolean" />
                      <ext:RecordField Name="l_sent" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="d_podate" Direction="DESC" />
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
                <ext:Column ColumnID="c_pono" DataIndex="c_pono" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_podate" DataIndex="d_podate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="c_orno" DataIndex="c_orno" Header="Order Request" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" />
                <ext:CheckColumn ColumnID="l_revisi" DataIndex="l_revisi" Header="Revisi" Width="50" />
                <ext:CheckColumn ColumnID="l_print" DataIndex="l_print" Header="Cetak" Width="50" />
                <ext:CheckColumn ColumnID="l_send" DataIndex="l_send" Header="Kirim" Width="50" />
                <ext:CheckColumn ColumnID="l_sent" DataIndex="l_sent" Header="Terkirim" Width="50" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{cbGudangFltr}, #{txPOFltr}, #{txDateFltr}, #{txORFltr}, #{cbSuplierFltr});reloadFilterGrid(#{gridMain});"
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
                                  <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
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
                          <ext:TextField ID="txORFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
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
                                        ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplierFltr}), '']]"
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
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  <uc:PurchaseOrderCtrl ID="PurchaseOrderCtrl1" runat="server" />
  <uc:PurchaseOrderPrintCtrl ID="PurchaseOrderPrintCtrl1" runat="server" />
</asp:Content>
