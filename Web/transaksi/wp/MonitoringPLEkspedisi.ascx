<%--Created By Indra Monitoring Process 20180523FM--%>

<script type="text/javascript" language="javascript">
    var commandGridFunction = function(rec, comName, wndDO, gridDO, hidWP, gridDtl) {
        var itm = rec.get('c_nodoc');
        var store = '';
        
        switch (comName) {
            case 'DetailWP':
                if ((!Ext.isEmpty(wndDO)) && (!Ext.isEmpty(gridDO))) {
                    hidWP.setValue(itm);
                    
                    wndDO.setTitle(String.format('Detail Manifest - {0}', rec.get('c_nodoc')));
                    wndDO.hide = false;
                    wndDO.show();

                    store = gridDO.getStore();
                    store.removeAll();
                    store.reload();

                    store = gridDtl.getStore();
                    store.removeAll();
                }
             break;            
        }
    }
    
</script>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MonitoringPLEkspedisi.ascx.cs"
  Inherits="transaksi_wp_MonitoringPLEkspedisi" %>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="false" MinHeight="480" MinWidth="725" Layout="Fit" >
  <Content>
    <ext:Hidden ID="hfEPNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeNameCtrl" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" MaxHeight="175" >
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Informasi Ekspedisi" Height="210" Layout="Fit" ButtonAlign="Center" >
          <Items>
            <ext:Panel ID="Panel1" runat="server" Padding="5" Layout="Column">
              <Items>
                <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:TextField ID="txGudang" runat="server" AllowBlank="false" FieldLabel="Gudang Asal" MaxLength="10" Width="140" ReadOnly="true" />
                    <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Tgl. Buat Manifest" LabelWidth="250" ReadOnly="true" Width="250">
                      <Items>
                        <ext:DateField ID="txTglEkspedisi" runat="server" AllowBlank="false" Format="dd-MM-yyyy" LabelWidth="250" EnableKeyEvents="true" Width="140" />
                      </Items>
                    </ext:CompositeField>
                    <ext:TextField ID="txNamaEkspedisi" runat="server" FieldLabel="Nama Ekspedisi" MaxLength="100" Width="140" ReadOnly="true" />
                    <ext:TextField ID="txCabang" runat="server" FieldLabel="Cabang Tujuan" MaxLength="100" Width="140" ReadOnly="true" />
                  </Items>
                </ext:Panel>
                <ext:Panel ID="Panel3" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:TextField ID="txNoResi" runat="server" FieldLabel="Nomor Resi" MaxLength="100" Width="140" ReadOnly="true" />
                    <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Tgl. Buat Resi" ReadOnly="true" Width="140">
                      <Items>
                        <ext:DateField ID="txTglResi" runat="server" AllowBlank="false" Format="dd-MM-yyyy" EnableKeyEvents="true" Width="140" />
                      </Items>
                    </ext:CompositeField>
                    <ext:TextField ID="txKoli" runat="server" FieldLabel="Jml. Koli(Koli)" MaxLength="100" Width="140" ReadOnly="true" StyleSpec="text-align:right;" />
                    <ext:TextField ID="txBerat" runat="server" FieldLabel="Jml. Berat(Kg)" MaxLength="100" Width="140" ReadOnly="true" StyleSpec="text-align:right;" />
                    <ext:TextField ID="txVolume" runat="server" FieldLabel="Jml. Volume(m3)" MaxLength="100" Width="140" ReadOnly="true" StyleSpec="text-align:right;" />
                    <ext:TextField ID="txReceh" runat="server" FieldLabel="Jml. Receh(pcs)" MaxLength="100" Width="140" ReadOnly="true" StyleSpec="text-align:right;" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>            
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Details Ekspedisi" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <LoadMask ShowMask="true" />
                <Listeners>
                  <Command Handler="commandGridFunction(record, command, #{winDetailWP}, #{gpDetailWP}, #{hidWP}, #{GridDetail2});" />
                </Listeners>
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0380-c" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_expno = @0', #{hfEPNo}.getValue(), 'System.String']]" Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_nodoc" />
                        <ext:RecordField Name="n_koli" Type="Float" />
                        <ext:RecordField Name="n_berat" Type="Float" />
                        <ext:RecordField Name="n_vol" Type="Float" />
                        <ext:RecordField Name="n_receh" Type="Float" />
                        <ext:RecordField Name="n_kolireceh" Type="Float" />                        
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:Column Width="30" DataIndex="" Resizable="false">
                      <Commands>
                        <ext:ImageCommand Icon="BookOpen" CommandName="DetailWP" ToolTip-Text="Rincian DO" ToolTip-Title="Command" />
                      </Commands>
                  </ext:Column>
                  <ext:Column DataIndex="c_nodoc" Header="Nomor Pallet" Width="120" />
                  <ext:NumberColumn DataIndex="n_koli" Header="Koli Utuh" Format="0.000,00/i" Width="100" Align="Right" />
                  <ext:NumberColumn DataIndex="n_kolireceh" Header="Koli Receh" Format="0.000,00/i" Width="100" Align="Right" />   
                  <ext:NumberColumn DataIndex="n_receh" Header="Qty. Receh" Format="0.000,00/i" Width="100" Align="Right" />
                  <ext:NumberColumn DataIndex="n_vol" Header="Volume(m3)" Format="0.000,00/i" Width="100" Align="Right" />
                  <ext:NumberColumn DataIndex="n_berat" Header="Berat(Kg)" Format="0.000,00/i" Width="100" Align="Right" />
                </Columns>
              </ColumnModel>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>

<ext:Window ID="winDetailWP" runat="server" Width="1200" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidWP" runat="server" />
  </Content>
  <Items>
    <ext:FormPanel ID="pnlGridDetailBox" runat="server" Layout="ColumnLayout">
        <Items>
            <ext:Panel ID="Panel4" runat="server" ColumnWidth="0.50" Layout="FitLayout">
                <Items>
                    <ext:GridPanel ID="gpDetailWP" runat="server">
                        <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server">
                                <DirectEvents>
                                    <RowSelect OnEvent="OnSelectGrid">
                                        <ExtraParams>
                                            <ext:Parameter Name="c_noTrans" Value="this.getSelected().data['c_no']" Mode="Raw" />
                                        </ExtraParams>
                                    </RowSelect>
                                </DirectEvents>
                            </ext:RowSelectionModel>
                        </SelectionModel>
                        <Store>
                            <ext:Store ID="strGridMain" runat="server" RemotePaging="false" RemoteSort="false">
                                <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                        CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                    <ext:Parameter Name="start" Value="0" />
                                    <ext:Parameter Name="limit" Value="-1" />
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="model" Value="0319" />
                                    <ext:Parameter Name="sort" Value="" />
                                    <ext:Parameter Name="dir" Value="" />
                                    <ext:Parameter Name="parameters" Value="[['c_nodoc = @0', paramValueGetter(#{hidWP}), ''],
                                                                            ['Tipe', '1', 'System.String']]"
                                        Mode="Raw" />                                                                               
                                </BaseParams>
                                <Reader>
                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                        <Fields>
                                            <ext:RecordField Name="c_no" />
                                            <ext:RecordField Name="n_karton" Type="Float" />
                                            <ext:RecordField Name="n_receh" Type="Float" />
                                            <ext:RecordField Name="n_hiddenkarton" Type="Float" />
                                            <ext:RecordField Name="n_hiddenreceh" Type="Float" />
                                            <ext:RecordField Name="c_type_editkoli" />
                                            <ext:RecordField Name="v_ket_editkoli" />
                                            <ext:RecordField Name="l_void" Type="Boolean" />
                                            <ext:RecordField Name="l_new" Type="Boolean" />
                                            <ext:RecordField Name="l_modified" Type="Boolean" />
                                            <ext:RecordField Name="l_modifiedkoli" Type="Boolean" />
                                            <ext:RecordField Name="c_sp" />
                                            <ext:RecordField Name="c_spno" />
                                            <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ctl341">
                            <Columns>
                                <ext:Column DataIndex="c_sp" Header="No. SP Cabang" Width="120" />
                                <ext:Column DataIndex="c_spno" Header="No. SP HO" Width="100" />                            
                                <ext:Column DataIndex="c_no" Header="No. DO" Width="100" />
                                <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="SP Received Time" Format ="dd/MM/yyyy H:i" Width="110"/>
                                <ext:NumberColumn DataIndex="n_karton" Header="Koli Utuh" Format="0.000,00/i" Width="75" />
                                <ext:NumberColumn DataIndex="n_receh" Header="Koli Receh" Format="0.000,00/i" Width="75" />                                                                                  
                            </Columns>
                        </ColumnModel>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
            <ext:Panel ID="Panel5" runat="server" ColumnWidth="0.50" Layout="FitLayout">
                <Items>
                    <ext:GridPanel ID="GridDetail2" runat="server">
                        <LoadMask ShowMask="true" />
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" ID="ctl344" />
                        </SelectionModel>
                        <Store>
                            <ext:Store runat="server" RemotePaging="false" RemoteSort="false" ID="Sx">
                                <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                        CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                    <ext:Parameter Name="start" Value="0" />
                                    <ext:Parameter Name="limit" Value="-1" />
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="sort" Value="" />
                                    <ext:Parameter Name="dir" Value="" />
                                </BaseParams>
                                <Reader>
                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                        <Fields>
                                            <ext:RecordField Name="c_iteno" />
                                            <ext:RecordField Name="v_itnam" />
                                            <ext:RecordField Name="v_cunam" />                                                                
                                            <ext:RecordField Name="c_spno" />
                                            <ext:RecordField Name="c_batch" />
                                            <ext:RecordField Name="n_qty" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel ID="ctl345">
                            <Columns>
                                <ext:CommandColumn Width="25">
                                </ext:CommandColumn>
                                <ext:Column DataIndex="c_iteno" Header="Kode Barang" Width="75" />
                                <ext:Column DataIndex="v_itnam" Header="Nama Barang" />
                                <ext:Column DataIndex="v_cunam" Header="Cabang Tujuan" />                                                                                                        
                                <ext:Column DataIndex="c_spno" Header="Nomor PL" />
                                <ext:Column DataIndex="c_batch" Header="Batch" />
                                <ext:Column DataIndex="n_qty" Header="Qty" Width="75" />
                            </Columns>
                        </ColumnModel>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
        </Items>
    </ext:FormPanel>
  </Items>
  </ext:Window>
 
<ext:Window ID="wndDown" runat="server" Hidden="true" />