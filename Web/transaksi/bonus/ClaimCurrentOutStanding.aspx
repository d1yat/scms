<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClaimCurrentOutStanding.aspx.cs" Inherits="transaksi_bonus_ClaimCurrentOutStanding"
MasterPageFile="~/Master.master" %>

<%@ Register Src="ClaimCurrentOutStandingCtrl.ascx" TagName="ClaimCurrentOutStandingCtrl"
 TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
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
            <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                <ext:Parameter Name="Parameter" Value="c_iteno" />
                <ext:Parameter Name="PrimaryID" Value="record.data.c_iteno" Mode="Raw" />
                <ext:Parameter Name="PrimaryName" Value="record.data.v_itnam" Mode="Raw" />
                <ext:Parameter Name="Jml" Value="record.data.n_sisa" Mode="Raw" />
              </ExtraParams>
            </Command>
          </DirectEvents>
          <SelectionModel>
            <ext:RowSelectionModel SingleSelect="true" />
          </SelectionModel>
          <Store>
            <ext:Store ID="storeGridClaim" runat="server" SkinID="OriginalExtStore" RemoteSort="true">
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
                <ext:Parameter Name="model" Value="0159" />
                <ext:Parameter Name="parameters" Value="[['c_iteno', paramValueGetter(#{txKodeFltr}) + '%', ''],
                  ['v_itnam', paramValueGetter(#{txNamaFltr}) + '%', '']]" Mode="Raw" />
              </BaseParams>
              <Reader>
                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                  IDProperty="c_iteno">
                  <Fields>
                    <ext:RecordField Name="c_iteno" />
                    <ext:RecordField Name="n_sisa" Type="Float"/>
                    <ext:RecordField Name="v_itnam" />
                    <ext:RecordField Name="v_nama" />
                    <ext:RecordField Name="c_nosup" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
              <SortInfo Field="c_iteno" Direction="DESC" />
            </ext:Store>
          </Store>
          <ColumnModel>
            <Columns>
              <ext:CommandColumn Width="50" Resizable="false">
                <Commands>
                  <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                </Commands>
              </ext:CommandColumn>
              <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode Item" Width="100" />
              <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Item" Width="250"/>
              <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" Width="250" />
              <ext:NumberColumn ColumnID="n_sisa" DataIndex="n_sisa" Header="Quantity" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txKodeFltr}, #{txNamaFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txKodeFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNamaFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
<asp:PlaceHolder ID="phCtrl" runat="server" />
<uc1:ClaimCurrentOutStandingCtrl runat="server" ID="ClaimCurrentOutStandingCtrl" />
</asp:Content>