<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Discount.aspx.cs" Inherits="master_discount_Discount" %>

<%@ Register Src="DiscountCtrl.ascx" TagName="DiscountCtrl" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
              <ext:ComboBox ID="cbTipeTBFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                Width="200" ItemSelector="tr.search-item" ListWidth="225" MinChars="3" AllowBlank="true">
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
                      <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '01', 'System.String'],
                        ['c_portal = @0', '3', 'System.Char'],
                        ['@in.c_type', '[\'01\';\'03\']', 'System.String[]']]" Mode="Raw" />
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
                  <table cellpading="0" cellspacing="1" style="width: 225px">
                  <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                  <tpl for="."><tr class="search-item">
                  <td>{c_type}</td><td>{v_ket}</td>
                  </tr></tpl>
                  </table>
                  </Html>
                </Template>
                <Listeners>
                  <Select Handler="refreshGrid(#{gridMain});" />
                </Listeners>
              </ext:ComboBox>
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
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_nodisc" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_nodisc" Mode="Raw" />
                  <ext:Parameter Name="ItemID" Value="record.data.c_iteno" Mode="Raw" />
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
                  <ext:Parameter Name="model" Value="0111" />
                  <ext:Parameter Name="parameters" Value="[['typeDiscount', paramValueGetter(#{cbTipeTBFltr}), 'System.String'],
                    ['v_itnam', paramValueGetter(#{txItemNamaFltr}) + '%', ''],
                    ['d_start = @0', paramValueGetter(#{txDateMulaiFltr}), 'System.DateTime'],
                    ['d_finish = @0', paramValueGetter(#{txDateAkhirFltr}), 'System.DateTime']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_iteno">
                    <Fields>
                      <ext:RecordField Name="c_nodisc" />
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="v_itnam" />
                      <ext:RecordField Name="d_start" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="d_finish" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="n_discon" Type="Float" />
                      <ext:RecordField Name="n_discoff" Type="Float" />
                      <ext:RecordField Name="l_aktif" Type="Boolean" />
                      <ext:RecordField Name="l_status" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="v_itnam" Direction="ASC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_nodisc" DataIndex="c_nodisc" Header="Nomor" />
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama" Width="250" />
                <ext:DateColumn ColumnID="d_start" DataIndex="d_start" Header="Mulai" Format="dd-MM-yyyy" />
                <ext:DateColumn ColumnID="d_finish" DataIndex="d_finish" Header="Berakhir" Format="dd-MM-yyyy" />
                <ext:NumberColumn ColumnID="n_discon" DataIndex="n_discon" Header="On" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_discoff" DataIndex="n_discoff" Header="Off" Format="0.000,00/i" />
                <ext:CheckColumn ColumnID="l_aktif" DataIndex="l_aktif" Header="Aktif" Width="50" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txItemNamaFltr}, #{txDateMulaiFltr}, #{txDateAkhirFltr}, #{txRefIDFltr}, #{cbSuplierFltr}, #{txVcIDFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txItemNamaFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDateMulaiFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDateAkhirFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>
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
  <uc:DiscountCtrl ID="DiscountCtrl1" runat="server" />
</asp:Content>
