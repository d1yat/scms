<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EkspedisiCabang.aspx.cs" 
Inherits="transaksi_pengiriman_EkspedisiCabang" MasterPageFile="~/Master.master" %>

<%@ Register Src="EkspedisiCabangCtrl.ascx" TagName="EkspedisiCabangCtrl" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
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
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
              <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
                <Listeners>
                  <Click Handler="refreshGrid(#{gridMain});" />
                </Listeners>
              </ext:Button>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
        <ext:GridPanel runat="server" ID="gridMain">
          <LoadMask ShowMask="true" />
          <Listeners>
              <Command Handler="if(command == 'Delete') { voidEXPDataFromStore(record); }" />
            </Listeners>
          <DirectEvents>
            <Command OnEvent="GridMainCommand" Before="if(command != 'Select') { return false; }">
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                <ext:Parameter Name="Parameter" Value="c_expno" />
                <ext:Parameter Name="PrimaryID" Value="record.data.c_expno" Mode="Raw" />
                <ext:Parameter Name="SecondaryID" Value="record.data.c_noexpcab" Mode="Raw" />
              </ExtraParams>
            </Command>
          </DirectEvents>
          <SelectionModel>
            <ext:RowSelectionModel SingleSelect="true" />
          </SelectionModel>
          <Store>
              <ext:Store ID="strGridMain" runat="server" RemotePaging="true" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0181" />
                  <ext:Parameter Name="parameters" Value="[['c_expno', paramValueGetter(#{txEXPFltr}) + '%', ''],
                                                           ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                                                           ['c_exp', paramValueGetter(#{cbExpFltr}) + '%', ''],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_expno">
                    <Fields>
                      <ext:RecordField Name="c_cusno" />
                      <ext:RecordField Name="c_noexpcab" />
                      <ext:RecordField Name="d_cabang" />
                      <ext:RecordField Name="t_cabang" />
                      <ext:RecordField Name="l_status" Type="Boolean" />
                      <ext:RecordField Name="c_expno" />
                      <ext:RecordField Name="c_resi" />
                      <ext:RecordField Name="c_exp" />
                      <ext:RecordField Name="d_resi" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_cunam" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="c_noexpcab" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_expno" Direction="DESC" />
              </ext:Store>
            </Store>
           <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="Nomor" Hideable="false" />
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="150" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Ekspedisi" />
                <ext:Column ColumnID="c_resi" DataIndex="c_resi" Header="No Resi" />
                <ext:DateColumn ColumnID="d_resi" DataIndex="d_resi" Header="Tanggal" Format="dd-MM-yyyy hh:mm:ss" Width="150" />
                <ext:Column ColumnID="c_noexpcab" DataIndex="c_noexpcab" Header="No Trans" />
                <ext:CheckColumn ColumnID="l_status" DataIndex="l_status" Header="Status" />
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
                            <Click Handler="clearFilterGridHeader(#{GridMain}, #{txEXPFltr}, #{cbCustomerFltr}, #{cbExpFltr});reloadFilterGrid(#{gridMain});"
                              Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:Button>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:TextField ID="txEXPFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                          <Listeners>
                            <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                          </Listeners>
                        </ext:TextField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                          Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                          AllowBlank="true" ForceSelection="false">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="false" />
                          </CustomConfig>
                          <Store>
                            <ext:Store ID="Store2" runat="server" AutoLoad="false">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="start" Value="={0}" />
                                <ext:Parameter Name="limit" Value="={10}" />
                                <ext:Parameter Name="model" Value="2011" />
                                <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
                                  Mode="Raw" />
                                <ext:Parameter Name="sort" Value="v_cunam" />
                                <ext:Parameter Name="dir" Value="ASC" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
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
                            <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:ComboBox>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:ComboBox ID="cbExpFltr" runat="server" DisplayField="v_ket" ValueField="c_exp"
                          Width="250" TypeAhead="false" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" 
                          AllowBlank="true" ForceSelection="false">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="true" />
                          </CustomConfig>
                          <Store>
                            <ext:Store ID="Store3" runat="server" RemotePaging="false">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="allQuery" Value="true" />
                                <ext:Parameter Name="model" Value="2081" />
                                <ext:Parameter Name="parameters" Value="[['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbExpFltr}), '']]"
                                  Mode="Raw" />
                                <ext:Parameter Name="sort" Value="c_exp" />
                                <ext:Parameter Name="dir" Value="ASC" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader IDProperty="c_exp" Root="d.records" SuccessProperty="d.success"
                                  TotalProperty="d.totalRows">
                                  <Fields>
                                    <ext:RecordField Name="c_exp" />
                                    <ext:RecordField Name="v_ket" />
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                            </ext:Store>
                          </Store>
                          <Template ID="Template2" runat="server">
                            <Html>
                            <table cellpading="0" cellspacing="1" style="width: 200px">
                                    <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                                    <tpl for="."><tr class="search-item">
                                        <td>{c_exp}</td><td>{v_ket}</td>
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
  <uc:EkspedisiCabangCtrl ID="EkspedisiCabangCtrl" runat="server" />
</asp:Content>

