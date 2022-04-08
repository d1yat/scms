<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="SuratPesanan.aspx.cs" Inherits="transaksi_pembelian_SuratPesanan" %>
  
<%@ Register Src="SuratPesananCtrl.ascx" TagName="SuratPesananCtrl" TagPrefix="uc" %>
<%@ Register Src="SuratPesananPrintCtrl.ascx" TagName="SuratPesananPrintCtrl" TagPrefix="uc" %>
<%@ Register Src="HistorySP.ascx" TagName="HistorySP" TagPrefix="uc" %>
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_spno'), txt);
                }
              });
          }
        });
    }

    var getRowClass = function(record) {
        var cStatus = record.get('diffdate');

        if (cStatus != "ND" && cStatus != null) {
            return "magenta";
        }
    }
  </script>

  <style type="text/css">    
    .magenta { 
        background: #FFCC66; 
    }        
  </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfTypeName" runat="server" />
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
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel ID="gridMain" runat="server">
            <LoadMask ShowMask="true" />
            <Listeners>
              <Command Handler="if(command == 'Delete') { voidSPDataFromStore(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command == 'Select' || command == 'History') { return true; } ">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_spno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_spno" Mode="Raw" />
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
                  <ext:Parameter Name="model" Value="0011" />
                  <ext:Parameter Name="parameters" Value="[['c_spno', paramValueGetter(#{txSPFltr}) + '%', ''],
                    ['d_spdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_sp', paramValueGetter(#{txSPCFltr}) + '%', ''],
                    ['c_type = @0', paramValueGetter(#{cbTipeFltr}) , 'System.String'],
                    ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                    ['spComplete = @0', paramValueGetter(#{sbCompleteFltr}) , 'System.Boolean'],
                    ['l_cek = @0', paramValueGetter(#{sbCekFltr}) , 'System.Boolean'],
                    ['spPartial = @0', paramValueGetter(#{sbPartialFltr}) , 'System.Boolean']]" Mode="Raw" />
                                        
                    <%--['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']--%>

                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_spno">
                    <Fields>
                      <ext:RecordField Name="c_spno" />
                      <ext:RecordField Name="d_spdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="c_sp" />
                      <ext:RecordField Name="v_type_desc" />
                      <ext:RecordField Name="v_cunam" />
                      <ext:RecordField Name="spComplete" Type="Boolean" />
                      <ext:RecordField Name="spPartial" Type="Boolean" />
                      <ext:RecordField Name="l_print" Type="Boolean" />
                      <ext:RecordField Name="l_cek" Type="Boolean" />
                      <ext:RecordField Name="l_delete" Type="Boolean" />
                      <%--<ext:RecordField Name="d_spinsert" Type="Date" DateFormat="M$" />--%>
                      <ext:RecordField Name="d_spinsert" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="d_etasp" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="d_etdsp" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="diffdate" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_spno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="46" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                    <ext:GridCommand CommandName="History" Icon="MonitorEdit" ToolTip-Title="" ToolTip-Text="Lihat history perubahan ETD" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_spno" DataIndex="c_spno" Header="Nomor" />
                <ext:DateColumn ColumnID="d_spdate" DataIndex="d_spdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="c_sp" DataIndex="c_sp" Header="Nomor cabang" Hideable="false" Width="150" />
                <ext:Column ColumnID="v_type_desc" DataIndex="v_type_desc" Header="Tipe" />
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="150" />
                <ext:CheckColumn ColumnID="spComplete" DataIndex="spComplete" Header="Selesai" Width="50" />
                <ext:CheckColumn ColumnID="spPartial" DataIndex="spPartial" Header="Parsial" Width="50" />
                <ext:CheckColumn ColumnID="l_cek" DataIndex="l_cek" Header="Cek" Width="50" />
                <ext:CheckColumn ColumnID="l_print" DataIndex="l_print" Header="Cetak" Width="50" />
                <ext:CheckColumn ColumnID="l_delete" DataIndex="l_delete" Header="delete" Width="50" />
                <ext:DateColumn ColumnID="d_spinsert" DataIndex="d_spinsert" Header="Tanggal terima"
                  Format="dd-MM-yyyy HH:mm:ss" Width="150" />
                <%--<ext:DateColumn ColumnID="d_spinsert" DataIndex="d_spinsert" Header="Jam terima" Width="75"
                  Format="HH:mm:ss" />--%>
                <ext:DateColumn ColumnID="d_etdsp" DataIndex="d_etdsp" Header="Tanggal ETD SP" Format="dd-MM-yyyy" />
                <ext:DateColumn ColumnID="d_etasp" DataIndex="d_etasp" Header="Tanggal ETA SP" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="diffdate" DataIndex="diffdate" Header="Deviasi" Width="80" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txSPFltr}, #{txDateFltr}, #{txSPCFltr}, #{cbTipeFltr}, #{cbCustomerFltr}, #{sbCompleteFltr}, #{sbPartialFltr}, #{sbCekFltr});reloadFilterGrid(#{gridMain});"
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
                          <ext:TextField ID="txSPCFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbTipeFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
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
                                  <ext:Parameter Name="model" Value="2001" />
                                  <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '47', 'System.String'],
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
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
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
                          <ext:SelectBox ID="sbCompleteFltr" runat="server" ListWidth="50">
                            <Items>
                              <ext:ListItem Text="&nbsp;" Value="" />
                              <ext:ListItem Text="Ya" Value="true" />
                              <ext:ListItem Text="Tdk" Value="false" />
                            </Items>
                            <Listeners>
                              <Select handler="reloadFilterGrid(#{gridMain})" buffer="100" delay="100" />
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
                              <Select handler="reloadFilterGrid(#{gridMain})" buffer="100" delay="100" />
                            </Listeners>
                          </ext:SelectBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:SelectBox ID="sbCekFltr" runat="server" ListWidth="50">
                            <Items>
                              <ext:ListItem Text="&nbsp;" Value="" />
                              <ext:ListItem Text="Ya" Value="true" />
                              <ext:ListItem Text="Tdk" Value="false" />
                            </Items>
                            <Listeners>
                              <Select handler="reloadFilterGrid(#{gridMain})" buffer="100" delay="100" />
                            </Listeners>
                          </ext:SelectBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <%--<ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDateEntryFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>--%>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                    </Columns>
                  </ext:HeaderRow>
                </HeaderRows>
                <GetRowClass Fn="getRowClass" />
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
  
  <uc:SuratPesananCtrl ID="SuratPesananCtrl1" runat="server" />
  <uc:SuratPesananPrintCtrl ID="SuratPesananPrintCtrl1" runat="server" />
  <uc:HistorySP ID="HistorySP1" runat="server" />
</asp:Content>
