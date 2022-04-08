<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OktPrekursorBulanan.ascx.cs" Inherits="reporting_Transaksi_OktPrekursorBulanan" %>

<ext:Panel ID="Panel1" runat="server">
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:FormPanel ID="frmReportSJ" Height="200" runat="server" Padding="5" Frame="True" Layout="Form">
          <Items>
              <ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
              <ext:SelectBox ID="cbBulan" runat="server" FieldLabel="Bulan" SelectedIndex="0"
                AllowBlank="false">
                <Items>
                  <ext:ListItem Value="01" Text="Januari" />
                  <ext:ListItem Value="02" Text="Februari" />
                  <ext:ListItem Value="03" Text="Maret" />
                  <ext:ListItem Value="04" Text="April" />
                  <ext:ListItem Value="05" Text="Mei" />
                  <ext:ListItem Value="06" Text="Juni" />
                  <ext:ListItem Value="07" Text="Juli" />
                  <ext:ListItem Value="08" Text="Agustus" />
                  <ext:ListItem Value="09" Text="September" />
                  <ext:ListItem Value="10" Text="Oktober" />
                  <ext:ListItem Value="11" Text="November" />
                  <ext:ListItem Value="12" Text="Desember" />
                </Items>
          </ext:SelectBox>
          <ext:SelectBox ID="cbPODO" runat="server" FieldLabel="PO/DO" SelectedIndex="0"
            AllowBlank="false">
            <Items>
              <ext:ListItem Value="01" Text="PO" />
              <ext:ListItem Value="02" Text="DO" />
            </Items>
          </ext:SelectBox>
          <%--PJ--%>
          <ext:SelectBox ID="cbJenis" runat="server" FieldLabel="Jenis" SelectedIndex="0"
            AllowBlank="false">
            <Items>
              <ext:ListItem Value="01" Text="OKT" />
              <ext:ListItem Value="02" Text="Prekursor" />
              <ext:ListItem Value="03" Text="OOT" />
              
            </Items>
          </ext:SelectBox>
          <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="1"
          AllowBlank="false">
          <Items>
            <ext:ListItem Value="01" Text="PDF" />
            <ext:ListItem Value="02" Text="Excel Data Only" />
            <ext:ListItem Value="03" Text="Excel" />
          </Items>
          </ext:SelectBox>
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button ID="Button1" runat="server" Icon="ReportGo" Text="Generate">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmReportSJ});">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
  </Buttons>
  <Listeners>
    <AfterRender Handler="initializePanel(#{frmReportSJ}, #{chkAsync});" />
  </Listeners>
</ext:Panel>
