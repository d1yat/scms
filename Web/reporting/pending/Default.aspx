<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default.aspx.cs" Inherits="reporting_pending_Default" %>

<%@ Register Src="SuratPesanan.ascx" TagName="SuratPesanan" TagPrefix="uc" %>
<%@ Register Src="SuratPesananGudang.ascx" TagName="SuratPesananGudang" TagPrefix="uc" %>
<%@ Register Src="SuratPesananPO.ascx" TagName="SuratPesananPO" TagPrefix="uc" %>
<%@ Register Src="PurchaseOrderLogistik.ascx" TagName="PurchaseOrderLogistik" TagPrefix="uc" %>
<%@ Register Src="PurchaseOrderFinance.ascx" TagName="PurchaseOrderFinance" TagPrefix="uc" %>
<%@ Register Src="DeliveryOrder.ascx" TagName="DeliveryOrder" TagPrefix="uc" %>
<%@ Register Src="ReturCustomer.ascx" TagName="ReturCustomer" TagPrefix="uc" %>
<%@ Register Src="ReceiveNote.ascx" TagName="ReceiveNote" TagPrefix="uc" %>
<%@ Register Src="ReturSuplier.ascx" TagName="ReturSuplier" TagPrefix="uc" %>
<%@ Register Src="SuratTandaTerima.ascx" TagName="SuratTandaTerima" TagPrefix="uc" %>
<%@ Register Src="PackingList.ascx" TagName="PackingList" TagPrefix="uc" %>
<%@ Register Src="Combo.ascx" TagName="Combo" TagPrefix="uc" %>
<%@ Register Src="SuratJalan.ascx" TagName="SuratJalan" TagPrefix="uc" %>
<%@ Register Src="PendingPOPeriodik.ascx" TagName="PendingPOPeriodik" TagPrefix="uc" %>
<%@ Register Src="PendingDOBelumRN.ascx" TagName="PendingDOBelumRN" TagPrefix="uc" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
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
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Window ID="wndDown" runat="server" Hidden="true" />
  
  <uc:SuratPesanan ID="SuratPesanan1" runat="server" Visible="false" />
  <uc:SuratPesananGudang ID="SuratPesananGudang1" runat="server" Visible="false" />
  <uc:SuratPesananPO ID="SuratPesananPO1" runat="server" Visible="false" />
  <uc:PurchaseOrderLogistik ID="PurchaseOrderLogistik1" runat="server" Visible="false" />
  <uc:PurchaseOrderFinance ID="PurchaseOrderFinance1" runat="server" Visible="false" />
  <uc:DeliveryOrder ID="DeliveryOrder1" runat="server" Visible="false" />
  <uc:ReturCustomer ID="ReturCustomer1" runat="server" Visible="false" />
  <uc:ReceiveNote ID="ReceiveNote1" runat="server" Visible="false" />
  <uc:ReturSuplier ID="ReturSuplier1" runat="server" Visible="false" />
  <uc:SuratTandaTerima ID="SuratTandaTerima1" runat="server" Visible="false" />
  <uc:PackingList ID="PackingList1" runat="server" Visible="false" />
  <uc:Combo ID="Combo1" runat="server" Visible="false" />
  <uc:SuratJalan ID="SuratJalan1" runat="server" Visible="false" />
  <uc:PendingPOPeriodik ID="PendingPOPeriodik" runat="server" Visible="false" />
  <uc:PendingDOBelumRN ID="PendingDOBelumRN" runat="server" Visible="false" /> <!--Indra D. 20170312-->
</asp:Content>
