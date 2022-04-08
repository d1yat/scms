<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HistorySP.ascx.cs" Inherits="transaksi_pembelian_HistorySP" %>

<ext:Window ID="winDetail" runat="server" Height="200" Width="700" Hidden="true" Resizable="false"
  Maximizable="true" MinHeight="200" MinWidth="700" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="txtspno" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="200">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="History Perubahan Leadtime" Height="200" Layout="Fit"  >
          <Items>
            <ext:GridPanel ID="gridDetailHistory" runat="server" AutoScroll="true">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false" >
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0157" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" /> 
                    <ext:Parameter Name="parameters" Value="[['spno', #{txtspno}.getValue(), 'System.String']]" Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_spno" />
                        <ext:RecordField Name="c_sp" />
                        <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="d_before" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="d_after" Type="Date" DateFormat="M$" />   
                        <ext:RecordField Name="v_nama" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>               
                  <ext:Column DataIndex="c_spno" Header="Nomor SP" Width="90" />
                  <ext:Column DataIndex="c_sp" Header="Nomor SP Cabang" width="150" />
                  <ext:DateColumn DataIndex="d_entry" Header="Tanggal Input" Format="dd-MM-yyyy" Width="87" />
                  <ext:DateColumn DataIndex="d_before" Header="Sebelum Perubahan" Format="dd-MM-yyyy" Width="87" />
                  <ext:DateColumn DataIndex="d_after" Header="Setelah Perubahan" Format="dd-MM-yyyy" Width="87" />
                  <ext:Column DataIndex="v_nama" Header="Yang Merubah" width="150" />
                </Columns>
              </ColumnModel>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />