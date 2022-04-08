<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Master.master" CodeFile="Transfer.aspx.cs"
  Inherits="transaksi_transfer_Transfer" %>

<%@ Register Src="TransferGudang.ascx" TagName="TransferGudang" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
  <uc1:TransferGudang ID="TransferGudang" runat="server" />
</asp:Content>
