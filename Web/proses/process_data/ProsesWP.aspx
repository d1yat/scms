<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProsesWP.aspx.cs" 
Inherits="proses_wp" MasterPageFile="~/Master.master" %>

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
          <ext:SelectBox ID="cbTipe" FieldLabel="Tipe Proses" runat="server" AllowBlank="false" SelectedIndex="0"
          Width="250">
          <Items>
            <ext:ListItem Text="Waktu Pelayanan Cabang" Value="1" />
            <ext:ListItem Text="Waktu Pelayanan Serah Terima" Value="2" />
            <ext:ListItem Text="Waktu Pelayanan Gudang Retur" Value="3" />
          </Items>
        </ext:SelectBox>
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="true" hidden="true" />
        </Items>
        <Buttons>
          <ext:Button runat="server" Icon="CogStart" Text="Proses">
              <DirectEvents>
                <Click OnEvent="Report_OnGenerate">
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