<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReturSupplier.aspx.cs" Inherits="transaksi_retur_ReturSupplier"
  MasterPageFile="~/Master.master" %>

<%@ Register Src="ReturSupplierPembelianCtrl.ascx" TagName="ReturSupplierPembelianCtrl"
  TagPrefix="uc" %>
<%@ Register Src="ReturSupplierRepackCtrl.ascx" TagName="ReturSupplierRepackCtrl"
  TagPrefix="uc" %>
<%@ Register Src="ReturSupplierPrintCtrl.ascx" TagName="ReturSupplierPrintCtrl" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidRSDataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_rsno'), rec.get('c_gdg'), txt);
                }
              });
          }
        });
    }

    var prepareCommandsParentRS = function(record, toolbar) {
        var accp = toolbar.items.get(0); // accept button

        var isSubmitRS = false;

        if (!Ext.isEmpty(record)) {
            isSubmitRS = record.get('l_confirm');
        }

        if (isSubmitRS) {
            accp.setVisible(true);
        }
        else {
            accp.setVisible(false);
        }
    }

  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfType" runat="server" />
  <ext:Hidden ID="hfTypeName" runat="server" />
  <ext:Hidden ID="hfMode" runat="server" />
  <ext:Hidden ID="hfConfirm" runat="server" />
  <ext:Hidden ID="hfGudang" runat="server" />  
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
              <ext:Button ID="btnPrintRS" runat="server" Text="Cetak" Icon="Printer">
                <DirectEvents>
                  <Click OnEvent="btnPrintRS_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
              <ext:ToolbarSeparator />
              <ext:Button ID="Button1" runat="server" Text="Segarkan" Icon="ArrowRefresh">
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
              <Command Handler="if(command == 'Delete') { voidRSDataFromStore(record, #{DirectMethods}); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_rsno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_rsno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <%--<ext:RowSelectionModel SingleSelect="true" />--%>              
              <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridRS" runat="server" SkinID="OriginalExtStore" RemoteSort="true">
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
                  <ext:Parameter Name="model" Value="0051" />
                  <ext:Parameter Name="parameters" Value="[['c_rsno', paramValueGetter(#{txRSFltr}) + '%', ''],
                  ['d_rsdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                  ['c_nosup = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String'],
                  ['c_gdg = @0', paramValueGetter(#{cbGudangFltr}) , 'System.Char'],
                  ['c_type = @0', paramValueGetter(#{hfType}) , 'System.String'],
                  ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_rsno">
                    <Fields>
                      <ext:RecordField Name="c_gdg" />
                      <ext:RecordField Name="c_rsno" />
                      <ext:RecordField Name="d_rsdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="v_descTrans" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="l_confirm" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_rsno" Direction="DESC" />
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
                <ext:Column ColumnID="c_rsno" DataIndex="c_rsno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_rsdate" DataIndex="d_rsdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" />
                <ext:Column ColumnID="v_descTrans" DataIndex="v_descTrans" Header="Tipe" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" Width="250" />
                <ext:CommandColumn ColumnID="l_confirm" DataIndex="l_confirm" Header="Disposisi Full" ButtonAlign="Center">
                    <Commands>
                        <ext:GridCommand Icon="Accept" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommandsParentRS(record, toolbar);" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txRSFltr}, #{txDateFltr}, #{cbSuplierFltr}, #{cbGudangFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txRSFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                          <ext:ComboBox ID="cbGudangFltr" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="200" ListWidth="200" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false"
                            TypeAhead="false">
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
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudangFltr}), '']]"
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
                            <Template ID="Template2" runat="server">
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
                            <Template ID="Template1" runat="server">
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
                      <ext:HeaderColumn>
                        <Component>
                            <ext:Button ID="Button4" runat="server" ToolTip="RS sudah disposisi Full" Icon="Accept">
                                <DirectEvents>
                                    <Click OnEvent="SubmitSelection" Success = "refreshGrid(#{gridMain});">
                                    <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin proses." />
                                    <EventMask ShowMask="true" />
                                        <ExtraParams>
                                            <ext:Parameter Name="Values" Value="Ext.encode(#{gridMain}.getRowsValues({selectedOnly:true}))"
                                                Mode="Raw" />
                                            <ext:Parameter Name="BtnTipe" Value="1" Mode="Raw" />
                                            <ext:Parameter Name="katcol" Value="1" Mode="Raw" />
                                        </ExtraParams>
                                    </Click>
                                </DirectEvents>
                                <%--<Listeners>
                                  <Click Handler="refreshGrid(#{gridMain});" />
                                </Listeners>--%>
                            </ext:Button>
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
  <uc:ReturSupplierPrintCtrl ID="ReturSupplierPrintCtrl1" runat="server" />
</asp:Content>
