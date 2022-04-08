<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master.master" 
CodeFile="ClaimBonusAcc.aspx.cs" Inherits="transaksi_bonus_ClaimBonusAcc" %>

<%@ Register Src="ClaimBonusAccRegularCtrl.ascx" TagName="ClaimBonusAccRegularCtrl"
 TagPrefix="uc1" %>
<%@ Register Src="ClaimBonusAccSTTCtrl.ascx" TagName="ClaimBonusAccSTTCtrl"
 TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
 <script type="text/javascript">
   var voidClaimAccDataFromStore = function(rec, hid) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_claimaccno'), hid.getValue(), txt);
                }
              });
          }
        });
   }
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfMode" runat="server" />
  <ext:Hidden ID="hfType" runat="server" />
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
                <Command Handler="if(command == 'Delete') { voidClaimAccDataFromStore(record, #{hfType}); }" />
              </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_claimaccno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_claimaccno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridClaimAcc" runat="server" SkinID="OriginalExtStore" RemoteSort="true">
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
                  <ext:Parameter Name="model" Value="0070" />
                  <ext:Parameter Name="parameters" Value="[['c_claimaccno', paramValueGetter(#{txClaimFltr}) + '%', ''],
                    ['d_claimaccdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_nosup = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String'],
                    ['c_type = @0', paramValueGetter(#{hfType}) , 'System.String']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_claimaccno">
                    <Fields>
                      <ext:RecordField Name="c_claimaccno" />
                      <ext:RecordField Name="d_claimaccdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="c_claimnoprinc" />
                      <ext:RecordField Name="d_claimdateprinc" Type="Date" DateFormat="M$"/>
                      <ext:RecordField Name="c_claimno" />
                      <ext:RecordField Name="v_nama" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_claimaccno" Direction="DESC" />
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
                <ext:Column ColumnID="c_claimaccno" DataIndex="c_claimaccno" Header="Nomor" Hideable="false" Width="150" />
                <ext:DateColumn ColumnID="d_claimaccdate" DataIndex="d_claimaccdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="c_claimnoprinc" DataIndex="c_claimnoprinc" Header="No Prinsipal" />
                <ext:DateColumn ColumnID="d_claimdateprinc" DataIndex="d_claimdateprinc" Header="Tanggal Prinsipal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="c_claimno" DataIndex="c_claimno" Header="No Claim" Width="200" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Prinsipal" Width="200" />
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
                                <Click Handler="clearFilterGridHeader(#{gridMain}, #{txRSFltr}, #{txDateFltr}, #{cbSuplierFltr});reloadFilterGrid(#{gridMain});"
                                  Buffer="300" Delay="300" />
                              </Listeners>
                            </ext:Button>
                          </Component>
                        </ext:HeaderColumn>
                        <ext:HeaderColumn>
                          <Component>
                            <ext:TextField ID="txClaimFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                        <ext:HeaderColumn/> 
                        <ext:HeaderColumn/> 
                        <ext:HeaderColumn/> 
                        <ext:HeaderColumn>
                          <Component>
                            <ext:ComboBox ID="cbSuplierFltr" runat="server" DisplayField="v_nama" ValueField="c_nosup"
                              Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                              AllowBlank="true" ForceSelection="false">
                              <Store>
                                <ext:Store runat="server">
                                  <CustomConfig>
                                    <ext:ConfigItem Name="allowBlank" Value="true" />
                                  </CustomConfig>
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
                                  ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplierFltr}), '']]" Mode="Raw" />
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
  <asp:PlaceHolder ID="phCtrl" runat="server" />
</asp:Content>