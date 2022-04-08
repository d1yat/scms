<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master.master" CodeFile="ReturCustomer.aspx.cs"
  Inherits="transaksi_retur_ReturCustomer" %>

<%@ Register Src="ReturCustomerCtrl.ascx" TagName="ReturCustomerCtrl" TagPrefix="uc" %>
<%@ Register Src="ReturCustomerProcessCtrl.ascx" TagName="ReturCustomerProcessCtrl"
  TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidRCDataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_rcno'), rec.get('c_gdg'), txt);
                }
              });
          }
        });
      }

      var prepareCommandsParentRC = function(record, toolbar) {
        var accp = toolbar.items.get(0); // accept button

        var isSubmitRC = false;

        if (!Ext.isEmpty(record)) {
          isSubmitRC = record.get('l_sent');
        }

        if (isSubmitRC) {
          accp.setVisible(false);
        }
        else {
          accp.setVisible(true);
        }
      }

      var submitRCData = function(rec) {
        if (Ext.isEmpty(rec)) {
          return;
        }

        ShowConfirm('Kirim ?', 'Apakah anda yakin ingin memproses nomor ini ?',
        function(btn) {
          if (btn == 'yes') {
            Ext.net.DirectMethods.SubmitMethod(rec.get('c_rcno'));
          }
        });
      }

      var selectedSavedRCData = function(rcNumber) {
        Ext.net.DirectMethods.SelectedSavedMethod(rcNumber);
      }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport runat="server" Layout="Fit">
    <Content>
      <ext:Hidden ID="hfMode" runat="server" />
      <ext:Hidden ID="hfGudang" runat="server" />
      <ext:Hidden ID="hfType" runat="server" />
    </Content>
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
              <Command Handler="if(command == 'Delete') { voidRCDataFromStore(record); }else if(command == 'Submit') { submitRCData(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; } ">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_rcno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_rcno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridRC" runat="server" SkinID="OriginalExtStore" RemoteSort="true">
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
                  <ext:Parameter Name="model" Value="0037" />
                  <ext:Parameter Name="parameters" Value="[['c_rcno', paramValueGetter(#{txRCFltr}) + '%', ''],
                    ['d_rcdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_gdg = @0', paramValueGetter(#{hfGudang}), 'System.Char'],
                    ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                    ['pbbrno', paramValueGetter(#{txPBBR}) + '%', ''],
                    ['@contains.c_entry.Contains(@0)', paramValueGetter(#{txNipFltr}) , 'System.String']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_rcno">
                    <Fields>
                      <ext:RecordField Name="c_rcno" />
                      <ext:RecordField Name="c_gdg" />
                      <ext:RecordField Name="d_rcdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="v_cunam" />
                      <%--<ext:RecordField Name="v_ket" />--%>
                      <ext:RecordField Name="c_pin" />
                      <ext:RecordField Name="pbbrno" />
                      <ext:RecordField Name="c_entry" />
                      <ext:RecordField Name="l_send" Type="Boolean" />
                      <ext:RecordField Name="l_sent" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_rcno" Direction="DESC" />
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
                <ext:Column ColumnID="c_rcno" DataIndex="c_rcno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_rcdate" DataIndex="d_rcdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" />
                <%--<ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" />--%>
                <ext:Column ColumnID="c_pin" DataIndex="c_pin" Header="Pin" />
                <ext:Column ColumnID="pbbrno" DataIndex="pbbrno" Header="No PBB / PBR" Width="175" />
                <%--<ext:CheckColumn ColumnID="l_send" DataIndex="l_send" Header="Send" />--%>
                <ext:Column ColumnID="c_entry" DataIndex="c_entry" Header="N I P" />                            
                <ext:CommandColumn ColumnID="l_sent" DataIndex="l_sent" Header="Kirim" Width="50" ButtonAlign="Center">
                <Commands>
                  <ext:GridCommand CommandName="Submit" Icon="Accept" ToolTip-Title="" ToolTip-Text="Kirim RC" />
                </Commands>
                <PrepareToolbar Handler="prepareCommandsParentRC(record, toolbar);" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txRCFltr}, #{txDateFltr}, #{cbGudangFltr}, #{cbCustomerFltr}, #{txPBBR}, #{txNipFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <%--<Component>
                          <ext:ComboBox ID="cbGudangFltr" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250" MinChars="3"
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
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2031" />
                                  <ext:Parameter Name="parameters" Value="[['c_gdg != @0', '3', 'System.Char']]" Mode="Raw" />
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
                        </Component>--%>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txRCFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                                    ['l_stscus = @0', true, 'System.Boolean'],
                                    ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
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
                      <ext:HeaderColumn />
                      
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txPBBR" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
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
  <uc:ReturCustomerCtrl ID="ReturCustomerCtrl" runat="server" />
  <uc:ReturCustomerProcessCtrl ID="ReturCustomerProcessCtrl" runat="server" />
</asp:Content>
