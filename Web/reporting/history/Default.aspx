<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default.aspx.cs" Inherits="reporting_history_Default" %>

<%@ Register Src="Asuransi.ascx" TagName="Asuransi" TagPrefix="uc" %>
<%@ Register Src="POTransaksi.ascx" TagName="POTransaksi" TagPrefix="uc" %>
<%@ Register Src="POLimit.ascx" TagName="POLimit" TagPrefix="uc" %>
<%@ Register Src="SuratPesanan.ascx" TagName="SuratPesanan" TagPrefix="uc" %>
<%@ Register Src="SuratPesananGudang.ascx" TagName="SuratPesananGudang" TagPrefix="uc" %>
<%@ Register Src="SuratPesananBatal.ascx" TagName="SuratPesananBatal" TagPrefix="uc" %>
<%@ Register Src="SuratTandaTerima.ascx" TagName="SuratTandaTerima" TagPrefix="uc" %>
<%@ Register Src="BlockItem.ascx" TagName="BlockItem" TagPrefix="uc" %>
<%@ Register Src="Combo.ascx" TagName="Combo" TagPrefix="uc" %>
<%@ Register Src="ItemBatchGudang.ascx" TagName="ItemBatchGudang" TagPrefix="uc" %>
<%@ Register Src="ItemBatchNasional.ascx" TagName="ItemBatchNasional" TagPrefix="uc" %>
<%@ Register Src="Expedisi.ascx" TagName="Expedisi" TagPrefix="uc" %>
<%@ Register Src="QueryPenjualan.ascx" TagName="QueryPenjualan" TagPrefix="uc" %>
<%@ Register Src="QueryPembelian.ascx" TagName="QueryPembelian" TagPrefix="uc" %>
<%@ Register Src="QueryClaim.ascx" TagName="QueryClaim" TagPrefix="uc" %>
<%@ Register Src="QuerySales.ascx" TagName="QuerySales" TagPrefix="uc" %>
<%@ Register Src="QueryPurchase.ascx" TagName="QueryPurchase" TagPrefix="uc" %>
<%@ Register Src="QuerySaldo.ascx" TagName="QuerySaldo" TagPrefix="uc" %>
<%@ Register Src="ResiEkspedisi.ascx" TagName="ResiEkspedisi" TagPrefix="uc" %>
<%@ Register Src="ResiEkspedisiDOSJ.ascx" TagName="ResiEkspedisiDOSJ" TagPrefix="uc" %>
<%@ Register Src="Shipment.ascx" TagName="Shipment" TagPrefix="uc" %>
<%@ Register Src="BiayaEkspedisi.ascx" TagName="BiayaEkspedisi" TagPrefix="uc" %>
<%@ Register Src="BiayaInvoiceVsResi.ascx" TagName="BiayaInvoiceVsResi" TagPrefix="uc" %>
<%@ Register Src="RekapInvoice.ascx" TagName="RekapInvoice" TagPrefix="uc" %>
<%@ Register Src="ListPembayaranEP.ascx" TagName="ListPembayaranEP" TagPrefix="uc" %>
<%@ Register Src="POPending.ascx" TagName="POPending" TagPrefix="uc" %>
<%@ Register Src="RNCabang.ascx" TagName="RNCabang" TagPrefix="uc" %>
<%@ Register Src="Pemusnahan.ascx" TagName="Pemusnahan" TagPrefix="uc" %>
<%@ Register Src="ReturSupplier.ascx" TagName="ReturSupplier" TagPrefix="uc" %>




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
  
  <uc:Asuransi ID="Asuransi1" runat="server" Visible="false" />
  <uc:POTransaksi ID="POTransaksi1" runat="server" Visible="false" />
  <uc:POLimit ID="POLimit1" runat="server" Visible="false" />
  <uc:SuratPesanan ID="SuratPesanan1" runat="server" Visible="false" />
  <uc:SuratPesananGudang ID="SuratPesananGudang1" runat="server" Visible="false" />
  <uc:SuratPesananBatal ID="SuratPesananBatal1" runat="server" Visible="false" />
  <uc:SuratTandaTerima ID="SuratTandaTerima1" runat="server" Visible="false" />
  <uc:BlockItem ID="BlockItem1" runat="server" Visible="false" />
  <uc:Combo ID="Combo1" runat="server" Visible="false" />
  <uc:ItemBatchGudang ID="ItemBatchGudang1" runat="server" Visible="false" />
  <uc:ItemBatchNasional ID="ItemBatchNasionall" runat="server" Visible="false" />
  <uc:Expedisi ID="Expedisi1" runat="server" Visible="false" />
  <uc:QueryPenjualan ID="QueryPenjualan1" runat="server" Visible="false" />
  <uc:QueryClaim ID="QueryClaim1" runat="server" Visible="false" />
  <uc:QueryPembelian ID="QueryPembelian1" runat="server" Visible="false" />
  <uc:QuerySales ID="QuerySales1" runat="server" Visible="false" />
  <uc:QueryPurchase ID="QueryPurchase" runat="server" Visible="false" />
  <uc:QuerySaldo ID="QuerySaldo" runat="server" Visible="false" />
  <uc:ResiEkspedisi ID="ResiEkspedisi" runat="server" Visible="false" />
  <uc:ResiEkspedisiDOSJ ID="ResiEkspedisiDOSJ" runat="server" Visible="false" />
  <uc:Shipment ID="Shipment" runat="server" Visible="false" />
  <uc:BiayaEkspedisi ID="BiayaEkspedisi" runat="server" Visible="false" />
  <uc:BiayaInvoiceVsResi ID="BiayaInvoiceVsResi" runat="server" Visible="false" />
  <uc:RekapInvoice ID="RekapInvoice" runat="server" Visible="false" />
  <uc:ListPembayaranEP ID="ListPembayaranEP" runat="server" Visible="false" />  
  <uc:POPending ID="POPending" runat="server" Visible="false" />  
  <uc:RNCabang ID="RNCabang" runat="server" Visible="false" />  
  <uc:Pemusnahan ID="Pemusnahan" runat="server" Visible="false" />  
  <uc:ReturSupplier ID="ReturSupplier1" runat="server" Visible="false" /> 
</asp:Content>
