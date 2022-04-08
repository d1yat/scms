<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default.aspx.cs" Inherits="reporting_konsolidasi_Default" %>

<%@ Register Src="BDP.ascx" TagName="BDP" TagPrefix="uc" %>
<%@ Register Src="RDP.ascx" TagName="RDP" TagPrefix="uc" %>
<%@ Register Src="StokNasional.ascx" TagName="StokNasional" TagPrefix="uc" %>
<%@ Register Src="BDPPharos.ascx" TagName="BDPPharos" TagPrefix="uc" %>

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
  
  <uc:BDP ID="BDP1" runat="server" Visible="false" />
  <uc:RDP ID="RDP1" runat="server" Visible="false" />
  <uc:StokNasional ID="StokNasional1" runat="server" Visible="false" />
  <uc:BDPPharos ID="BDPPharos1" runat="server" Visible="false" />
</asp:Content>
