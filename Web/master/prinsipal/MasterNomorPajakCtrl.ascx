<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterNomorPajakCtrl.ascx.cs" Inherits="master_nomorpajakCtrl" %>

<ext:Window ID="winDetail" runat="server" Height="250" Width="500" Hidden="true"
  Maximizable="true" MinHeight="250" MinWidth="450" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfIDXno" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="300">
        <ext:FormPanel ID="frmHeaders" runat="server" Height="400" Padding="10">
          <Items>
            <ext:FormPanel ID="FormPanel1" runat="server" Header="false" BodyBorder="false" MonitorPoll="500"
              MonitorValid="true" Padding="0" Width="720" Height="400" ButtonAlign="Right" Layout="Column">
              <Items>
                <ext:Panel ID="pnlKiri" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <ext:TextField ID="txTahun" runat="server" MaxLength="4" FieldLabel="Tahun" Width="50" AllowBlank="false" />
                    <ext:TextField ID="txDigit1" runat="server" MaxLength="3" FieldLabel="Digit 1" Width="50" AllowBlank="false" />
                    <ext:TextField ID="txDigit2" runat="server" MaxLength="3" FieldLabel="Digit 2" Width="50" AllowBlank="false" />
                    <ext:TextField ID="txAwal" runat="server" MaxLength="8" FieldLabel="Nomor Awal" Width="85" AllowBlank="false" />
                    <ext:TextField ID="txAkhir" runat="server" MaxLength="8" FieldLabel="Nomor Akhir" Width="85" AllowBlank="false" />
                    <ext:TextField ID="txCurrent" runat="server" MaxLength="8" FieldLabel="Current" Width="85" AllowBlank="false" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:FormPanel>
          </Items>
        </ext:FormPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfIDXno}.getValue()" Mode="Raw" />
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




