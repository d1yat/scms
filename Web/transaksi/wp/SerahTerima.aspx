<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SerahTerima.aspx.cs" Inherits="transaksi_wp_SerahTerima"
  MasterPageFile="~/Master.master" %>

<%@ Register Src="~/transaksi/wp/SerahTerimaPickerCtrl.ascx" TagName="SerahTerimaPickerCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaInkJetCtrl.ascx" TagName="SerahTerimaInkJetCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaCheckerCtrl.ascx" TagName="SerahTerimaCheckerCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaPackerCtrl.ascx" TagName="SerahTerimaPackerCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaTransportasiCtrl.ascx" TagName="SerahTerimaTransportasiCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaPendingCtrl.ascx" TagName="SerahTerimaPendingCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaSearchCtrl.ascx" TagName="SerahTerimaSearchCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaRCCtrl.ascx" TagName="SerahTerimaRCCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaPBBRCtrl.ascx" TagName="SerahTerimaPBBRCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaBASPBGdgCtrl.ascx" TagName="SerahTerimaBASPBGdgCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaBASPBAdmCtrl.ascx" TagName="SerahTerimaBASPBAdmCtrl"
  TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaCPRCtrl.ascx" TagName="SerahTerimaCPRCtrl"
  TagPrefix="uc" %>
  
  <%@ Register Src="~/transaksi/wp/SERAHTERIMAPRINTCTRL.ascx" TagName="SERAHTERIMAPRINTCTRL"
  TagPrefix="uc" %>
  
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
<ext:Window ID="wndDown" runat="server" Hidden="true" />
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

    var voidEXPDataFromStore = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?', function(btn) {
        if (btn == 'yes') {
          ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.', function(btnP, txt) {
            if (btnP == 'ok') {
              if (txt.trim().length < 1) {
                txt = 'Kesalahan pemakai.';
              }

              Ext.net.DirectMethods.DeleteMethod(rec.get('c_expno'), txt);
            }
          });
        }
      });
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfGdg" runat="server" />
  <ext:Hidden ID="hfMode" runat="server" />
  <ext:Hidden ID="hfType" runat="server" />
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
                <%--Tambah--%>
              <ext:Button ID="btnAddNew" runat="server" Text="Tambah" Icon="Add">
                <DirectEvents>
                  <Click OnEvent="btnAddNew_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
              
              <%--Catak--%>
              
              <ext:ToolbarSeparator />
                   <ext:Button ID="btnPrintPL" runat="server" Text="Cetak" Icon="Printer">
                    <DirectEvents>
                      <Click OnEvent="btnPrintST_OnClick">
                        <EventMask ShowMask="true" />
                      </Click>
                    </DirectEvents>
                  </ext:Button>
              <ext:ToolbarSeparator />
              
               <%--Segarkan--%>
               
              <ext:Button ID="Button1" runat="server" Text="Segarkan" Icon="ArrowRefresh">
                <Listeners>
                  <Click Handler="refreshGrid(#{gridMain});" />
                </Listeners>
              </ext:Button>
              <ext:ToolbarSeparator />
              <ext:Button ID="btnView" runat="server" Text="View Pending" Icon="ApplicationViewIcons">
                <DirectEvents>
                  <Click OnEvent="btnViewPending_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
              <ext:ToolbarSeparator />
              <ext:Button ID="btnSearch" runat="server" Text="Searching DO/SJ" Icon="ApplicationViewIcons">
                <DirectEvents>
                  <Click OnEvent="btnSearch_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel ID="gridMain" runat="server">
            <LoadMask ShowMask="true" />
            <Listeners>
              <%--<Command Handler="if(command == 'Delete') { voidWPDataFromStore(record); }" />--%>
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_nodoc" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_nodoc" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridWP" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0307" />
                  <ext:Parameter Name="parameters" Value="[['c_gdg = @0', paramValueGetter(#{hfGdg}) , 'System.Char'],
                                                                    ['c_type = @0', paramValueGetter(#{hfType}) , 'System.String'],
                                                                    ['c_nodoc', paramValueGetter(#{txTransFltr}) + '%', ''],
                                                                    ['d_entry = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                                                                    ['c_entry = @0', paramValueGetter(#{cbPenerimaFltr}) , 'System.String'],
                                                                    ['c_give = @0', paramValueGetter(#{cbPenyerahFltr}) , 'System.String']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_nodoc">
                    <Fields>
                      <ext:RecordField Name="c_nodoc" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_entry" />
                      <ext:RecordField Name="v_give" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_nodoc" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />--%>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                  </Commands>
                  <%--<PrepareToolbar Handler="prepareCommands(record, toolbar);" />--%>
                  <%--<PrepareToolbar Handler="prepareCommandsPage(toolbar, #{hfMode}.getValue());" />--%>
                </ext:CommandColumn>
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="150" />
                <ext:Column ColumnID="c_nodoc" DataIndex="c_nodoc" Header="Nomor" Hideable="false"
                  Width="210" />
                <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Tanggal" Format="dd-MM-yyyy"
                  Width="200" />
                <ext:Column ColumnID="v_entry" DataIndex="v_entry" Header="Penerima" Width="150" />
                <ext:Column ColumnID="v_give" DataIndex="v_give" Header="Penyerah" Width="150" />
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
                              <Click Handler="clearFilterGridHeader(#{GridMain}, #{txTransFltr}, #{txDateFltr}, #{cbPenerimaFltr} ,#{cbPenyerahFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txTransFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDateFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbPenerimaFltr" runat="server" DisplayField="v_nama" ValueField="c_nip"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
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
                                  <ext:Parameter Name="model" Value="2171" />
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_nip.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPenerimaFltr}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_nama" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                    TotalProperty="d.totalRows">
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
                              <table cellpading="0" cellspacing="1" style="width: 400px">
                              <tr><td class="body-panel">Nip</td><td class="body-panel">Nama</td></tr>
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
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbPenyerahFltr" runat="server" DisplayField="v_nama" ValueField="c_nip"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
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
                                  <ext:Parameter Name="model" Value="2171" />
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_nip.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPenyerahFltr}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_nama" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                    TotalProperty="d.totalRows">
                                    <Fields>
                                      <ext:RecordField Name="c_nip" />
                                      <ext:RecordField Name="v_nama" />
                                    </Fields>
                                  </ext:JsonReader>
                                </Reader>
                              </ext:Store>
                            </Store>
                            <Template ID="Template2" runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="1" style="width: 400px">
                              <tr><td class="body-panel">Nip</td><td class="body-panel">Nama</td></tr>
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
  <uc:SerahTerimaPickerCtrl ID="SerahTerimaPickerCtrl" runat="server" />
  <uc:SerahTerimaInkJetCtrl ID="SerahTerimaInkJetCtrl" runat="server" />
  <uc:SerahTerimaCheckerCtrl ID="SerahTerimaCheckerCtrl" runat="server" />
  <uc:SerahTerimaPackerCtrl ID="SerahTerimaPackerCtrl" runat="server" />
  <uc:SerahTerimaTransportasiCtrl ID="SerahTerimaTransportasiCtrl" runat="server" />
  <uc:SerahTerimaPendingCtrl ID="SerahTerimaPendingCtrl" runat="server" />
  <uc:SerahTerimaSearchCtrl ID="SerahTerimaSearchCtrl" runat="server" />
  <uc:SerahTerimaRCCtrl ID="SerahTerimaRCCtrl" runat="server" /> 
  <uc:SerahTerimaPBBRCtrl ID="SerahTerimaPBBRCtrl" runat="server" />
  <uc:SerahTerimaBASPBGdgCtrl ID="SerahTerimaBASPBGdgCtrl" runat="server" />
  <uc:SerahTerimaBASPBAdmCtrl ID="SerahTerimaBASPBAdmCtrl" runat="server" />
  <uc:SerahTerimaCPRCtrl ID="SerahTerimaCPRCtrl" runat="server" />
  <uc:SerahTerimaPrintCtrl ID="SERAHTERIMAPRINTCTRL" runat="server" />
  
  
  
</asp:Content>

