<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LimitPODivPri.aspx.cs"
  Inherits="transaksi_pembelian_LimitPODivPri" MasterPageFile="~/Master.master" %>

<%@ Register Src="LimitPODivPriCtrl.ascx" TagName="LimitPODivPriCtrl"
  TagPrefix="uc" %>
  

  
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

<script type="text/javascript">

    var recalcHdr = function(store) {

        var n_avgslsval = 0,
        n_salesmtd = 0,
        n_percentage = 0,
        limit = 0,
        avaiblelimit = 0,
        n_posisaqtyreal = 0,
        n_pomtdqtyreal = 0;

        store.each(function(r) {
            if (r.get('l_total') == false) {
                //calc total
                n_avgslsval += r.get('n_avgslsval');
                n_salesmtd += r.get('n_salesmtd');
                n_percentage += r.get('n_percentage');
                limit += r.get('limit');
                avaiblelimit += r.get('avaiblelimit');
                n_posisaqtyreal += r.get('n_posisaqtyreal');
                n_pomtdqtyreal += r.get('n_pomtdqtyreal');
            }
        });

        //insert total

        var rec = '';

        idxTotal = store.findExact('l_total', true);
        rec = store.getAt(idxTotal);

        if (idxTotal != -1) {
            rec.set('n_avgslsval', n_avgslsval);
            rec.set('n_salesmtd', n_salesmtd);
            rec.set('n_percentage', n_percentage);
            rec.set('limit', limit);
            rec.set('avaiblelimit', avaiblelimit);
            rec.set('n_posisaqtyreal', n_posisaqtyreal);
            rec.set('n_pomtdqtyreal', n_pomtdqtyreal);
        }
        else {

            var gridcount = store.getCount();

            store.insert(gridcount, new Ext.data.Record({
                'n_avgslsval': n_avgslsval,
                'n_salesmtd': n_salesmtd,
                'n_percentage': n_percentage,
                'limit': limit,
                'avaiblelimit': avaiblelimit,
                'n_posisaqtyreal': n_posisaqtyreal,
                'n_pomtdqtyreal': n_pomtdqtyreal,
                'l_total': true
            }));
        }
    }

    var prepareCommands = function(rec, toolbar) {
        if (rec.get('l_total') == true) {
            var det = toolbar.items.get(0)
            det.setVisible(false);
        }
    }

    var getRowClassHdr = function(record) {
        var cStatus = record.get('l_total');

        if (cStatus == true) {
            return "gray";
        }
    }


</script>


<style type="text/css">
        .gray {
	        background: #C0C0C0;
        }
</style>

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
                  <ext:Parameter Name="Parameter" Value="c_nosup" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_nosup" Mode="Raw" />
                  <ext:Parameter Name="tahun" Value="record.data.n_tahun" Mode="Raw" />
                  <ext:Parameter Name="bulan" Value="record.data.n_bulan" Mode="Raw" />
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
                <BaseParams>
                  <ext:Parameter Name="start" Value="0" />
                  <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                  <ext:Parameter Name="model" Value="05008" />
                  <ext:Parameter Name="parameters" Value="[
                            ['n_tahun = @0', paramValueGetter(#{cbPeriode1}) , 'System.Decimal'],
                            ['n_bulan = @0', paramValueGetter(#{cbPeriode2}) , 'System.Decimal'],
                            ['c_nosup = @0', paramValueGetter(#{cbSuplier}) , 'System.String']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_nosup">
                    <Fields>
                      <ext:RecordField Name="c_nosup" Type="String" />
                      <ext:RecordField Name="v_nama" Type="String" />
                      <ext:RecordField Name="n_tahun" Type="Float" />
                      <ext:RecordField Name="n_bulan" Type="Float" />
                      <ext:RecordField Name="n_avgslsval" Type="Float" />
                      <ext:RecordField Name="n_salesmtd" Type="Float" />
                      <ext:RecordField Name="n_percentage" Type="Float" />
                      <ext:RecordField Name="limit" Type="Float" />
                      <ext:RecordField Name="avaiblelimit" Type="Float" />
                      <ext:RecordField Name="d_nextlimit" Type="Float" />
                      <ext:RecordField Name="n_posisaqtyreal" Type="Float" />
                      <ext:RecordField Name="n_pomtdqtyreal" Type="Float" />
                      <ext:RecordField Name="n_balance" Type="Float" />
                      <ext:RecordField Name="l_aktif" Type="Boolean" />
                      <ext:RecordField Name="l_total" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <Listeners>
                    <Load Handler="recalcHdr(#{gridMain}.getStore());" />
                </Listeners>
                <SortInfo Field="v_nama" Direction="ASC" />
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
                <ext:Column ColumnID="c_nosup" DataIndex="c_nosup" Header="Kode" Width="100" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Supplier" Width="250" />
                <ext:Column ColumnID="n_tahun" DataIndex="n_tahun" Header="Tahun" Width="100" />
                <ext:Column ColumnID="n_bulan" DataIndex="n_bulan" Header="Bulan" Width="100" />
                <ext:NumberColumn ColumnID="n_avgslsval" DataIndex="n_avgslsval" Header="Avg. Sales" Width="100" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_salesmtd" DataIndex="n_salesmtd" Header="Sales MTD" Width="100" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_percentage" DataIndex="n_percentage" Header="Sales Contribution (%)" Width="100" Format="0.000,0000/i" />
                <ext:NumberColumn ColumnID="limit" DataIndex="limit" Header="Limit" Width="120" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="avaiblelimit" DataIndex="avaiblelimit" Header="Sisa Limit" Width="120" Format="0.000,00/i" />
                <ext:Column ColumnID="d_nextlimit" DataIndex="d_nextlimit" Header="Next Limit %" Width="100" />
                <ext:NumberColumn ColumnID="n_posisaqtyreal" DataIndex="n_posisaqtyreal" Header="PO Pending All" Width="100" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_pomtdqtyreal" DataIndex="n_pomtdqtyreal" Header="PO Pending MTD" Width="100" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_balance" DataIndex="n_balance" Header="Balance" Width="120" Format="0.000,00/i" />
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
                    </Columns>
                  </ext:HeaderRow>
                </HeaderRows>
                <getRowClass Fn="getRowClassHdr" />
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
                    <SelectedItem Value="100" />
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
  <uc:LimitPODivPriCtrl ID="LimitPODivPriCtrl" runat="server" />
</asp:Content>
