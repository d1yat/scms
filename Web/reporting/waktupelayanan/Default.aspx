<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default.aspx.cs" Inherits="reporting_waktupelayanan_Default" %>

<%@ Register Src="WPCabang.ascx" TagName="WPCabang" TagPrefix="uc" %>
<%@ Register Src="WPSuplier.ascx" TagName="WPSuplier" TagPrefix="uc" %>
<%@ Register Src="WPSuplierCabang.ascx" TagName="WPSuplierCabang" TagPrefix="uc" %>
<%@ Register Src="WPSerahTerima.ascx" TagName="WPSerahTerima" TagPrefix="uc" %>
<%@ Register Src="WPSPPLConfirm.ascx" TagName="WPSPPLConfirm" TagPrefix="uc" %>
<%@ Register Src="WPSTGudangRetur.ascx" TagName="WPSTGudangRetur" TagPrefix="uc" %>


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
  
  <uc:WPCabang ID="WPCabang1" runat="server" Visible="false" />
  <uc:WPSuplier ID="WPSuplier1" runat="server" Visible="false" />
  <uc:WPSuplierCabang ID="WPSuplierCabang1" runat="server" Visible="false" />
  <uc:WPSerahTerima ID="WPSerahTerima1" runat="server" Visible="false" />
  <uc:WPSPPLConfirm ID="WPSPPLConfirm1" runat="server" Visible="false" />
  <uc:WPSTGudangRetur ID="WPSTGudangRetur1" runat="server" Visible="false" />
</asp:Content>
