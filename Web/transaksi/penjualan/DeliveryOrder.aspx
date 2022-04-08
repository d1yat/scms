<%@ Page Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="DeliveryOrder.aspx.cs"
  Inherits="transaksi_penjualan_DeliveryOrder" %>

<%@ Register Src="DO_PL.ascx" TagName="DO_PL" TagPrefix="uc" %>
<%@ Register Src="DO_STT.ascx" TagName="DO_STT" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfMode" runat="server" />
  <uc:DO_PL ID="DOPL" runat="server" />
  <uc:DO_STT ID="DOSTT" runat="server" />
</asp:Content>