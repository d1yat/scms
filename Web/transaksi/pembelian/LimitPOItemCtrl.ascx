<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LimitPOItemCtrl.ascx.cs"
  Inherits="transaksi_pembelian_LimitPOItemCtrl" %>

<script type="text/javascript">
    var beforeEditDataConfirm = function(e, hfPrevBudget) {
        var oldval = e.record.get('n_budget');

        hfPrevBudget.setValue(oldval);
    }

    var afterEditDataConfirm = function(e, store, hfPrefVal) {
        recalc(store);
        
        var prevVal = hfPrefVal,
        newVal = e.record.get('n_budget'),
        availablebudget = e.record.get('n_availablebudget'),
        dif = 0,
        newavailablebudget = 0;

        dif = newVal - prevVal;

        newavailablebudget = availablebudget + dif;

        e.record.set('n_availablebudget', newavailablebudget);

        e.record.set('l_modified', true);
    }
    
    var recalc = function(store) {
        var lblimitalokasi = Ext.getCmp('<%= lbLimitAlokasi.ClientID %>');
        var lblimitdiv = Ext.getCmp('<%= lbLimitDivPri.ClientID %>');

        var limitqty = 0,
        totalLimit = 0,
        limitdivpri = parseFloat(lblimitdiv.getText().replace(".", "").replace(".", "").replace(".", "").replace(".", "").replace(",", ".")),
        salpri = 0,
//        balance = 0,
        pomtd = 0,
        contri = 0,
        idxTotal = 0;

        //calc total
        var avgslsvalue = 0,
        slsmtdvalue = 0,
        slscontri = 0;

        store.each(function(r) {
            if (r.get('l_total') == false) {
                limitqty = r.get('n_budget');
                salpri = r.get('n_salpri');
                pomtd = r.get('n_pomtdqtyreal');
                //        contri = r.get('n_percentage');

                totalLimit += (limitqty * salpri);

//                balance = (limitqty - pomtd) //((limitdivpri / 100) * contri) / salpri;

//                r.set('n_balance', balance);

                //calc total

                avgslsvalue += r.get('n_avgslsval');
                slsmtdvalue += r.get('n_slsqtyvalue');
                slscontri += r.get('n_percentage');
            }
        });

        //insert total

        var rec = '';

        idxTotal = store.findExact('l_total', true);
        rec = store.getAt(idxTotal);

        if (idxTotal != -1) {
            rec.set('n_avgslsval', avgslsvalue);
            rec.set('n_slsqtyvalue', slsmtdvalue);
            rec.set('n_percentage', slscontri);
        }
        else {

            var gridcount = store.getCount();

            store.insert(gridcount, new Ext.data.Record({
                'n_avgslsval': avgslsvalue,
                'n_slsqtyvalue': slsmtdvalue,
                'n_percentage': slscontri,
                'l_total': true
            }));
        }


        if (!Ext.isEmpty(totalLimit)) {
            lblimitalokasi.setText(myFormatNumber(totalLimit));
        }
    }
    
    
    var commandGridFunction = function(rec, comName, wndDC1, gridDC1, hidDC1, wndDC2, gridDC2, hidDC2, wndCab, gridCab, hidCab) {
    var itm = rec.get('c_iteno');
    var store = '';

    switch (comName) {
      case 'DetailSOHDC1':
        if ((!Ext.isEmpty(wndDC1)) && (!Ext.isEmpty(gridDC1))) {
          hidDC1.setValue(itm);

          wndDC1.setTitle(String.format('Detail Stock Good DC1 - {0}', rec.get('v_itnam')));
          wndDC1.hide = false;
          wndDC1.show();

                store = gridDC1.getStore();
                store.removeAll();
                store.reload();
            }
            break;
        case 'DetailSOHDC2':
            if ((!Ext.isEmpty(wndDC2)) && (!Ext.isEmpty(gridDC2))) {
                hidDC2.setValue(itm);

                wndDC2.setTitle(String.format('Detail Stock Good DC2 - {0}', rec.get('v_itnam')));
                wndDC2.hide = false;
                wndDC2.show();

                store = gridDC2.getStore();
                store.removeAll();
                store.reload();
            }
            break;
        case 'DetailSOHCab':
            if ((!Ext.isEmpty(wndCab)) && (!Ext.isEmpty(gridCab))) {
                hidCab.setValue(itm);

                wndCab.setTitle(String.format('Detail Stock Good Cab - {0}', rec.get('v_itnam')));
                wndCab.hide = false;
                wndCab.show();

                store = gridCab.getStore();
                store.removeAll();
                store.reload();
            }
            break;
    }
}

var prepareCommands = function(rec, cmd) {
    if (rec.get('l_total') == true) {
        cmd.hidden = true;
    }
}

var getRowClass = function(record) {
    var cStatus = record.get('l_total');
    
    if (cStatus == true) {
        return "gray";
    }
}

var setDefaultLimit = function(chkDefault, store) {

    var chk = chkDefault.getValue();

    if (chk) {
        store.each(function(r) {
        r.set('n_budgettemp', r.get('n_budget'));
        r.set('n_budget', r.get('n_forecast')); 
        });
    }
    else {
        store.each(function(r) {
        r.set('n_budget', r.get('n_budgettemp'));
        });
    }

    recalc(store);
}

</script>

<style type="text/css">
        .gray {
	        background: #C0C0C0;
        }
</style>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfDivPriNo" runat="server" />
    <ext:Hidden ID="hfTahun" runat="server" />
    <ext:Hidden ID="hfBulan" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfPrevBudget" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit" AutoScroll="true">
                <LoadMask ShowMask="true" />
                <Listeners>
                    <Command Handler="commandGridFunction(record, command, #{winDetailSOHDC1}, #{gpDetailSOHDC1}, #{hidItemDC1}, #{winDetailSOHDC2}, #{gpDetailSOHDC2}, #{hidItemDC2}, #{winDetailSOHCab}, #{gpDetailSOHCab}, #{hidItemCab});" />
                </Listeners>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Store>
                    <ext:Store ID="Store5" runat="server" RemotePaging="false" RemoteSort="false">
                        <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="05007" />
                            <ext:Parameter Name="sort" Value="" />
                            <ext:Parameter Name="dir" Value="" />
                            <ext:Parameter Name="parameters" Value="[
                                ['kddivpri', #{hfDivPriNo}.getValue(), 'System.String'],
                                ['tahun', #{hfTahun}.getValue(), 'System.Decimal'],
                                ['bulan', #{hfBulan}.getValue(), 'System.Decimal'],
                                ['@contains.v_itnam.Contains(@0)', paramRawValueGetter(#{txItemNameFltr}), '']]"
                                Mode="Raw" />
                        </BaseParams>
                        <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                <Fields>
                                    <ext:RecordField Name="c_iteno" />
                                    <ext:RecordField Name="v_itnam" />
                                    <ext:RecordField Name="n_salpri" Type="Float" />
                                    <ext:RecordField Name="n_sohreal1" Type="Float" />
                                    <ext:RecordField Name="n_sohreal2" Type="Float" />
                                    <ext:RecordField Name="n_sohcabreal" Type="Float" />
                                    <ext:RecordField Name="n_sohtotal" Type="Float" />
                                    <ext:RecordField Name="n_avgslsqty" Type="Float" />
                                    <ext:RecordField Name="n_avgslsval" Type="Float" />
                                    <ext:RecordField Name="n_slsqtyreal" Type="Float" />
                                    <ext:RecordField Name="n_slsqtyvalue" Type="Float" />
                                    <ext:RecordField Name="n_percentage" Type="Float" />
                                    <ext:RecordField Name="v_ket" />
                                    <ext:RecordField Name="n_posisaqtyreal" Type="Float" />
                                    <ext:RecordField Name="n_pomtdqtyreal" Type="Float" />
                                    <ext:RecordField Name="n_forecast" Type="Float" />
                                    <ext:RecordField Name="n_budget" Type="Float" />
                                    <ext:RecordField Name="n_budgettemp" Type="Float" />
                                    <ext:RecordField Name="n_ponqty" Type="Float" />
                                    <ext:RecordField Name="n_availablebudget" Type="Float" />
                                    <ext:RecordField Name="l_modified" Type="Boolean" />      
                                    <ext:RecordField Name="l_total" Type="Boolean" />                   
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <Listeners>
                            <Load Handler="recalc(#{gridDetail}.getStore());" />
                        </Listeners>
                      <SortInfo Field="v_itnam" Direction="ASC" />
                    </ext:Store>
                </Store>
                <ColumnModel>
                    <Columns>
                        <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode" Width="50" />
                        <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Barang" Width="200" />
                        <ext:NumberColumn ColumnID="n_salpri" DataIndex="n_salpri" Header="Price" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_sohreal1" DataIndex="n_sohreal1" Header="SOH DC1" Format="0.000,00/i">
                            <Commands>
                              <ext:ImageCommand Icon="Book" CommandName="DetailSOHDC1" ToolTip-Text="Rincian Stock DC!"
                                ToolTip-Title="Command" />
                            </Commands>
                            <PrepareCommand Handler="prepareCommands(record, command);" />
                        </ext:NumberColumn>
                        <ext:NumberColumn ColumnID="n_sohreal2" DataIndex="n_sohreal2" Header="SOH DC2" Format="0.000,00/i">
                            <Commands>
                              <ext:ImageCommand Icon="Book" CommandName="DetailSOHDC2" ToolTip-Text="Rincian Stock DC2"
                                ToolTip-Title="Command" />
                            </Commands>
                            <PrepareCommand Handler="prepareCommands(record, command);" />
                        </ext:NumberColumn>
                        <ext:NumberColumn ColumnID="n_sohcabreal" DataIndex="n_sohcabreal" Header="SOH Branch" Format="0.000,00/i">
                            <Commands>
                              <ext:ImageCommand Icon="Book" CommandName="DetailSOHCab" ToolTip-Text="Rincian Stock Cabang"
                                ToolTip-Title="Command" />
                            </Commands>
                            <PrepareCommand Handler="prepareCommands(record, command);" />
                        </ext:NumberColumn>
                        <ext:NumberColumn ColumnID="n_sohtotal" DataIndex="n_sohtotal" Header="SOH Total" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_avgslsqty" DataIndex="n_avgslsqty" Header="Avg Sales Qty" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_avgslsval" DataIndex="n_avgslsval" Header="Avg Sales Value" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_slsqtyreal" DataIndex="n_slsqtyreal" Header="Sales MTD Qty" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_slsqtyvalue" DataIndex="n_slsqtyvalue" Header="Sales MTD Value" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_percentage" DataIndex="n_percentage" Header="Sales Contribution (%)" Format="0.000,0000/i" />
                        <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Category" Width="100" />
                        <ext:NumberColumn ColumnID="n_posisaqtyreal" DataIndex="n_posisaqtyreal" Header="PO Pending All (Qty)" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_pomtdqtyreal" DataIndex="n_pomtdqtyreal" Header="PO Pending MTD (Qty)" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_forecast" DataIndex="n_forecast" Header="Limit Suggestion" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_budget" DataIndex="n_budget" Header="Limit (Qty)" Format="0.000,00/i">
                            <Editor>
                                <ext:NumberField ID="NumberField3" runat="server" AllowDecimals="true" AllowNegative="false" MinValue="0"
                                DecimalPrecision="2" />
                            </Editor>
                        </ext:NumberColumn>
                        <ext:NumberColumn ColumnID="n_ponqty" DataIndex="n_ponqty" Header="PO MTD (Qty)" Format="0.000,00/i" /> 
                        <ext:NumberColumn ColumnID="n_availablebudget" DataIndex="n_availablebudget" Header="Sisa Limit" Format="0.000,00/i" />     
                        <ext:CheckColumn ColumnID="l_modified"  DataIndex="l_modified" Header="Modify" Width="50" />
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
                                                  <Click Handler="clearFilterGridHeader(#{gridDetail}, #{txItemNameFltr});reloadFilterGrid(#{gridDetail});"
                                                    Buffer="300" Delay="300" />
                                                </Listeners>
                                              </ext:Button>
                                            </Component>
                                          </ext:HeaderColumn>
                                          <ext:HeaderColumn>
                                            <Component>
                                              <ext:TextField ID="txItemNameFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                <Listeners>
                                                  <KeyUp Handler="reloadFilterGrid(#{gridDetail})" Buffer="300" Delay="300" />
                                                </Listeners>
                                              </ext:TextField>
                                            </Component>
                                          </ext:HeaderColumn>                             
                                        </Columns>
                                    </ext:HeaderRow>
                                </HeaderRows>
                                <GetRowClass Fn="getRowClass" /> 
                            </ext:GridView>
                        </View>
                <Listeners>
                    <BeforeEdit Handler="beforeEditDataConfirm(e, #{hfPrevBudget});" />
                    <AfterEdit Handler="afterEditDataConfirm(e, #{gridDetail}.getStore(), #{hfPrevBudget}.getValue());" />
                </Listeners>
            </ext:GridPanel>
          </Items>
          <BottomBar>
            <ext:Toolbar ID="Toolbar2" runat="server" Layout="FitLayout">
              <Items>
                <ext:FormPanel ID="FormPanel1" runat="server" AutoScroll="true" Height="30" Layout="FitLayout">
                  <Items>
                    <ext:Panel ID="Panel1" runat="server" Padding="5" AutoScroll="true" Layout="Column">
                      <Items>
                        <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" ColumnWidth=".1" Layout="Form">
                          <Items>
                          </Items>
                        </ext:Panel>
                        <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                          <Items>
                            <ext:Label ID="lbLimitDivPri" runat="server" Text="" FieldLabel="Limit Div Principal" />
                            <%--<ext:Label ID="lbLimitPrinc" runat="server" Text="" FieldLabel="Limit Principal" />
                            <ext:Label ID="lbLimitSisaPrinc" runat="server" Text="" FieldLabel="Limit Sisa Principal" />--%>
                          </Items>
                        </ext:Panel>
                        <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                          <Items>
                            <ext:Label ID="lbLimitAlokasi" runat="server" Text="" FieldLabel="Alokasi Limit" />
                          </Items>
                        </ext:Panel>
                        <%--<ext:Panel ID="Panel5" runat="server" Border="false" Header="false" ColumnWidth=".33" Layout="Form">
                          <Items>
                            <ext:Label ID="lbLimitUsed" runat="server" Text="" FieldLabel="Value" />
                          </Items>
                        </ext:Panel>--%>
                      </Items>
                    </ext:Panel>
                  </Items>
                </ext:FormPanel>
              </Items>
            </ext:Toolbar>
          </BottomBar>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <BottomBar>
      <ext:Toolbar ID="tb" runat="server">
          <Items>
            <ext:Checkbox runat="server" ID="chkSuggest" FieldLabel="Set Suggestion Limit" >
            <Listeners>
                <Check Handler="setDefaultLimit(this, #{gridDetail}.getStore());" />
            </Listeners>
            </ext:Checkbox>
          </Items>
      </ext:Toolbar>
  </BottomBar>
  <Buttons>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfDivPriNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="limitdiv" Value="#{lbLimitDivPri}.getText()" Mode="Raw" />
            <ext:Parameter Name="limitalokasi" Value="#{lbLimitAlokasi}.getText()" Mode="Raw" />
            <ext:Parameter Name="tahun" Value="#{hfTahun}.getValue()" Mode="Raw" />
            <ext:Parameter Name="bulan" Value="#{hfBulan}.getValue()" Mode="Raw" />        
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
<ext:Window ID="winDetailSOHDC1" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemDC1" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailSOHDC1" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store8" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingGoodBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50033" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemDC1}), '']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
              <Fields>
                <ext:RecordField Name="c_gdg" />
                <ext:RecordField Name="c_iteno" />
                <ext:RecordField Name="v_itnam" />
                <ext:RecordField Name="c_batch" />
                <ext:RecordField Name="n_gsisa" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="c_batch" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="c_gdg" Header="Gudang" />
          <ext:Column DataIndex="c_iteno" Header="Nomor Item" />
          <ext:Column DataIndex="v_itnam" Header="Nama Item" />
          <ext:Column DataIndex="c_batch" Header="Batch" />
          <ext:NumberColumn DataIndex="n_gsisa" Header="Jumlah" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <BottomBar>
        <ext:PagingToolbar ID="GmPagingGoodBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label5" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer5" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingGoodBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingGoodBB}.pageSize = parseInt(this.getValue()); #{gmPagingGoodBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>
<%--<ext:Window ID="winDetailSOHDC1" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemDC1" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailSOHDC1" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store2" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingGoodBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50033" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemDC1}), '']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
              <Fields>
                <ext:RecordField Name="c_gdg" />
                <ext:RecordField Name="c_iteno" />
                <ext:RecordField Name="v_itnam" />
                <ext:RecordField Name="c_batch" />
                <ext:RecordField Name="n_gsisa" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="c_batch" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="c_gdg" Header="Gudang" />
          <ext:Column DataIndex="c_iteno" Header="Nomor Item" />
          <ext:Column DataIndex="v_itnam" Header="Nama Item" />
          <ext:Column DataIndex="c_batch" Header="Batch" />
          <ext:NumberColumn DataIndex="n_gsisa" Header="Jumlah" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <BottomBar>
        <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label2" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
            <ext:ComboBox ID="ComboBox2" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="100" />
              <Listeners>
                <Select Handler="#{gmPagingGoodBB}.pageSize = parseInt(this.getValue()); #{gmPagingGoodBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>--%>
<ext:Window ID="winDetailSOHDC2" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemDC2" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailSOHDC2" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store1" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingGoodBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50033" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemDC2}), '']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
              <Fields>
                <ext:RecordField Name="c_gdg" />
                <ext:RecordField Name="c_iteno" />
                <ext:RecordField Name="v_itnam" />
                <ext:RecordField Name="c_batch" />
                <ext:RecordField Name="n_gsisa" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="c_batch" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="c_gdg" Header="Gudang" />
          <ext:Column DataIndex="c_iteno" Header="Nomor Item" />
          <ext:Column DataIndex="v_itnam" Header="Nama Item" />
          <ext:Column DataIndex="c_batch" Header="Batch" />
          <ext:NumberColumn DataIndex="n_gsisa" Header="Jumlah" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <BottomBar>
        <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label1" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
            <ext:ComboBox ID="ComboBox1" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingGoodBB}.pageSize = parseInt(this.getValue()); #{gmPagingGoodBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>
<ext:Window ID="winDetailSOHCab" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemCab" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailSOHCab" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store9" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBadBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50034" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemCab}), ''],
              ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success" >
              <Fields>
                <ext:RecordField Name="c_gdg" />
                <ext:RecordField Name="c_iteno" />
                <ext:RecordField Name="v_itnam" />
                <ext:RecordField Name="c_batch" />
                <ext:RecordField Name="n_bsisa" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="c_batch" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="c_gdg" Header="Gudang" />
          <ext:Column DataIndex="c_iteno" Header="Nomor Item" />
          <ext:Column DataIndex="v_itnam" Header="Nama Item" />
          <ext:Column DataIndex="c_batch" Header="Batch" />
          <ext:NumberColumn DataIndex="n_bsisa" Header="Jumlah" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <BottomBar>
        <ext:PagingToolbar ID="GmPagingBadBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label6" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer6" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingBadBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingBadBB}.pageSize = parseInt(this.getValue()); #{gmPagingBadBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>