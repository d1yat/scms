<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="InvoiceEkspedisiInternal.aspx.cs" Inherits="transaksi_InvoiceEkspedisiInternal" %>


<%@ Register Src="InvoiceEkspedisiInternalCtrl.ascx" TagName="InvoiceEkspedisiInternalCtrl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
  <script type="text/javascript">
    var voidDataFromMainStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_fbno'), txt);
                }
              });
          }
        });
    }
    </script>
    <script type="text/javascript" language="javascript" src="../../scripts/fakturClientLogic.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
<ext:Hidden ID="hfGdg" runat="server" />
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
              <Command Handler="if(command == 'Delete') { voidDataFromMainStore(record); }" />
            </Listeners>--%>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_ieno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_ieno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridMain" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="04003" />
                  <ext:Parameter Name="parameters" Value="[['c_ieno', paramRawValueGetter(#{txFakturIDFltr}) + '%', ''],
                    ['d_iedate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean'],
                    ['c_gdg = @0', paramValueGetter(#{hfGdg}) , 'System.Char']
                    ]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_ieno">
                    <Fields>
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="c_ieno" />
                      <%--<ext:RecordField Name="c_ie" />--%>
                      <ext:RecordField Name="d_iedate" Type="Date" DateFormat="M$" />
                      <%--<ext:RecordField Name="c_cusno" />--%>
                      <ext:RecordField Name="v_nama_exp" />
                      <ext:RecordField Name="n_bilva_faktur" Type="Float" />
                      <ext:RecordField Name="n_netvol" Type="Float" />
                      <ext:RecordField Name="v_ket" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_ieno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />--%>
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" />
                <ext:Column ColumnID="c_ieno" DataIndex="c_ieno" Header="Nomor" />
                <%--<ext:Column ColumnID="c_ie" DataIndex="c_ie" Header="Faktur" />--%>
                <ext:DateColumn ColumnID="d_iedate" DataIndex="d_iedate" Header="Tanggal" Format="dd-MM-yyyy" />
                <%--<ext:Column ColumnID="c_cusno" DataIndex="c_cusno" Header="Customer" />--%>
                <%--<ext:Column ColumnID="v_nama_exp" DataIndex="v_nama_exp" Header="Ekspedisi"
                  Width="250" />--%>
                <ext:NumberColumn ColumnID="n_bilva_faktur" DataIndex="n_bilva_faktur" Header="Fisik Faktur" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_netvol" DataIndex="n_netvol" Header="N E T" Format="0.000,00/i" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" Width="250" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txFakturIDFltr}, #{txDateFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txFakturIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
  <uc1:InvoiceEkspedisiInternalCtrl ID="InvoiceEkspedisiInternalCtrl1" runat="server" />
</asp:Content>
