<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaPO.aspx.cs" Inherits="transaksi_wp_SerahTerimaPO"
  MasterPageFile="~/Master.master" %>

<%@ Register Src="~/transaksi/wp/SerahTerimaPOCtrl.ascx" TagName="SerahTerimaPOCtrl" TagPrefix="uc" %>
<%@ Register Src="~/transaksi/wp/SerahTerimaPOPendingCtrl.ascx" TagName="SerahTerimaPOPendingCtrl" TagPrefix="uc" %>
  
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
              <ext:ToolbarSeparator />
              <ext:Button ID="btnView" runat="server" Text="View Pending" Icon="ApplicationViewIcons">
                <DirectEvents>
                  <Click OnEvent="btnViewPending_OnClick">
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
                  <ext:Parameter Name="model" Value="0340" />
                  <ext:Parameter Name="parameters" Value="[['c_gdg = @0', paramValueGetter(#{hfGdg}) , 'System.Char'],
                                                            ['c_type = @0', '01', ''],
                                                            ['(l_scan == null ? false : l_scan) = @0', 'true' , 'System.Boolean'],
                                                            ['c_nodoc', paramValueGetter(#{txTransFltr}) + '%', ''],
                                                            ['d_wpdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                                                            ['c_urut', paramValueGetter(#{txUrutFltr}) + '%', ''],
                                                            ['c_nosup = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String'],
                                                            ['c_plat', paramValueGetter(#{txPlatFltr}) + '%', ''],                                                                    
                                                            ['c_scan = @0', paramValueGetter(#{cbReceiveFltr}) , 'System.String']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_nodoc">
                    <Fields>
                      <ext:RecordField Name="c_nodoc" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="d_wpdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="c_urut" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="c_plat" />
                      <ext:RecordField Name="v_scan" />
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
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" />
                <ext:Column ColumnID="c_nodoc" DataIndex="c_nodoc" Header="Nomor" />
                <ext:DateColumn ColumnID="d_wpdate" DataIndex="d_wpdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="c_urut" DataIndex="c_urut" Header="No.Antrian" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" />
                <ext:Column ColumnID="c_plat" DataIndex="c_plat" Header="No.Plat" />
                <ext:Column ColumnID="v_scan" DataIndex="v_scan" Header="Receiver" Width="150" />
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
                              <Click Handler="clearFilterGridHeader(#{GridMain}, #{txTransFltr}, #{txDateFltr}, #{txUrutFltr}, #{cbSuplierFltr}, #{txPlatFltr}, #{cbReceiveFltr});reloadFilterGrid(#{gridMain});"
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
                          <ext:TextField ID="txUrutFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
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
                                    <ext:Store ID="Store2" runat="server">
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
                                <Template ID="Template2" runat="server">
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
                          <ext:TextField ID="txPlatFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbReceiveFltr" runat="server" DisplayField="v_nama" ValueField="c_nip"
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
  <uc:SerahTerimaPOCtrl ID="SerahTerimaPOCtrl" runat="server" />
  <uc:SerahTerimaPOPendingCtrl ID="SerahTerimaPOPendingCtrl" runat="server" />  
</asp:Content>
