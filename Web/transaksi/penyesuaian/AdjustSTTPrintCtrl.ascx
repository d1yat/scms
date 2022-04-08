<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdjustSTTPrintCtrl.ascx.cs"
  Inherits="transaksi_penyesuaian_AdjustSTTPrintCtrl" %>
<ext:Window ID="winPrintDetail" runat="server" Height="125" Width="390" Hidden="true"
  Maximizable="false" Layout="Fit" Icon="Printer">
  <Content>
    <ext:Hidden ID="hfType" runat="server" />
  </Content>
  <Items>
    <ext:FormPanel runat="server" Layout="Form" Padding="5" LabelWidth="150">
      <Items>
        <ext:CompositeField runat="server" FieldLabel="Nomor Adjustment STT">
          <Items>
            <ext:TextField ID="stNumber1" runat="server" MaxLength="10" Width="90" />
            <ext:Label runat="server" Text=" - " />
            <ext:TextField ID="stNumber2" runat="server" MaxLength="10" Width="90" />
          </Items>
        </ext:CompositeField>
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" />
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button runat="server" Text="Cetak" Icon="PrinterGo">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate">
          <Confirmation ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="TypeCode" Value="#{hfType}.getValue()" Mode="Raw" />
            <ext:Parameter Name="STID1" Value="#{stNumber1}.getValue()" Mode="Raw" />
            <ext:Parameter Name="STID2" Value="#{stNumber2}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Async" Value="#{chkAsync}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
