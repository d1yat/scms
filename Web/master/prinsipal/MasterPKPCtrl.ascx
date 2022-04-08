<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterPKPCtrl.ascx.cs" Inherits="master_pkpCtrl" %>

<ext:Window ID="winDetail" runat="server" Height="500" Width="500" Hidden="true"
  Maximizable="true" MinHeight="250" MinWidth="500" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfPKPno" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="300">
        <ext:FormPanel ID="frmHeaders" runat="server" Height="550" Padding="10">
          <Items>
            <ext:FormPanel ID="FormPanel1" runat="server" Header="false" BodyBorder="false" MonitorPoll="500"
              MonitorValid="true" Padding="0" Width="720" Height="550" ButtonAlign="Right" Layout="Column">
              <Items>
                <ext:Panel ID="pnlKiri" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <ext:TextField ID="txPKPNameHdr" runat="server" AllowBlank="false" FieldLabel="Nama"
                          Width="150" />
                    <ext:TextField ID="txNPWP" runat="server" AllowBlank="false" FieldLabel="NPWP"
                          Width="150" />
                    <ext:TextField ID="txNPPKP" runat="server" AllowBlank="false" FieldLabel="NPPKP"
                          Width="150" />
                    <ext:DateField ID="txNPPKPDateHdr" runat="server" AllowBlank="false" FieldLabel="Tanggal NPPKP"
                          Width="100" Format="dd-MM-yyyy" />   
                    <ext:TextArea ID="txAlamat1" runat="server" AllowBlank="true" FieldLabel="Alamat 1"
                          Width="250" Height="100" MaxLength="100" /> 
                    <ext:TextArea ID="txAlamat2" runat="server" AllowBlank="true" FieldLabel="Alamat 2"
                          Width="250" Height="100" MaxLength="100" /> 
                    <ext:TextField ID="txPhone1" runat="server" AllowBlank="false" FieldLabel="Phone 1"
                          Width="150" />
                    <ext:TextField ID="txFax1" runat="server" AllowBlank="false" FieldLabel="Fax 1"
                          Width="150" />
                    <ext:TextField ID="txFax2" runat="server" AllowBlank="false" FieldLabel="Fax 2"
                          Width="150" />
                    <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" Width="50" />
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
            <ext:Parameter Name="NumberID" Value="#{hfPKPno}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
      <DirectEvents>
        <Click OnEvent="ReloadBtn_Click">
          <EventMask ShowMask="true" />
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




