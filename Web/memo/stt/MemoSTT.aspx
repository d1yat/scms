<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MemoSTT.aspx.cs" Inherits="memo_stt_MemoSTT"
  MasterPageFile="~/Master.master" %>

<%@ Register Src="MemoSTTDonasiCtrl.ascx" TagName="MemoSTTDonasiCtrl" TagPrefix="uc" %>
<%@ Register Src="MemoSTTSampleCtrl.ascx" TagName="MemoSTTSampleCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidMemoDataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_gdg'), rec.get('c_mtno'), txt);
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
              <ext:Button ID="btnPrintPL" runat="server" Text="Cetak" Icon="Printer" />
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
              <Command Handler="if(command == 'Delete') { voidMemoDataFromStore(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_mtno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_mtno" Mode="Raw" />
                  <ext:Parameter Name="GudangID" Value="record.data.c_gdg" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridMemo" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0048" />
                  <ext:Parameter Name="parameters" Value="[['d_mtdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_mtno', paramValueGetter(#{txMemoFltr}) + '%', ''],
                    ['c_gdg = @0', paramValueGetter(#{cbGudangFltr}) , 'System.Char'],
                    ['c_type = @0', paramValueGetter(#{hfType}) , 'System.String'],
                    ['c_nip = @0', paramValueGetter(#{cbKryFltr}) , 'System.String']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader IDProperty="c_mtno" TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                    <Fields>
                      <ext:RecordField Name="c_gdg" />
                      <ext:RecordField Name="c_mtno" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="d_mtdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="c_memo" />
                      <ext:RecordField Name="v_ketTrans" />
                      <ext:RecordField Name="c_type" />
                      <ext:RecordField Name="Nama" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_mtno" Direction="DESC" />
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
                <ext:Column ColumnID="c_mtno" DataIndex="c_mtno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_mtdate" DataIndex="d_mtdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="150" />
                <ext:Column ColumnID="v_ketTrans" DataIndex="v_ketTrans" Header="Tipe" Width="100" />
                <ext:Column ColumnID="c_memo" DataIndex="c_memo" Header="Memo" Width="200" />
                <ext:Column ColumnID="Nama" DataIndex="Nama" Header="Pemohon" Width="200" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{cbGudangFltr}, #{txRNFltr}, #{txDateFltr}, #{txDOFltr}, #{cbSuplierFltr}, #{txDateDOFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txMemoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                            Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
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
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbKryFltr" runat="server" DisplayField="v_nama" ValueField="c_nip"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="500" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
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
                                  <ext:Parameter Name="limit" Value="10" />
                                  <ext:Parameter Name="model" Value="2121" />
                                  <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                    ['@contains.c_nip.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbKryFltr}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_nama" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader IDProperty="c_nip" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                                    <Fields>
                                      <ext:RecordField Name="c_nip" />
                                      <ext:RecordField Name="v_nama" />
                                    </Fields>
                                  </ext:JsonReader>
                                </Reader>
                              </ext:Store>
                            </Store>
                            <Template ID="Template1" runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="1" style="width: 500px">
                              <tr><td class="body-panel">NIP</td><td class="body-panel">Nama</td></tr>
                              <tpl for="."><tr class="search-item">
                                  <td>{c_nip}</td><td>{v_nama}</td>
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
  <uc:MemoSTTDonasiCtrl ID="MemoSTTDonasiCtrl" runat="server" />
  <uc:MemoSTTSampleCtrl ID="MemoSTTSampleCtrl" runat="server" />
</asp:Content>
