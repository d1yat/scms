<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LimitPOPrincipal.aspx.cs"
  Inherits="transaksi_pembelian_LimitPOPrincipal" MasterPageFile="~/Master.master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

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
        besls = e.record.get('n_besls'),
        percentadj = e.record.get('n_percentadj'),
        stdStock = e.record.get('n_qty'),
        soh = e.record.get('n_soh'),
        sit = e.record.get('n_sit'),        
        dif = 0,
        bestslsadj = 0,
        qtyeom = 0,
        estpurchase = 0,
        newavailablebudget = 0;

        bestslsadj = besls * percentadj;
        qtyeom = bestslsadj * stdStock;

        estpurchase = bestslsadj + qtyeom - soh - sit;
        
        dif = newVal - prevVal;
        newavailablebudget = availablebudget + dif;

        e.record.set('n_bestslsadj', bestslsadj);
        e.record.set('n_qtyeom', qtyeom);
        e.record.set('n_estpurchase', estpurchase);        
        e.record.set('n_availablebudget', newavailablebudget);
        e.record.set('l_modified', true);
    }

    var recalc = function(store) {


        var balance = 0,
        pomtd = 0,
        budget = 0;

        //calc total
        var sohtotal = 0,
        sitTotal = 0,
        lastMonthTotal = 0,
        avgLastmonthTotal = 0,
        salesMTDTotal = 0,
        idxLastMonthTotal = 0,
        idxAvgLastMonthTotal = 0,
        salesContrTotal = 0,
        bestEstTotal = 0,
        stockProjectionTotal = 0,
        PurcEstTotal = 0,
        PendPOLastMonthTotal = 0,
        PendPOMTDTotal = 0,
        forecastTotal = 0,
        budgetTotal = 0,
        balanceTotal = 0;


        store.each(function(r) {
            if (r.get('l_total') == false) {
                budget = r.get('n_budget');
                pomtd = r.get('n_pomtdqtyval');

                //                balance = budget - pomtd;

                //                r.set('n_availablebudget', balance);

                //calc total
                sohtotal += r.get('n_soh');
                sitTotal += r.get('n_sit');
                lastMonthTotal += r.get('n_sls');
                avgLastmonthTotal += r.get('n_avgsls');
                salesMTDTotal += r.get('n_slsmtdval');
//                idxLastMonthTotal += r.get('n_idxlastmonth');
//                idxAvgLastMonthTotal += r.get('n_idx3lastmonth');
                salesContrTotal += r.get('n_percentage');
                bestEstTotal += r.get('n_besls');
                stockProjectionTotal += r.get('n_qtyeom');
                PurcEstTotal += r.get('n_estpurchase');
                PendPOLastMonthTotal += r.get('n_posisaqtyval');
                PendPOMTDTotal += r.get('n_pomtdqtyval');
                forecastTotal += r.get('n_forecast');
                budgetTotal += r.get('n_budget');
                balanceTotal += r.get('n_availablebudget');
            }
        });

        //insert total
        var rec = '',
        idxTotal = 0;

        idxTotal = store.findExact('l_total', true);
        rec = store.getAt(idxTotal);
        idxLastMonthTotal = (sohtotal + sitTotal) / lastMonthTotal;
        idxAvgLastMonthTotal = (sohtotal + sitTotal) / avgLastmonthTotal;
        
        if (idxTotal != -1) {
            rec.set('n_soh', sohtotal);
            rec.set('n_sit', sitTotal);
            rec.set('n_sls', lastMonthTotal);
            rec.set('n_avgsls', avgLastmonthTotal);
            rec.set('n_slsmtdval', salesMTDTotal);
            rec.set('n_idxlastmonth', idxLastMonthTotal);
            rec.set('n_idx3lastmonth', idxAvgLastMonthTotal);
            rec.set('n_percentage', salesContrTotal);
            rec.set('n_besls', bestEstTotal);
            rec.set('n_qtyeom', stockProjectionTotal);
            rec.set('n_estpurchase', PurcEstTotal);
            rec.set('n_posisaqtyval', PendPOLastMonthTotal);
            rec.set('n_pomtdqtyval', PendPOMTDTotal);
            rec.set('n_forecast', forecastTotal);
            rec.set('n_budget', budgetTotal);
            rec.set('n_availablebudget', balanceTotal);
        }
        else {

            var gridcount = store.getCount();

            store.insert(gridcount, new Ext.data.Record({
                'n_soh': sohtotal,
                'n_sit': sitTotal,
                'n_sls': lastMonthTotal,
                'n_avgsls': avgLastmonthTotal,
                'n_slsmtdval': salesMTDTotal,
                'n_idxlastmonth': idxLastMonthTotal,
                'n_idx3lastmonth': idxAvgLastMonthTotal,
                'n_percentage': salesContrTotal,
                'n_besls': bestEstTotal,
                'n_qtyeom': stockProjectionTotal,
                'n_estpurchase': PurcEstTotal,
                'n_posisaqtyval': PendPOLastMonthTotal,
                'n_pomtdqtyval': PendPOMTDTotal,
                'n_forecast': forecastTotal,
                'n_budget': budgetTotal,
                'n_availablebudget': balanceTotal,
                'l_total': true
            }));
        }


        //        if (!Ext.isEmpty(totalLimit)) {
        //            lblimitalokasi.setText(myFormatNumber(totalLimit));
        //        }
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

    var getRowClass = function(record) {
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
    <ext:Hidden ID="hfPrevBudget" runat="server" />
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
              </Items>
            </ext:FormPanel>
          </North>
          <Center>
            <ext:Panel ID="Panel2" runat="server" Layout="FitLayout">
              <Items>
          <ext:GridPanel ID="gridMain" runat="server">
            <LoadMask ShowMask="true" />
            <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
                <ext:Store ID="storeGridItem" runat="server" RemotePaging="false" RemoteSort="false">
                    <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                    </Proxy>
                    <BaseParams>
                        <ext:Parameter Name="start" Value="0" />
                        <ext:Parameter Name="limit" Value="-1" />
                        <ext:Parameter Name="model" Value="05011" />
                        <ext:Parameter Name="sort" Value="" />
                        <ext:Parameter Name="dir" Value="" />
                        <ext:Parameter Name="parameters" Value="[
                            ['@contains.c_nosup.Contains(@0)', paramRawValueGetter(#{txNosupFltr}), ''],
                            ['tahun', #{cbPeriode1}.getValue(), 'System.Decimal'],
                            ['bulan', #{cbPeriode2}.getValue(), 'System.Decimal'],
                            ['@contains.v_nama.Contains(@0)', paramRawValueGetter(#{txSupNamFltr}), '']]"
                            Mode="Raw" />
                    </BaseParams>
                    <Reader>
                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                            <Fields>
                                <ext:RecordField Name="c_nosup" />
                                <ext:RecordField Name="n_tahun" />
                                <ext:RecordField Name="n_bulan" />
                                <ext:RecordField Name="v_nama" />
                                <ext:RecordField Name="n_soh" Type="Float" />
                                <ext:RecordField Name="n_sit" Type="Float" />
                                <ext:RecordField Name="n_sls" Type="Float" />
                                <ext:RecordField Name="n_avgsls" Type="Float" />
                                <ext:RecordField Name="n_slsmtdval" Type="Float" />
                                <ext:RecordField Name="n_idxlastmonth" Type="Float" />
                                <ext:RecordField Name="n_idx3lastmonth" Type="Float" />
                                <ext:RecordField Name="n_percentage" Type="Float" />
                                <ext:RecordField Name="n_besls" Type="Float" />
                                <ext:RecordField Name="n_percentadj" Type="Float" />
                                <ext:RecordField Name="n_bestslsadj" Type="Float" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                                <ext:RecordField Name="n_qtyeom" Type="Float" />
                                <ext:RecordField Name="n_estpurchase" Type="Float" />
                                <ext:RecordField Name="n_posisaqtyval" Type="Float" />
                                <ext:RecordField Name="n_pomtdqtyval" Type="Float" />
                                <ext:RecordField Name="n_forecast" Type="Float" />
                                <ext:RecordField Name="n_budget" Type="Float" />
                                <ext:RecordField Name="n_availablebudget" Type="Float" />
                                <ext:RecordField Name="l_modified" Type="Boolean" />      
                                <ext:RecordField Name="l_total" Type="Boolean" />                   
                            </Fields>
                        </ext:JsonReader>
                    </Reader>
                    <Listeners>
                        <Load Handler="recalc(#{gridMain}.getStore());" />
                    </Listeners>
                  <SortInfo Field="v_nama" Direction="ASC" />
                </ext:Store>
            </Store>
            <ColumnModel>
                <Columns>
                    <ext:Column ColumnID="c_nosup" DataIndex="c_nosup" Header="Code" Width="50" />
                    <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Principal" Width="200" />
                    <ext:NumberColumn ColumnID="n_soh" DataIndex="n_soh" Header="Stok Awal (SOH)" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_sit" DataIndex="n_sit" Header="SIT" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_sls" DataIndex="n_sls" Header="Sales Last Month" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_avgsls" DataIndex="n_avgsls" Header="Avg Sales Last 3 Month" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_slsmtdval" DataIndex="n_slsmtdval" Header="Sales MTD" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_idxlastmonth" DataIndex="n_idxlastmonth" Header="Index Last Month" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_idx3lastmonth" DataIndex="n_idx3lastmonth" Header="Index Avg Last 3 Month" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_percentage" DataIndex="n_percentage" Header="Sales Contribution(%)" Format="0.000,0000/i" />
                    <ext:NumberColumn ColumnID="n_besls" DataIndex="n_besls" Header="Best Estimate Sales" Format="0.000,00/i" >
                        <Editor>
                            <ext:NumberField ID="NumberField1" runat="server" AllowDecimals="true" AllowNegative="false" MinValue="0"
                            DecimalPrecision="2" />
                        </Editor>
                    </ext:NumberColumn>
                    <ext:NumberColumn ColumnID="n_percentadj" DataIndex="n_percentadj" Header="% ADJ" Format="0.000,0000/i" >
                        <Editor>
                            <ext:NumberField ID="NumberField2" runat="server" AllowDecimals="true" AllowNegative="false" MaxValue="100" MinValue="0"
                            DecimalPrecision="2" />
                        </Editor>
                    </ext:NumberColumn>
                    <ext:NumberColumn ColumnID="n_bestslsadj" DataIndex="n_bestslsadj" Header="Best Estimate Sales After Adj" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_qty" DataIndex="n_qty" Header="Std Stock" Format="0.000,00/i" >
                        <Editor>
                            <ext:NumberField ID="NumberField4" runat="server" AllowDecimals="true" AllowNegative="false" MinValue="0"
                            DecimalPrecision="2" />
                        </Editor>
                    </ext:NumberColumn>
                    <ext:NumberColumn ColumnID="n_qtyeom" DataIndex="n_qtyeom" Header="Stock Projection End of Month" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_estpurchase" DataIndex="n_estpurchase" Header="Purchase Estimate" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_posisaqtyval" DataIndex="n_posisaqtyval" Header="Pending PO Last Month" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_pomtdqtyval" DataIndex="n_pomtdqtyval" Header="Pending PO MTD" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_forecast" DataIndex="n_forecast" Header="Forecast" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_budget" DataIndex="n_budget" Header="Limit PO" Format="0.000,00/i" >
                        <Editor>
                            <ext:NumberField ID="NumberField5" runat="server" AllowDecimals="true" AllowNegative="false" MinValue="0"
                            DecimalPrecision="2" />
                        </Editor>
                    </ext:NumberColumn>
                    <ext:NumberColumn ColumnID="n_availablebudget" DataIndex="n_availablebudget" Header="Sisa Limit" Format="0.000,00/i" />    
                    <ext:CheckColumn ColumnID="l_modified"  DataIndex="l_modified" Header="Modify" Width="50" />
                </Columns>
                    </ColumnModel>
                    <View>
                        <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                            <HeaderRows>
                                <ext:HeaderRow>
                                    <Columns>
                                      <ext:HeaderColumn>
                                        <Component>
                                          <ext:TextField ID="txNosupFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                            <Listeners>
                                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="1000" />
                                            </Listeners>
                                          </ext:TextField>
                                        </Component>
                                      </ext:HeaderColumn>
                                      <ext:HeaderColumn>
                                        <Component>
                                          <ext:TextField ID="txSupNamFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                            <Listeners>
                                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="1000" />
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
                <AfterEdit Handler="afterEditDataConfirm(e, #{gridMain}.getStore(), #{hfPrevBudget}.getValue());" />
            </Listeners>
            <Buttons>
            <ext:Button ID="btnSimpan" runat="server" Icon="Disk" Text="Simpan">
            <DirectEvents>
              <Click OnEvent="SaveBtn_Click">
                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                <ExtraParams>
                  <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())"
                    Mode="Raw" />
                </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>
          </Buttons>
        </ext:GridPanel>
              </Items>
            </ext:Panel>
          </Center>
        </ext:BorderLayout>
      </Items>
    </ext:Panel>
  </Items>
</ext:Viewport>
</asp:Content>