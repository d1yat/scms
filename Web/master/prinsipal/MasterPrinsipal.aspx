<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MasterPrinsipal.aspx.cs"
  Inherits="master_prinsipal_MasterPrinsipal" MasterPageFile="~/Master.master" %>

<%@ Register Src="MasterPrinsipalCtrl.ascx" TagName="MasterPrinsipalCtrl" TagPrefix="uc" %>
<%@ Register Src="MasterPrinsipalLeadtime.ascx" TagName="MasterPrinsipalLeadtime" TagPrefix="uc" %>
<%@ Register Src="MasterPrinsipalHistory.ascx" TagName="MasterPrinsipalHistory" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidDataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_nosup'), txt);
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
            <DirectEvents>
              <%--Indra 20180815FM--%>
              <%--<Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">--%>
              <Command OnEvent="gridMainCommand" >
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_nosup" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_nosup" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridItem" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0152" />
                  <ext:Parameter Name="parameters" Value="[['c_nosup', paramValueGetter(#{txSuplIDFltr}) + '%', ''],
                              ['@contains.v_nama.Contains(@0)', paramValueGetter(#{txSuplNameFltr}) , 'System.String']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_nosup">
                    <Fields>
                      <ext:RecordField Name="c_nosup" Type="String" />
                      <ext:RecordField Name="v_nama" Type="String" />
                      <%--<ext:RecordField Name="v_boss" Type="String" />
                      <ext:RecordField Name="v_contact" Type="String" />--%>
                      <%--<ext:RecordField Name="v_alamat1" Type="String" />--%>
                      <%--<ext:RecordField Name="v_telepon1" Type="String" />
                      <ext:RecordField Name="v_fax1" Type="String" />--%>
                      <ext:RecordField Name="l_aktif" Type="Boolean" />
                      <ext:RecordField Name="n_leadtime" Type="String" />
                      <ext:RecordField Name="n_leadtime_akhir" Type="String" />
                      <ext:RecordField Name="c_status" Type="String" />
                      <ext:RecordField Name="v_ket" Type="String" />
                      <ext:RecordField Name="keterangan" Type="Date" DateFormat="M$" />                        
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_nosup" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <%--Indra 20180815FM--%>
                    <ext:GridCommand CommandName="EditLeadtime" Icon="Car" ToolTip-Title="" ToolTip-Text="Ubah leadtime" />
                    <ext:GridCommand CommandName="EditDataPrincipal" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Ubah data principal" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_nosup" DataIndex="c_nosup" Header="Kode Principal" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama Principal" />
                <%--Indra 20180815FM--%>
                <%--<ext:Column ColumnID="v_boss" DataIndex="v_boss" Header="Pemilik" Width="150" />--%>
                <%--<ext:Column ColumnID="v_alamat1" DataIndex="v_alamat1" Header="Alamat" Width="350" />--%>
                <%--<ext:Column ColumnID="v_telepon1" DataIndex="v_telepon1" Header="No Telp." Width="80" />
                <ext:Column ColumnID="v_fax1" DataIndex="v_fax1" Header="No Fax." Width="80" />--%>
                <ext:CheckColumn ColumnID="l_aktif" DataIndex="l_aktif" Header="Aktif"  />
                <ext:Column ColumnID="n_leadtime" DataIndex="n_leadtime" Header="Leadtime Awal" Align="Center" />
                <ext:Column ColumnID="n_leadtime_akhir" DataIndex="n_leadtime_akhir" Header="Leadtime Perubahan" Align ="Center" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Status"  />
                <ext:DateColumn DataIndex="keterangan" Header="Efective Date" Format="dd-MM-yyyy" Width="75" />               
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txSuplIDFltr}, #{txSuplNameFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txSuplIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txSuplNameFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:TextField>                          
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
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
                  <ext:Label runat="server" Text="Page size:" />
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

        <Buttons>
          <%--Indra 20180815FM--%>
          <ext:Button runat="server" ID="Button2" Icon="Disk" Text="History Perubahan Leadtime">
            <DirectEvents>
              <Click OnEvent="btnHistory_OnClick">
                <EventMask ShowMask="true" />
              </Click>
            </DirectEvents>
          </ext:Button>
        </Buttons>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  
  <uc:MasterPrinsipalCtrl ID="MasterPrinsipalCtrl" runat="server" />
  <uc:MasterPrinsipalLeadtime ID="MasterPrinsipalLeadtime" runat="server" />
  <uc:MasterPrinsipalHistory ID="MasterPrinsipalHistory" runat="server" />
  
</asp:Content>

