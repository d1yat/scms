<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="OrderRequestGudang.aspx.cs" Inherits="transaksi_pembelian_OrderRequestGudang" %>

<%@ Register Src="OrderRequestGudangCtrl.ascx" TagName="OrderRequestGudangCtrl" TagPrefix="uc1" %>
<%@ Register Src="OrderRequestProcessGudangCtrl.ascx" TagName="OrderRequestProcessGudangCtrl"
  TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidORDataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_spgno'), txt);
                }
              });
          }
        });
    }

    var submitSGData = function(hf, rec) {
      if (Ext.isEmpty(rec) || Ext.isEmpty(hf)) {
        return;
      }

      ShowConfirm('Proses ?', 'Apakah anda yakin ingin memproses nomor ini ?',
        function(btn) {
          if (btn == 'yes') {
            Ext.net.DirectMethods.SubmitMethod(rec.get('c_spgno'));
          }
        });
    }

    var prepareCommandsParent = function(record, toolbar) {
      var accp = toolbar.items.get(0); // accept button

      var isSubmitOR = false;

      if (!Ext.isEmpty(record)) {
        isSubmitOR = record.get('l_status');
      }

      if (isSubmitOR) {
        accp.setVisible(false);
      }
      else {
        accp.setVisible(true);
      }
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfTypeName" runat="server" />
  <ext:Hidden ID="hfMode" runat="server" />
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
              <%--<ext:Button ID="btnPrintPL" runat="server" Text="Cetak" Icon="Printer" />
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
              <Command Handler="if(command == 'Delete') { voidORDataFromStore(record); } else if(command == 'Submit') { submitSGData(#{hfGudang}, record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_spgno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_spgno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridOR" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0015" />
                  <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                  <ext:Parameter Name="parameters" Value="[['c_spgno', paramValueGetter(#{txSPGFltr}) + '%', ''],
                    ['d_spgdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_type = @0', paramValueGetter(#{cbTipeFltr}) , 'System.String'],
                    ['c_nosup = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String'],
                    ['c_gdg1 = @0', paramValueGetter(#{cbGudangFltrFrom}) , 'System.Char'],
                    ['c_gdg2 = @0', paramValueGetter(#{cbGudangFltrTo}) , 'System.Char'],
                    ['spgComplete = @0', paramValueGetter(#{sbCompleteFltr}) , 'System.Boolean'],
                    ['spgPartial = @0', paramValueGetter(#{sbPartialFltr}) , 'System.Boolean'],
                    ['ETD = @0', paramRawValueGetter(#{txETDDate}) , 'System.DateTime'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_spgno">
                    <Fields>
                      <%--<ext:RecordField Name="c_gdg1" />--%>
                      <%--<ext:RecordField Name="c_gdg2" />--%>
                      <ext:RecordField Name="c_spgno" />
                      <ext:RecordField Name="d_spgdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="v_type_desc" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="v_gudang_from" />
                      <ext:RecordField Name="v_gudang_to" />
                      <ext:RecordField Name="l_status" Type="Boolean" />
                      <ext:RecordField Name="spgComplete" Type="Boolean" />
                      <ext:RecordField Name="spgPartial" Type="Boolean" />
                      <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                      <ext:RecordField Name="ETD" Type="Date" DateFormat="M$" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_spgno" Direction="DESC" />
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
                <ext:Column ColumnID="c_spgno" DataIndex="c_spgno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_spgdate" DataIndex="d_spgdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_gudang_from" DataIndex="v_gudang_from" Header="Asal" />
                <ext:Column ColumnID="v_gudang_to" DataIndex="v_gudang_to" Header="Tujuan" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" />
                <ext:Column ColumnID="v_type_desc" DataIndex="v_type_desc" Header="Tipe" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" />
                <ext:CheckColumn ColumnID="spgComplete" DataIndex="spgComplete" Header="Selesai"
                  Width="50" />
                <ext:CheckColumn ColumnID="spgPartial" DataIndex="spgPartial" Header="Parsial" Width="50" />
                <ext:CommandColumn ColumnID="l_status" DataIndex="l_status" Header="Proses" Width="50"
                  ButtonAlign="Center">
                  <Commands>
                    <ext:GridCommand CommandName="Submit" Icon="Accept" ToolTip-Title="" ToolTip-Text="Konfirmasi permintaan gudang" />
                  </Commands>
                  <PrepareToolbar Handler="prepareCommandsParent(record, toolbar);" />
                </ext:CommandColumn>
                <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                <ext:DateColumn ColumnID="ETD" DataIndex="ETD" Header="ETD" Format="dd-MM-yyyy" />
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
                              <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                              <%--<Click Handler="clearFilterGridHeader(#{gridMain}, #{txSPGFltr}, #{txDateFltr}, #{cbTipeFltr}, #{cbSuplierFltr}, #{sbCompleteFltr}, #{sbPartialFltr}, #{cbGudangFltrFrom}, #{cbGudangFltrTo});reloadFilterGrid(#{gridMain});" Buffer="300" Delay="300" />--%>
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txSPGFltr}, #{txDateFltr}, #{cbTipeFltr}, #{cbSuplierFltr}, #{sbCompleteFltr}, #{sbPartialFltr}, #{cbGudangFltrFrom}, #{cbGudangFltrTo}, #{txETDDate});reloadFilterGrid(#{gridMain});" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>   
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txSPGFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                          <ext:ComboBox ID="cbGudangFltrFrom" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="300" AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store1" runat="server" RemotePaging="false">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="allQuery" Value="true" />
                                  <ext:Parameter Name="model" Value="2031" />
                                  <ext:Parameter Name="parameters" Value="[[]]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="c_gdg" />
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbGudangFltrTo" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="300" AllowBlank="true" ForceSelection="false">
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
                                  <ext:Parameter Name="model" Value="2031" />
                                  <ext:Parameter Name="parameters" Value="[[]]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="c_gdg" />
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
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbTipeFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                            ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3" AllowBlank="true"
                            ForceSelection="false">
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
                                  <ext:Parameter Name="model" Value="2001" />
                                  <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '48', 'System.String'],
                                    ['c_portal = @0', '3', 'System.Char']]" Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="c_notrans" />
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
                            <Template runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="1" style="width: 200px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                              <tpl for="."><tr class="search-item">
                              <td>{c_type}</td><td>{v_ket}</td>
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
                      <ext:HeaderColumn>
                        <Component>
                          <ext:SelectBox ID="sbCompleteFltr" runat="server" ListWidth="50">
                            <Items>
                              <ext:ListItem Text="&nbsp;" Value="" />
                              <ext:ListItem Text="Ya" Value="true" />
                              <ext:ListItem Text="Tdk" Value="false" />
                            </Items>
                            <Listeners>
                              <Select Handler="reloadFilterGrid(#{gridMain})" Buffer="100" Delay="100" />
                            </Listeners>
                          </ext:SelectBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:SelectBox ID="sbPartialFltr" runat="server" ListWidth="50">
                            <Items>
                              <ext:ListItem Text="&nbsp;" Value="" />
                              <ext:ListItem Text="Ya" Value="true" />
                              <ext:ListItem Text="Tdk" Value="false" />
                            </Items>
                            <Listeners>
                              <Select Handler="reloadFilterGrid(#{gridMain})" Buffer="100" Delay="100" />
                            </Listeners>
                          </ext:SelectBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                      <ext:HeaderColumn>
                        <Component>
                            <ext:DateField ID="txETDDate" runat="server" Format="dd-MM-Y" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                    <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                                </Listeners>
                            </ext:DateField>
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
  <asp:PlaceHolder ID="phCtrl" runat="server" />
  <%--<uc1:OrderRequestGudangCtrl ID="OrderRequestGudangCtrl1" runat="server" />--%>
  <%--<uc2:OrderRequestProcessGudangCtrl ID="OrderRequestProcessGudangCtrl1" runat="server" />--%>
</asp:Content>
