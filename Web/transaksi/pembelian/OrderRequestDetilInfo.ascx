<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderRequestDetilInfo.ascx.cs" 
Inherits="transaksi_pembelian_OrderRequestDetilInfo" %>

<ext:Window ID="winDetail" runat="server" Height="300" Width="500" Hidden="true"
  Maximizable="true" MinHeight="300" MinWidth="500" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGdg" runat="server" />
    <ext:Hidden ID="hfItem" runat="server" />
  </Content>
 <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="250">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store runat="server" RemoteSort="true" SkinID="OriginalExtStore">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="model" Value="8096" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="ASC" />
                    <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                    ['Item', #{hfItem}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_cunam" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="NoTrans" />
                        <ext:RecordField Name="n_pending" Type="Float" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <SortInfo Field="NoTrans" Direction="DESC" />
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_cunam" Header="Nama Cabang" Width="150" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="150" />
                  <ext:Column DataIndex="NoTrans" Header="No Transaksi" Width="100" />
                  <ext:NumberColumn DataIndex="n_pending" Header="Jumlah" Format="0.000,00/i" Width="55" />
                </Columns>
              </ColumnModel>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
 </Items>
</ext:Window>
