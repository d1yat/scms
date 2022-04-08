<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" 
CodeFile="Default.aspx.cs" Inherits="reporting_Inventory_Default" %>

<%@ Register Src="PurchaseOrder.ascx" TagName="PurchaseOrder" TagPrefix="uc" %>
<%@ Register Src="ReceiveNote.ascx" TagName="ReceiveNote" TagPrefix="uc" %>
<%@ Register Src="ReturCustomer.ascx" TagName="ReturCustomer" TagPrefix="uc" %>
<%@ Register Src="ReturSupplier.ascx" TagName="ReturSupplier" TagPrefix="uc" %>
<%@ Register Src="PackingList.ascx" TagName="PackingList" TagPrefix="uc" %>
<%@ Register Src="TransferGudang.ascx" TagName="TransferGudang" TagPrefix="uc" %>
<%@ Register Src="POKhusuSP.ascx" TagName="POKhusuSP" TagPrefix="uc" %>
<%@ Register Src="SPPackingList.ascx" TagName="SPPackingList" TagPrefix="uc" %>
<%@ Register Src="Penjualan.ascx" TagName="Penjualan" TagPrefix="uc" %>
<%@ Register Src="ReturnDO.ascx" TagName="ReturnDO" TagPrefix="uc" %>
<%@ Register Src="SPExpedisi.ascx" TagName="SPExpedisi" TagPrefix="uc" %>
<%@ Register Src="AdjustStock.ascx" TagName="AdjustStock" TagPrefix="uc" %>
<%@ Register Src="AdjustSuratPesanan.ascx" TagName="AdjustSuratPesanan" TagPrefix="uc" %>
<%@ Register Src="AdjustPurchaseOrder.ascx" TagName="AdjustPurchaseOrder" TagPrefix="uc" %>
<%@ Register Src="AdjustSTT.ascx" TagName="AdjustSTT" TagPrefix="uc" %>
<%@ Register Src="AdjustCombo.ascx" TagName="AdjustCombo" TagPrefix="uc" %>
<%@ Register Src="AdjustFB.ascx" TagName="AdjustFB" TagPrefix="uc" %>
<%@ Register Src="AdjustFJ.ascx" TagName="AdjustFJ" TagPrefix="uc" %>
<%@ Register Src="Floating.ascx" TagName="Floating" TagPrefix="uc" %>
<%@ Register Src="LaporanPBF.ascx" TagName="LaporanPBF" TagPrefix="uc" %>
<%@ Register Src="OktPrekursorBulanan.ascx" TagName="OktPrekursorBulanan" TagPrefix="uc" %>
<%@ Register Src="SuratTandaTerima.ascx" TagName="STT" TagPrefix="uc" %>
<%@ Register Src="Combo.ascx" TagName="Combo" TagPrefix="uc" %>
<%@ Register Src="ListSP.ascx" TagName="ListSP" TagPrefix="uc" %>
<%@ Register Src="Pembelian.ascx" TagName="Pembelian" TagPrefix="uc" %>
<%@ Register Src="Enapza.ascx" TagName="Enapza" TagPrefix="uc" %>
<%@ Register Src="EreportAlkes.ascx" TagName="Ealkes" TagPrefix="uc" %>
<%@ Register Src="CustomerServiceLevel.ascx" TagName="CSL" TagPrefix="uc" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" Runat="Server">
  <link href="../../styles/main.css" type="text/css" rel="Stylesheet" />
  <script type="text/javascript">
  var initializePanel = function(f, c, t1, t2) {
    if (!Ext.isEmpty(f)) {
      f.getForm().reset();
    }

    if (!Ext.isEmpty(c)) {
      c.setValue(true);
    }

    var tgl = new Date();

    if (!Ext.isEmpty(t1)) {
      t1.setValue(tgl.getFirstDateOfMonth());
    }

    if (!Ext.isEmpty(t2)) {
      t2.setValue(tgl);
    }
  }

  var isValidForm = function(f) {
    if (Ext.isEmpty(f)) {
      return false;
    }

    if (!f.getForm().isValid()) {
      ShowWarning('Terdapat kesalahan dalam kriteria data.');

      return false;
    }
  }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" Runat="Server">
  <ext:Window ID="wndDown" runat="server" Hidden="true" />

  <uc:PurchaseOrder ID="PurchaseOrder" runat="server" Visible="false" /> 
  <uc:ReceiveNote ID="ReceiveNote" runat="server" Visible="false" />
  <uc:ReturCustomer ID="ReturCustomer" runat="server" Visible="false" />
  <uc:PackingList ID="PackingList" runat="server" Visible="false" />
  <uc:ReturSupplier ID="ReturSupplier" runat="server" Visible="false" />
  <uc:TransferGudang ID="TransferGudang" runat="server" Visible="false" />
  <uc:POKhusuSP ID="POKhusuSP" runat="server" Visible="false" />
  <uc:SPPackingList ID="SPPackingList" runat="server" Visible="false" />
  <uc:Penjualan ID="Penjualan" runat="server" Visible="false" />
  <uc:ReturnDO ID="ReturnDO" runat="server" Visible="false" />
  <uc:SPExpedisi ID="SPExpedisi" runat="server" Visible="false" />
  <uc:AdjustStock ID="AdjustStock" runat="server" Visible="false" />
  <uc:AdjustSuratPesanan ID="AdjustSuratPesanan" runat="server" Visible="false" />
  <uc:AdjustPurchaseOrder ID="AdjustPurchaseOrder" runat="server" Visible="false" />
  <uc:AdjustSTT ID="AdjustSTT" runat="server" Visible="false" />
  <uc:AdjustCombo ID="AdjustCombo" runat="server" Visible="false" />
  <uc:AdjustFB ID="AdjustFB" runat="server" Visible="false" />
  <uc:AdjustFJ ID="AdjustFJ" runat="server" Visible="false" />
  <uc:Floating ID="Floating" runat="server" Visible="false" />
  <uc:LaporanPBF ID="LaporanPBF" runat="server" Visible="false" />
  <uc:OktPrekursorBulanan ID="OktPrekursorBulanan" runat="server" Visible="false" />
  <uc:STT ID="STT" runat="server" Visible="false" />
  <uc:Combo ID="Combo" runat="server" Visible="false" />
  <uc:ListSP ID="ListSP" runat="server" Visible="false" />
  <uc:Pembelian ID="Pembelian" runat="server" Visible="false" />
  <uc:Enapza ID="Enapza" runat="server" Visible="false" />
  <uc:Ealkes ID="Ealkes" runat="server" Visible="false" />
  <uc:CSL ID="CSL" runat="server" Visible="false" />
</asp:Content>

