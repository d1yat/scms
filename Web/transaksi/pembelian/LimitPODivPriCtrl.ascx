<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LimitPODivPriCtrl.ascx.cs"
  Inherits="transaksi_pembelian_LimitPODivPriCtrl" %>

<script type="text/javascript">
    var afterEditDataConfirm = function(e, store) {
        recalc(store);

        e.record.set('l_modified', true);
    }

    var recalc = function(store) {
        var lblimitalokasi = Ext.getCmp('<%= lbLimitAlokasi.ClientID %>');
        var lblimitpri = Ext.getCmp('<%= lbLimitPri.ClientID %>');

        var limit = 0,
        totalLimit = 0,
        pomtd = 0,
        balance = 0,
        limitdivpri = parseFloat(lblimitpri.getText().replace(".", "").replace(".", "").replace(".", "").replace(".", "").replace(",", "."));


        var avgslsval = 0,
        slsval = 0,
        percentage = 0,
        budget = 0,
        forecast = 0,
        posisaval = 0,
        pomtdval = 0,
        balancetotal = 0;

        store.each(function(r) {
            if (r.get('l_total') == false) {
                limit = r.get('n_budget');
                pomtd = r.get('n_pomtdval');

                balance = limit - pomtd;
                r.set('n_balance', balance);

                totalLimit += limit;

                //calc total
                avgslsval += r.get('n_avgslsval');
                slsval += r.get('n_slsval');
                percentage += r.get('n_percentage');
                budget += r.get('n_budget');
                forecast += r.get('n_forecast');
                posisaval += r.get('n_posisaval');
                pomtdval += r.get('n_pomtdval');
                balancetotal += r.get('n_balance');
            }
        });

        if (!Ext.isEmpty(totalLimit)) {
            lblimitalokasi.setText(myFormatNumber(totalLimit));
        }

        //insert total
        var rec = '';

        idxTotal = store.findExact('l_total', true);
        rec = store.getAt(idxTotal);

        if (idxTotal != -1) {
            rec.set('n_avgslsval', avgslsval);
            rec.set('n_slsval', slsval);
            rec.set('n_percentage', percentage);
            rec.set('n_budget', budget);
            rec.set('n_forecast', forecast);
            rec.set('n_posisaval', posisaval);
            rec.set('n_pomtdval', pomtdval);
            rec.set('n_balance', balancetotal);
        }
        else {
            var gridcount = store.getCount();

            store.insert(gridcount, new Ext.data.Record({
                'n_avgslsval': avgslsval,
                'n_slsval': slsval,
                'n_percentage': percentage,
                'n_budget': budget,
                'n_forecast': forecast,
                'n_posisaval': posisaval,
                'n_pomtdval': pomtdval,
                'n_balance': balancetotal,
                'l_total': true
            }));
        }
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

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfNosup" runat="server" />
    <ext:Hidden ID="hfTahun" runat="server" />
    <ext:Hidden ID="hfBulan" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeNameCtrl" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit" AutoScroll="true">
                <LoadMask ShowMask="true" />
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
                            <ext:Parameter Name="model" Value="05009" />
                            <ext:Parameter Name="sort" Value="" />
                            <ext:Parameter Name="dir" Value="" />
                            <ext:Parameter Name="parameters" Value="[
                            ['nosup', #{hfNosup}.getValue(), 'System.String'],
                            ['tahun', #{hfTahun}.getValue(), 'System.Decimal'],
                            ['bulan', #{hfBulan}.getValue(), 'System.Decimal']]"
                                Mode="Raw" />
                        </BaseParams>
                        <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                <Fields>
                                    <ext:RecordField Name="n_tahun" Type="Float" />
                                    <ext:RecordField Name="n_bulan" Type="Float" />
                                    <ext:RecordField Name="c_kddivpri" />
                                    <ext:RecordField Name="v_nmdivpri" />
                                    <ext:RecordField Name="c_nosup" />
                                    <ext:RecordField Name="n_avgslsval" Type="Float" />
                                    <ext:RecordField Name="n_slsval" Type="Float" />
                                    <ext:RecordField Name="n_sohtotal" Type="Float" />
                                    <ext:RecordField Name="n_percentage" Type="Float" />
                                    <ext:RecordField Name="n_budget" Type="Float" />
                                    <ext:RecordField Name="n_forecast" Type="Float" />
                                    <ext:RecordField Name="n_posisaval" Type="Float" />
                                    <ext:RecordField Name="n_pomtdval" Type="Float" />
                                    <ext:RecordField Name="n_balance" Type="Float" />
                                    <ext:RecordField Name="l_modified" Type="Boolean" />      
                                    <ext:RecordField Name="l_total" Type="Boolean" />                       
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <Listeners>
                            <Load Handler="recalc(#{gridDetail}.getStore());" />
                        </Listeners>
                        <SortInfo Field="v_nmdivpri" Direction="ASC" />
                    </ext:Store>
                </Store>
                <ColumnModel>
                    <Columns>
                        <ext:Column ColumnID="c_kddivpri" DataIndex="c_kddivpri" Header="Code" Width="50" />
                        <ext:Column ColumnID="v_nmdivpri" DataIndex="v_nmdivpri" Header="Principal Division" Width="200" />
                        <ext:NumberColumn ColumnID="n_avgslsval" DataIndex="n_avgslsval" Header="Avg Sales" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_slsval" DataIndex="n_slsval" Header="Sales MTD" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_percentage" DataIndex="n_percentage" Header="Sales Contribution %" Format="0.000,0000/i" />
                        <ext:NumberColumn ColumnID="n_sohtotal" DataIndex="n_sohtotal" Header="SOH Total Value" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_forecast" DataIndex="n_forecast" Header="Limit Suggestion" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_budget" DataIndex="n_budget" Header="Limit" Format="0.000,00/i">
                            <Editor>
                              <ext:NumberField ID="NumberField1" runat="server" AllowDecimals="true" AllowNegative="false" MinValue="0"
                                DecimalPrecision="2" />
                            </Editor>
                        </ext:NumberColumn>
                        <ext:NumberColumn ColumnID="n_posisaval" DataIndex="n_posisaval" Header="PO Pending" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_pomtdval" DataIndex="n_pomtdval" Header="PO MTD" Format="0.000,00/i" />
                        <ext:NumberColumn ColumnID="n_balance" DataIndex="n_balance" Header="Balance" Format="0.000,00/i" />     
                        <ext:CheckColumn ColumnID="l_modified"  DataIndex="l_modified" Header="Modify" Width="50" />
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                        <HeaderRows>
                            <ext:HeaderRow>
                                <Columns>                                       
                                </Columns>
                            </ext:HeaderRow>
                        </HeaderRows>
                        <GetRowClass Fn="getRowClass" /> 
                    </ext:GridView>
                </View>
                <Listeners>
                    <AfterEdit Handler="afterEditDataConfirm(e, #{gridDetail}.getStore());" />
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
                            <ext:Label ID="lbLimitPri" runat="server" Text="" FieldLabel="Limit Principal" />
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
  <Buttons>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfNosup}.getValue()" Mode="Raw" />
            <ext:Parameter Name="limit" Value="#{lbLimitPri}.getText()" Mode="Raw" />
            <ext:Parameter Name="limitalokasi" Value="#{lbLimitAlokasi}.getText()" Mode="Raw" />
            <ext:Parameter Name="tahun" Value="#{hfTahun}.getValue()" Mode="Raw" />
            <ext:Parameter Name="bulan" Value="#{hfBulan}.getValue()" Mode="Raw" />  
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button2" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />

