<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Limit.aspx.cs" Inherits="master_budget_Limit" %>

<%@ Register Src="LimitCtrl.ascx" TagName="LimitCtrl" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Content>
      <ext:Hidden ID="hfBulan" runat="server" />
      <ext:Hidden ID="hfTahun" runat="server" />
    </Content>
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar ID="Toolbar1" runat="server">
            <Items>
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
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_nosup" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_nosup" Mode="Raw" />
                  <ext:Parameter Name="PrimaryNameID" Value="record.data.v_nama" Mode="Raw" />
                  <%--<ext:Parameter Name="Tahun" Value="record.data.n_tahun" Mode="Raw" />
                  <ext:Parameter Name="Bulan" Value="record.data.n_bulan" Mode="Raw" />--%>
                </ExtraParams>
              </Command>
            </DirectEvents>
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
                  <ext:Parameter Name="model" Value="0161" />
                  <ext:Parameter Name="parameters" Value="[['tahun', paramValueGetter(#{hfTahun}, (new Date()).getFullYear()), 'System.Decimal'],
                              ['bulan', paramValueGetter(#{hfBulan}, (new Date()).getMonth()) , 'System.Decimal'],
                              ['@contains.v_nama.Contains(@0)', paramRawValueGetter(#{txSuplNameFltr}) , 'System.String']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_nosup">
                    <Fields>
                      <ext:RecordField Name="c_nosup" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="n_tahun" Type="Int" />
                      <ext:RecordField Name="n_bulan" Type="Int" />
                      <ext:RecordField Name="n_limit" Type="Float" />
                      <ext:RecordField Name="n_avaiblelimit" Type="Float" />
                      <ext:RecordField Name="n_nextlimit" Type="Float" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="v_nama" Direction="ASC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" Width="200" />
                <ext:NumberColumn ColumnID="n_tahun" DataIndex="n_tahun" Header="Tahun" Width="50" Format="0000" />
                <ext:NumberColumn ColumnID="n_bulan" DataIndex="n_bulan" Header="Bulan" Width="50" Format="0000" />
                <ext:NumberColumn ColumnID="n_limit" DataIndex="n_limit" Header="Batasan" Width="150" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_avaiblelimit" DataIndex="n_avaiblelimit" Header="Sisa" Width="150" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_nextlimit" DataIndex="n_nextlimit" Header="% Anggaran" Width="75" Format="0.000,00/i" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txSuplNameFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
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
  <uc:LimitCtrl ID="LimitCtrl1" runat="server" />
</asp:Content>
