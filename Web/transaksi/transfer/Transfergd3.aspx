<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master.master" CodeFile="Transfergd3.aspx.cs"
  Inherits="transaksi_transfer_Transfer" %>

<%@ Register Src="TransferGudanggd3.ascx" TagName="TransferGudang" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
  <uc1:TransferGudang ID="TransferGudanggd3" runat="server" />
</asp:Content>
