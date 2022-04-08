<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Legend.ascx.cs"
  Inherits="transaksi_penjualan_Legend" %>

<ext:Window ID="winDetail" runat="server" Height="220" Width="600" Hidden="true"
  Maximizable="false" MinHeight="220" MinWidth="600" Layout="Fit">
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="" Height="150" Layout="FormLayout">
          <Items>
            <ext:Label ID="Label1" runat ="server" Text="Direct Shipment" StyleSpec="background: #a3c2c2; font-size : x-large;"></ext:Label>
            <ext:Label ID="LbWarna1" runat ="server" Text="Pelayanan full" StyleSpec="background: #AAF07B; font-size : x-large;"></ext:Label>
            <ext:Label ID="LbWarna2" runat ="server" Text="Pelayanan sebagian" StyleSpec="background: #FFCC66; font-size : x-large;"></ext:Label>
            <ext:Label ID="LbWarna3" runat ="server" Text="Waktu pelayanan lebih kecil dari hari ini" StyleSpec="background: #F9A2A4; font-size : x-large;"></ext:Label>
            <ext:Label ID="LbPengertian1" runat ="server" Text="P&P = Packed and Palletized" StyleSpec="font-size : x-large;"></ext:Label>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
