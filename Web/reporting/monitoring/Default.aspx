<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" 
Inherits="reporting_monitoring_Default" MasterPageFile="~/Master.master" %>

<%@ Register Src="PackingList.ascx" TagName="PackingList" TagPrefix="uc" %>
<%@ Register Src="PackingListBooked.ascx" TagName="PackingListBooked" TagPrefix="uc" %>
<%@ Register Src="PackingListConf.ascx" TagName="PackingListConf" TagPrefix="uc" %>
<%@ Register Src="SendDeliveryOrder.ascx" TagName="SendDeliveryOrder" TagPrefix="uc" %>
<%@ Register Src="SendReturCustomer.ascx" TagName="SendReturCustomer" TagPrefix="uc" %>
<%@ Register Src="SalesNasional.ascx" TagName="SalesNasional" TagPrefix="uc" %>
<%@ Register Src="StockIntegrityPending.ascx" TagName="StockIntegrityPending" TagPrefix="uc" %>
<%--hafizh--%>
<%@ Register Src="MONITORINGPRODUKTIFITASDC.ascx" TagName="ProduktifitasDC" TagPrefix="uc" %>

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
  
  <uc:PackingList ID="PackingList" runat="server" Visible="false" />
  <uc:PackingListBooked ID="PackingListBooked" runat="server" Visible="false" />
  <uc:PackingListConf ID="PackingListConf" runat="server" Visible="false" />
  <uc:SendDeliveryOrder ID="SendDeliveryOrder" runat="server" Visible="false" />
  <uc:SendReturCustomer ID="SendReturCustomer" runat="server" Visible="false" />
  <uc:SalesNasional ID="SalesNasional" runat="server" Visible="false" />
  <uc:StockIntegrityPending ID="StockIntegrityPending" runat="server" Visible="false" />
  <uc:ProduktifitasDC ID="ProduktifitasDC" runat="server" Visible="false" />
  
 </asp:Content>

