<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LimitPOItem.aspx.cs"
  Inherits="transaksi_pembelian_LimitPOItem" MasterPageFile="~/Master.master" %>

<%@ Register Src="LimitPOItemCtrl.ascx" TagName="LimitPOItemCtrl"
  TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="FitLayout">
  <Items>
    <ext:Panel ID="Panel1" runat="server">
      <Items>
        <ext:BorderLayout ID="bllayout" runat="server">
          <North MinHeight="75" MaxHeight="125" Collapsible="false">
            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Border="false"
              Padding="5" Height="100">
              <Items>
                <ext:SelectBox ID="cbPeriode1" runat="server" Width="75" AllowBlank="false" FieldLabel="Tahun">
                    <Listeners>
                        <Change Handler="reloadFilterGrid(#{gridMain});" />
                    </Listeners>
                </ext:SelectBox>
                <ext:SelectBox ID="cbPeriode2" runat="server" Width="75" AllowBlank="false" FieldLabel="Bulan">
                    <Listeners>
                        <Change Handler="reloadFilterGrid(#{gridMain});" />
                    </Listeners>
                </ext:SelectBox>
                <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
                  DisplayField="v_nama" Width="350" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
                  AllowBlank="true" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store7" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="={10}" />
                        <ext:Parameter Name="model" Value="2021" />
                        <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                            ['l_aktif == @0', true, 'System.Boolean'],
                            ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]" Mode="Raw" />
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
                    <table cellpading="0" cellspacing="0" style="width: 500px">
                  <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                  <tpl for=".">
                    <tr class="search-item">
                      <td>{c_nosup}</td><td>{v_nama}</td>
                    </tr>
                  </tpl>
                  </table>
                    </Html>
                  </Template>
                  <Listeners>
                    <Change Handler="reloadFilterGrid(#{gridMain});" />
                  </Listeners>
                </ext:ComboBox>
              </Items>
            </ext:FormPanel>
          </North>
          <Center>
            <ext:Panel ID="Panel2" runat="server" Layout="FitLayout">
              <Items>
                  <ext:GridPanel ID="gridMain" runat="server">
                    <LoadMask ShowMask="true" />
                    <DirectEvents>
                      <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                        <EventMask ShowMask="true" />
                        <ExtraParams>
                          <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                          <ext:Parameter Name="tahun" Value="record.data.n_tahun" Mode="Raw" />
                          <ext:Parameter Name="bulan" Value="record.data.n_bulan" Mode="Raw" />
                          <ext:Parameter Name="PrimaryID" Value="record.data.c_kddivpri" Mode="Raw" />
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
                          <ext:Parameter Name="model" Value="05006" />
                          <ext:Parameter Name="parameters" Value="[
                              ['n_tahun = @0', paramValueGetter(#{cbPeriode1}) , 'System.Decimal'],
                              ['n_bulan = @0', paramValueGetter(#{cbPeriode2}) , 'System.Decimal'],
                              ['c_nosup = @0', paramValueGetter(#{cbSuplier}) , 'System.String']]"
                                Mode="Raw" />
                        </BaseParams>
                        <Reader>
                          <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                            IDProperty="c_kddivpri">
                            <Fields> 
                              <ext:RecordField Name="n_tahun" />
                              <ext:RecordField Name="n_bulan" />
                              <ext:RecordField Name="c_kddivpri" />
                              <ext:RecordField Name="v_nmdivpri" />
                              <ext:RecordField Name="v_nama" />
                              <ext:RecordField Name="n_percentage" Type="Float" />
                              <ext:RecordField Name="n_budget" Type="Float" />
                              <ext:RecordField Name="n_balance" Type="Float" />
                              <ext:RecordField Name="l_aktif" Type="Boolean" />
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="v_nmdivpri" Direction="ASC" />
                      </ext:Store>
                    </Store>
                    <ColumnModel>
                      <Columns>
                        <ext:CommandColumn Width="25" Resizable="false">
                          <Commands>
                            <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                          </Commands>
                        </ext:CommandColumn>
                        <ext:Column ColumnID="n_tahun" DataIndex="n_tahun" Header="Tahun" Width="50" />
                        <ext:Column ColumnID="n_bulan" DataIndex="n_bulan" Header="Bulan" Width="60" />
                        <ext:Column ColumnID="c_kddivpri" DataIndex="c_kddivpri" Header="Kode" Width="50" />
                        <ext:Column ColumnID="v_nmdivpri" DataIndex="v_nmdivpri" Header="Nama" Width="200" />
                        <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Principal" Width="200" />
                        <ext:NumberColumn ColumnID="n_percentage" DataIndex="n_percentage" Header="%" Width="100" Format="0.000,0000/i" />
                        <ext:NumberColumn ColumnID="n_budget" DataIndex="n_budget" Header="Budget" Width="100" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_balance" DataIndex="n_balance" Header="Balance" Width="100" Format="0.000,00/i" />
                        <ext:CheckColumn ColumnID="l_aktif" DataIndex="l_aktif" Header="Aktif" Width="50" />
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
                                      <Click Handler="clearFilterGridHeader(#{gridMain}, #{txSupIDFltr}, #{cbPrincipalFltr});reloadFilterGrid(#{gridMain});"
                                        Buffer="300" Delay="300" />
                                    </Listeners>
                                  </ext:Button>
                                </Component>
                              </ext:HeaderColumn>
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
          </Center>
        </ext:BorderLayout>
      </Items>
    </ext:Panel>
  </Items>
</ext:Viewport>
  <uc:LimitPOItemCtrl ID="LimitPOItemCtrl" runat="server" />
</asp:Content>
