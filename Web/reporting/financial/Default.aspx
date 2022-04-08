<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" 
Inherits="reporting_financial_Default" MasterPageFile="~/Master.master" %>

<%@ Register Src="APFakturPending.ascx" TagName="APFakturPending" TagPrefix="uc" %>
<%@ Register Src="APGL.ascx" TagName="APGL" TagPrefix="uc" %>
<%@ Register Src="APList.ascx" TagName="APList" TagPrefix="uc" %>
<%@ Register Src="APListBayar.ascx" TagName="APListBayar" TagPrefix="uc" %>
<%@ Register Src="ARFakturPending.ascx" TagName="ARFakturPending" TagPrefix="uc" %>
<%@ Register Src="ARGL.ascx" TagName="ARGL" TagPrefix="uc" %>
<%@ Register Src="ARList.ascx" TagName="ARList" TagPrefix="uc" %>
<%@ Register Src="ARListBayar.ascx" TagName="ARListBayar" TagPrefix="uc" %>
<%@ Register Src="HPPDivAMS.ascx" TagName="HPPDivAMS" TagPrefix="uc" %>
<%@ Register Src="HPPDivAMSExtDisc.ascx" TagName="HPPDivAMSExtDisc" TagPrefix="uc" %>
<%@ Register Src="HPPDivPrinsipal.ascx" TagName="HPPDivPrinsipal" TagPrefix="uc" %>
<%@ Register Src="BeaKirim.ascx" TagName="BeaKirim" TagPrefix="uc" %>
<%@ Register Src="JatuhTempo.ascx" TagName="JatuhTempo" TagPrefix="uc" %>
<%@ Register Src="Pembayaran.ascx" TagName="Pembayaran" TagPrefix="uc" %>
<%@ Register Src="Claim.ascx" TagName="Claim" TagPrefix="uc" %>
<%@ Register Src="ClaimAcc.ascx" TagName="ClaimAcc" TagPrefix="uc" %>
<%@ Register Src="E-Faktur.ascx" TagName="Efaktur" TagPrefix="uc" %>
<%@ Register Src="FakturManual.ascx" TagName="FakturManual" TagPrefix="uc" %>



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
  
  <uc:APFakturPending ID="APFakturPending" runat="server" Visible ="false" />
  <uc:APGL ID="APGL" runat="server" Visible ="false" />
  <uc:APList ID="APList" runat="server" Visible ="false" />
  <uc:APListBayar ID="APListBayar" runat="server" Visible ="false" />
  <uc:ARFakturPending ID="ARFakturPending" runat="server" Visible ="false" />
  <uc:ARGL ID="ARGL" runat="server" Visible ="false" />
  <uc:ARList ID="ARList" runat="server" Visible ="false" />
  <uc:ARListBayar ID="ARListBayar" runat="server" Visible ="false" />
  <uc:HPPDivAMS ID="HPPDivAMS" runat="server" Visible ="false" />
  <uc:HPPDivAMSExtDisc ID="HPPDivAMSExtDisc" runat="server" Visible ="false" />
  <uc:HPPDivPrinsipal ID="HPPDivPrinsipal" runat="server" Visible ="false" />
  <uc:BeaKirim ID="BeaKirim" runat="server" Visible ="false" />
  <uc:JatuhTempo ID="JatuhTempo" runat="server" Visible ="false" />
  <uc:Pembayaran ID="Pembayaran" runat="server" Visible ="false" />
  <uc:Claim ID="Claim" runat="server" Visible ="false" />
  <uc:ClaimAcc ID="ClaimAcc" runat="server" Visible ="false" /> 
  <uc:Efaktur ID="Efaktur" runat="server" Visible ="false" /> 
  <uc:FakturManual ID="FakturManual" runat="server" Visible ="false" /> 
</asp:Content>
