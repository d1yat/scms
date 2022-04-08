<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LimitCtrl.ascx.cs" Inherits="master_budget_LimitCtrl" %>
<%@ Register Src="LimitEntryCtrl.ascx" TagName="LimitEntryCtrl" TagPrefix="uc" %>

<script language="javascript" type="text/javascript">
  var onSelectGridChanged = function(i, g, hf1, hf2) {
    var r = i.getSelected();
    var stor = g.getStore();

    if (!Ext.isEmpty(r)) {
      var thn = (r.get('n_tahun') || 0);
      var bln = (r.get('n_bulan') || 0);

      hf1.setValue(thn);
      hf2.setValue(bln);

      if (!Ext.isEmpty(stor)) {
        stor.removeAll();
        stor.reload();
      }
    }
  }
  var prepareCommands = function(rec, toolbar) {
    var brow = toolbar.items.get(0); // delete button
    var isDelete = false;

    if (!Ext.isEmpty(rec)) {
      isDelete = rec.get('l_delete');
    }

    brow.setVisible((!isDelete));
  }
  var storeResetDetail = function(g) {
    var stor = g.getStore();
    if (!Ext.isEmpty(stor)) {
      stor.removeAll();
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="550" Width="800" Hidden="true"
  Maximizable="true" MinHeight="400" MinWidth="600" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfSuplier" runat="server" />
    <ext:Hidden ID="hfSuplierName" runat="server" />
    <ext:Hidden ID="hfTahun" runat="server" />
    <ext:Hidden ID="hfBulan" runat="server" />
    <ext:Panel ID="pnlDynCtrl" runat="server" />
  </Content>
  <Items>
    <ext:Panel ID="pnlWindow" runat="server" Layout="Fit">
      <Items>
        <ext:BorderLayout ID="bllayout" runat="server">
          <North MinHeight="150" Collapsible="false" Split="true">
            <ext:Panel ID="pnlHdr" runat="server" Height="150" Layout="Fit">
              <TopBar>
                <ext:Toolbar ID="Toolbar1" runat="server">
                  <Items>
                    <ext:Button ID="btnAddNew" runat="server" Text="Tambah" Icon="Add">
                      <DirectEvents>
                        <Click OnEvent="btnAddNew_OnClick">
                          <EventMask ShowMask="true" />
                          <ExtraParams>
                            <ext:Parameter Name="PrimaryID" Value="#{hfSuplier}.getValue()" Mode="Raw" />
                            <ext:Parameter Name="PrimaryNameID" Value="#{hfSuplierName}.getValue()" Mode="Raw" />
                          </ExtraParams>
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
                <ext:GridPanel ID="gridHeaderList" runat="server">
                  <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true">
                      <Listeners>
                        <SelectionChange Handler="onSelectGridChanged(item, #{gridDetail}, #{hfTahun}, #{hfBulan});" />
                      </Listeners>
                    </ext:RowSelectionModel>
                  </SelectionModel>
                  <Store>
                    <ext:Store ID="storeHdrList" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                        <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingDtlHdrBB}.getValue())" Mode="Raw" />
                        <ext:Parameter Name="model" Value="0161" />
                        <ext:Parameter Name="parameters" Value="[['c_nosup = @0', paramValueGetter(#{hfSuplier}, '?') , 'System.String'],
                            ['(n_tahun != @0) || (n_bulan != @0)', 0, 'System.Decimal']]" Mode="Raw" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
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
                      <Listeners>
                        <Load Handler="storeResetDetail(#{gridDetail});" />
                        <LoadException Handler="storeResetDetail(#{gridDetail});" />
                      </Listeners>
                      <SortInfo Field="n_tahun" Direction="DESC" />
                    </ext:Store>
                  </Store>
                  <ColumnModel>
                    <Columns>
                      <ext:CommandColumn Width="25" Resizable="false">
                        <Commands>
                          <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                          <%--<ext:GridCommand CommandName="Reset" Icon="CogStart" ToolTip-Title="" ToolTip-Text="Kalkulasi ulang anggaran" />--%>
                        </Commands>
                      </ext:CommandColumn>
                      <ext:NumberColumn ColumnID="n_tahun" DataIndex="n_tahun" Header="Tahun" Width="50"
                        Format="0000" />
                      <ext:NumberColumn ColumnID="n_bulan" DataIndex="n_bulan" Header="Bulan" Width="50"
                        Format="0000" />
                      <ext:NumberColumn ColumnID="n_limit" DataIndex="n_limit" Header="Batasan" Width="150"
                        Format="0.000,00/i" />
                      <ext:NumberColumn ColumnID="n_avaiblelimit" DataIndex="n_avaiblelimit" Header="Sisa"
                        Width="150" Format="0.000,00/i" />
                      <ext:NumberColumn ColumnID="n_nextlimit" DataIndex="n_nextlimit" Header="% Anggaran"
                        Width="75" Format="0.000,00/i" />
                    </Columns>
                  </ColumnModel>
                  <BottomBar>
                    <ext:PagingToolbar runat="server" ID="gmPagingDtlHdrBB" PageSize="20">
                      <Items>
                        <ext:Label ID="Label1" runat="server" Text="Page size:" />
                        <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                        <ext:ComboBox ID="cbGmPagingDtlHdrBB" runat="server" Width="80">
                          <Items>
                            <ext:ListItem Text="5" />
                            <ext:ListItem Text="10" />
                            <ext:ListItem Text="20" />
                            <ext:ListItem Text="50" />
                            <ext:ListItem Text="100" />
                          </Items>
                          <SelectedItem Value="20" />
                          <Listeners>
                            <Select Handler="#{gmPagingDtlHdrBB}.pageSize = parseInt(this.getValue()); #{gmPagingDtlHdrBB}.doLoad();" />
                          </Listeners>
                        </ext:ComboBox>
                      </Items>
                    </ext:PagingToolbar>
                  </BottomBar>
                  <DirectEvents>
                    <Command Before="if(command != 'Select') { return false; }" OnEvent="OnGridCommand_Click">
                      <EventMask ShowMask="true" />
                      <ExtraParams>
                        <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                        <ext:Parameter Name="SuplierID" Value="record.data.c_nosup" Mode="Raw" />
                        <ext:Parameter Name="SuplierName" Value="record.data.v_nama" Mode="Raw" />
                        <ext:Parameter Name="Tahun" Value="record.data.n_tahun" Mode="Raw" />
                        <ext:Parameter Name="Bulan" Value="record.data.n_bulan" Mode="Raw" />
                        <ext:Parameter Name="Limit" Value="record.data.n_limit" Mode="Raw" />
                        <ext:Parameter Name="Persentase" Value="record.data.n_nextlimit" Mode="Raw" />
                      </ExtraParams>
                    </Command>
                  </DirectEvents>
                </ext:GridPanel>
              </Items>
            </ext:Panel>
          </North>
          <Center MinHeight="200">
            <ext:Panel ID="pnlDtl" runat="server" Layout="Fit">
              <Items>
                <ext:GridPanel ID="gridDetail" runat="server">
                  <LoadMask ShowMask="true" />
                  <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                  </SelectionModel>
                  <Store>
                    <ext:Store ID="storeDtl" runat="server">
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
                        <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingDtlDtlBB}.getValue())" Mode="Raw" />
                        <ext:Parameter Name="model" Value="0162" />
                        <ext:Parameter Name="parameters" Value="[['tahun', paramValueGetter(#{hfTahun}, (new Date()).getFullYear()), 'System.Decimal'],
                              ['bulan', paramValueGetter(#{hfBulan}, (new Date()).getMonth()) , 'System.Decimal'],
                              ['supplier', paramValueGetter(#{hfSuplier}, '?') , 'System.String']]" Mode="Raw" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                          <Fields>
                            <ext:RecordField Name="c_pono" />
                            <ext:RecordField Name="n_bilva" Type="Float" />
                            <ext:RecordField Name="c_entry" />
                            <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                            <ext:RecordField Name="c_update" />
                            <ext:RecordField Name="d_update" Type="Date" DateFormat="M$" />
                            <ext:RecordField Name="v_ket" />
                            <ext:RecordField Name="l_delete" Type="Boolean" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                      <SortInfo Field="c_pono" Direction="ASC" />
                    </ext:Store>
                  </Store>
                  <ColumnModel>
                    <Columns>
                      <ext:CommandColumn Width="25" Resizable="false">
                        <Commands>
                          <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                        </Commands>
                        <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                      </ext:CommandColumn>
                      <ext:Column ColumnID="c_pono" DataIndex="c_pono" Header="Pemasok" />
                      <ext:NumberColumn ColumnID="n_bilva" DataIndex="n_bilva" Header="Pembelanjaan" Width="150"
                        Format="0.000,00/i" />
                      <ext:Column ColumnID="c_entry" DataIndex="c_entry" Header="Dibuat" Width="50" />
                      <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Tanggal" Width="75"
                        Format="dd-MM-yyyy" />
                      <ext:Column ColumnID="c_update" DataIndex="c_update" Header="Diperbarui" />
                      <ext:DateColumn ColumnID="d_update" DataIndex="d_update" Header="Tanggal" Width="75"
                        Format="dd-MM-yyyy" />
                      <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" />
                      <ext:CheckColumn ColumnID="l_delete" DataIndex="l_delete" Header="Terhapus" Width="75" />
                    </Columns>
                  </ColumnModel>
                  <BottomBar>
                    <ext:PagingToolbar runat="server" ID="gmPagingDtlDtlBB" PageSize="20">
                      <Items>
                        <ext:Label ID="Label2" runat="server" Text="Page size:" />
                        <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
                        <ext:ComboBox ID="cbGmPagingDtlDtlBB" runat="server" Width="80">
                          <Items>
                            <ext:ListItem Text="5" />
                            <ext:ListItem Text="10" />
                            <ext:ListItem Text="20" />
                            <ext:ListItem Text="50" />
                            <ext:ListItem Text="100" />
                          </Items>
                          <SelectedItem Value="20" />
                          <Listeners>
                            <Select Handler="#{gmPagingDtlDtlBB}.pageSize = parseInt(this.getValue()); #{gmPagingDtlDtlBB}.doLoad();" />
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
      <%--<Buttons>
        <ext:Button ID="btnAddData" runat="server" Text="Tambah" Icon="Add">
          <DirectEvents>
            <Click OnEvent="btnAddNew_OnClick">
              <ExtraParams>
                <ext:Parameter Name="PrimaryID" Value="#{hfSuplier}.getValue()" Mode="Raw" />
                <ext:Parameter Name="PrimaryNameID" Value="#{hfSuplierName}.getValue()" Mode="Raw" />
              </ExtraParams>
              <EventMask ShowMask="true" />
            </Click>
          </DirectEvents>
        </ext:Button>
      </Buttons>--%>
    </ext:Panel>
  </Items>
  <Buttons>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<uc:LimitEntryCtrl ID="LimitEntryCtrl1" runat="server" />
