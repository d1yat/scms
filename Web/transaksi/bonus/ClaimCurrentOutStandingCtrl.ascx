<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ClaimCurrentOutStandingCtrl.ascx.cs" Inherits="transaksi_bonus_ClaimCurrentOutStandingCtrl" %>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfClaimAccNo" runat="server" />
    <ext:Hidden ID="hfItem" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center>
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store4" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0160" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_iteno = @0', #{hfItem}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_claimaccno" />
                        <ext:RecordField Name="d_claimaccdate" Type="Date" DateFormat="M$"/>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_claimaccno" />
                        <ext:RecordField Name="n_qtyacc" Type="Float" />
                        <ext:RecordField Name="n_qtytolak" Type="Float" /> 
                        <ext:RecordField Name="n_sisa" Type="Float" /> 
                       </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_claimaccno" Header="No Claim Acc" Width="150" />
                  <ext:DateColumn ColumnID="d_claimaccdate" DataIndex="d_claimaccdate" Header="Tanggal" Format="dd-MM-yyyy" />
                  <ext:NumberColumn DataIndex="n_sisa" Header="Qty Sisa" Format="0.000,00/i" Width="75" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>
            </ext:GridPanel>
          </Items>
          <Buttons>
            <ext:Button ID="Button1" runat="server" Icon="Printer" Text="Print">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click">
                  <Confirmation ConfirmRequest="true" Title="Print ?" Message="Anda yakin ingin mecetak data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfItem}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
   </Items>
  </ext:Window>
  <ext:Window ID="wndDown" runat="server" Hidden="true" />
