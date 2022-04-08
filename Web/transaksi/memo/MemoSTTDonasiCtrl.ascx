<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemoSTTDonasiCtrl.ascx.cs" 
Inherits="transaksi_memo_MemoSTTDonasiCtrl" %>

<ext:Window ID="winDetail" runat="server" Height="480" Width="1000" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfMemo" runat="server" />
    <ext:Hidden ID="hfMemoType" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
</ext:Window>
