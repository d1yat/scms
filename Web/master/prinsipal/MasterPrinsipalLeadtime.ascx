<%--
 Created By Indra
 20171231FM
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterPrinsipalLeadtime.ascx.cs" 
Inherits="master_prinsipal_MasterPrinsipalLeadtime" %>

<ext:Window ID="winDetail" runat="server" Height="400" Width="340" Hidden="true" Resizable="false"
  Maximizable="false" MinHeight="400" MinWidth="340" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfNoSup" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeDO" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="210" MaxHeight="210" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Height="400" Padding="10" Layout="FitLayout">
          <Items>
            <ext:Panel runat="server" Border="false" Header="false" Layout="Form" LabelAlign="Left">
              <Items>
                <ext:TextField ID="txKodePemasok" runat="server" FieldLabel="Kode Pemasok" Width="60" />
                <ext:TextField ID="txNamaPemasok" runat="server" FieldLabel="Nama Pemasok" Width="185" />
                <ext:NumberField ID="nmLeadtimeAwal" runat="server" FieldLabel="Leadtime Awal" Width="60" />
                <ext:NumberField ID="nmLeadtimePerubahan" runat="server" FieldLabel="Leadtime Perubahan" Width="60" MaxLength="3" />
                <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Efektive Date Leadtime">
                  <Items>
                    <ext:DateField ID="dtEfektiveDate" runat="server" Format="dd-MM-yyyy" EnableKeyEvents="true" />
                  </Items>
                </ext:CompositeField>            
                <ext:TextField ID="txAlasanPerubahan" runat="server" FieldLabel="Alasan Perubahan" Width="185" MaxLength="100" />
                <ext:TextField ID="txRequestor" runat="server" FieldLabel="Nama Requestor" Width="185" />
                <ext:DateField ID="dtTglRequest" runat="server" FieldLabel="Tanggal Request" Width="100" Format="dd-MM-yyyy" />
                <ext:TextField ID="txAlasanTolakSetuju" runat="server" FieldLabel="Alasan Batal/Tolak/Setuju" Width="185" MaxLength="100" />
              </Items>
            </ext:Panel>              
          </Items>
        </ext:FormPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button runat="server" ID="btnApprove" Icon="Tick" Text="Approv Pengajuan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoSup}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="01" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>  
    <ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Ubah Leadtime">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoSup}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="02" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" ID="btnCancel" Icon="StopRed" Text="Batal Pengajuan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoSup}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="03" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>        
    <ext:Button runat="server" ID="btnReject" Icon="Cross" Text="Reject Pengajuan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoSup}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="04" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>   
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />