<%@ Control Language="C#" AutoEventWireup="true" CodeFile="KursCtrl.ascx.cs" Inherits="master_kurs_KursCtrl" %>
<ext:Window ID="winDetail" runat="server" Height="250" Width="400" Hidden="true"
  Maximizable="true" MinHeight="250" MinWidth="400" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfKursId" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="300">
        <ext:FormPanel ID="frmHeaders" runat="server" Height="400" Padding="10">
          <Items>
            <ext:TextField ID="txSimbol" runat="server" FieldLabel="Simbol" AllowDecimals="true"
              AllowNegative="false" DecimalPrecision="2" MaxLength="20" Width="75" />
            <ext:TextField ID="txDesc" runat="server" MaxLength="50" FieldLabel="Deskripsi" />
            <ext:TextField ID="txDescFull" runat="server" MaxLength="50" FieldLabel="Deskripsi Asli"
              Width="250" />
            <ext:TextField ID="txCurr" runat="server" MaxLength="50" FieldLabel="Pecahan" Width="250" />
            <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" />
          </Items>
        </ext:FormPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
            Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfKursId}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
