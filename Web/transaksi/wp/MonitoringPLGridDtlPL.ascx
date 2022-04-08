<%--Created By Indra Monitoring Process 20180523FM--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MonitoringPLGridDtlPL.ascx.cs"
  Inherits="transaksi_wp_MonitoringPLGridDtlPL" %>

<ext:Window ID="winDetail" runat="server" Height="260" Width="620" Hidden="true"
  Maximizable="false" Layout="Fit" >
  <Content>
    <ext:Hidden ID="hfPLNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeNameCtrl" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <LoadMask ShowMask="true" />
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
                    <ext:Parameter Name="model" Value="0380-e" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_plno = @0', #{hfPLNo}.getValue(), 'System.String']]" Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_plno" />
                        <ext:RecordField Name="v_nama" />
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_batch" />                        
                        <ext:RecordField Name="n_qty" Type="Float" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:Column DataIndex="c_plno" Header="Nomor Dokumen" />
                  <ext:Column DataIndex="v_nama" Header="Principal/Tujuan" />
                  <ext:Column DataIndex="c_iteno" Header="Kode Barang" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />                  
                  <ext:NumberColumn DataIndex="n_qty" Header="Qty /(SP Pending)" Format="0.000,00/i" Align="Right" />
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
 
<ext:Window ID="wndDown" runat="server" Hidden="true" />