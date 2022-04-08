<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ClosingStock.aspx.cs" 
Inherits="proses_closing_ClosingStock" MasterPageFile="~/Master.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" AutoScroll="true" Layout="FormLayout"
        Padding="5" ButtonAlign="Center">
        <Items>
          <ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
          <ext:SelectBox ID="cbBulan" runat="server" FieldLabel="Bulan" Width="100" AllowBlank="false" />
        </Items>
        <Buttons>
          <ext:Button runat="server" Icon="CogStart" Text="Proses">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click">
                  <Confirmation ConfirmRequest="true" Title="Proses ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="Tahun" Value="#{cbTahun}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Bulan" Value="#{cbBulan}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
        </Buttons>
      </ext:Panel>
    </Items>
   </ext:Viewport>
</asp:Content>