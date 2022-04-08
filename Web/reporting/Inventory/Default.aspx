<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default.aspx.cs" Inherits="reporting_Inventory_Default" %>

<%@ Register Src="StokGudang.ascx" TagName="StokGudang" TagPrefix="uc" %>
<%@ Register Src="StockNasional.ascx" TagName="StockNasional" TagPrefix="uc" %>
<%@ Register Src="KartuBarangGudang.ascx" TagName="KartuBarangGudang" TagPrefix="uc" %>
<%@ Register Src="KartuBarangNasional.ascx" TagName="KartuBarangNasional" TagPrefix="uc" %>
<%@ Register Src="StockOpname.ascx" TagName="StockOpname" TagPrefix="uc" %>
<%@ Register Src="IndekStock.ascx" TagName="IndekStock" TagPrefix="uc" %>
<%@ Register Src="CurrentStock.ascx" TagName="CurrentStock" TagPrefix="uc" %>
<%@ Register Src="UmurStock.ascx" TagName="UmurStock" TagPrefix="uc" %>
<%@ Register Src="MutasiInventori.ascx" TagName="MutasiInventori" TagPrefix="uc" %>
<%@ Register Src="ExpireBatch.ascx" TagName="ExpireBatch" TagPrefix="uc" %>
<%@ Register Src="CurrentStockED.ascx" TagName="CurrentStockED" TagPrefix="uc" %>
<%@ Register Src="StockIntegrity.ascx" TagName="StockIntegrity" TagPrefix="uc" %>
<%@ Register Src="SPPLDO.ascx" TagName="SPPLDO" TagPrefix="uc" %>

<%@ Register Src="MonitoringExpired.ascx" TagName="MonitoringExpired" TagPrefix="uc" %>
<%@ Register Src="MonitoringExpiredCtrl.ascx" TagName="MonitoringExpiredCtrl" TagPrefix="uc" %>

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
  
  <uc:StokGudang ID="StokGudang" runat="server" Visible="false" />
  <uc:StockNasional ID="StockNasional" runat="server" Visible="false" />
  <uc:KartuBarangGudang ID="KartuBarangGudang" runat="server" Visible="false" />
  <uc:KartuBarangNasional ID="KartuBarangNasional" runat="server" Visible="false" />
  <uc:StockOpname ID="StockOpname" runat="server" Visible="false" />
  <uc:IndekStock ID="IndekStock" runat="server" Visible="false" />
  <uc:CurrentStock ID="CurrentStock" runat="server" Visible="false" />
  <uc:UmurStock ID="UmurStock" runat="server" Visible="false" />
  <uc:MutasiInventori ID="MutasiInventori" runat="server" Visible="false" />
  <uc:ExpireBatch ID="ExpireBatch" runat="server" Visible="false" />
  <uc:CurrentStockED ID="CurrentStockED" runat="server" Visible="false" />
  <uc:StockIntegrity ID="StockIntegrity" runat="server" Visible="false" />  
  <uc:SPPLDO ID="SPPLDO" runat="server" Visible="false" />
  <uc:MonitoringExpired ID="MonitoringExpired" runat="server" Visible="false" /> 
  <uc:MonitoringExpiredCtrl ID="MonitoringExpiredCtrl" runat="server" Visible="false" />
</asp:Content>
