<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterPrinsipalCtrl.ascx.cs" 
Inherits="master_prinsipal_MasterPrinsipalCtrl" %>

<ext:Window ID="winDetail" runat="server" Height="480" Width="1000" Hidden="true"
  Maximizable="true" MinHeight="500" MinWidth="1000" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfNoSup" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Height="195" Padding="10" Layout="Column">
          <Items>
            <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".35"
             Layout="Form" LabelAlign="Left">
              <Items>
                <ext:TextField ID="txNama" runat="server" FieldLabel="Nama" Width="185" AllowBlank="false" />
                
                <ext:TextField ID="txOwner" runat="server" FieldLabel="Pemilik" Width="185" />
                <ext:TextField ID="txContact" runat="server" FieldLabel="Contact" Width="185" AllowBlank="false" />
                <ext:TextArea ID="txAlamat1" runat="server" FieldLabel="Alamat 1" Width="185" AllowBlank="false"/>
                <ext:TextArea ID="txAlamat2" runat="server" FieldLabel="Alamat 2" Width="185" />
                <ext:TextField ID="txArea" runat="server" FieldLabel="Area" MaxLength="5" Width="50"  />
                <ext:TextField ID="txPhone1" runat="server" FieldLabel="Phone 1" Width="100"  />
                <ext:TextField ID="txPhone2" runat="server" FieldLabel="Phone 2" Width="100" />
                <ext:TextField ID="txPhone3" runat="server" FieldLabel="Phone 3" Width="100" />
                <ext:TextField ID="txFax1" runat="server" FieldLabel="Fax 1" Width="100" />
                <ext:TextField ID="txFax2" runat="server" FieldLabel="Fax 2" Width="100" />
              </Items>
            </ext:Panel>  
            <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".35"
             Layout="Form" LabelAlign="Left">
              <Items>
                <ext:TextField ID="txFax3" runat="server" FieldLabel="Fax 3" Width="100" />
                <ext:TextField ID="txBank" runat="server" FieldLabel="Bank" Width="185"/>
                <ext:TextArea ID="txAlamatBank" runat="server" FieldLabel="Alamat Bank" Width="185"/>
                <ext:NumberField ID="txTop" runat="server" FieldLabel="TOP" MaxLength="5" />
                <ext:TextField ID="txAcc1" runat="server" FieldLabel="Account 1" />
                <ext:TextField ID="txAcc2" runat="server" FieldLabel="Account 2" />
                <ext:TextField ID="txNpwp" runat="server" FieldLabel="NPWP" />
                <%--Indra 20180815FM--%>
                <%--<ext:NumberField ID="txLead" runat="server" FieldLabel="Lead Time" AllowBlank="false" />--%>
                <ext:NumberField ID="txLead" runat="server" FieldLabel="Lead Time" AllowBlank="true" Hidden="true" />
                <ext:TextField ID="txAcc" runat="server" FieldLabel="Account" />
                <ext:NumberField ID="txIndex" runat="server" FieldLabel="Index" DecimalPrecision="2" AllowDecimals="true" />
                <ext:NumberField ID="txDisc" runat="server" FieldLabel="Disc" DecimalPrecision="2" AllowDecimals="true" />
                <ext:TextField ID="txKodeGol" runat="server" FieldLabel="Kode Gol" Width="50" MaxLength="5" />
              </Items>
            </ext:Panel>    
            <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".3"
             Layout="Form" LabelAlign="Left">
             <Items>              
                <ext:TextField ID="txNamaTax" runat="server" FieldLabel="Nama Tax" Width="185" AllowBlank="false" />
                <ext:TextArea ID="txAlamatTax1" runat="server" FieldLabel="Alamat 1" Width="185" AllowBlank="false"/>
                <ext:TextArea ID="txAlamatTax2" runat="server" FieldLabel="Alamat 2" Width="185" />
                <ext:TextField ID="txNppkp" runat="server" FieldLabel="NPPKP" />
                <ext:DateField ID="dtNppkp" runat="server" FieldLabel="Tanggal NPPKP" Width="125"/>
                <ext:TextField ID="txTax" runat="server" FieldLabel="Tax" />
                <ext:Checkbox ID="chkImport" runat="server" FieldLabel="Import" Width="50" />
                <ext:Checkbox ID="chkFax" runat="server" FieldLabel="Fax" Width="50" />
                <ext:Checkbox ID="chkKons" runat="server" FieldLabel="Konsinyasi" Width="50" />
                <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" Width="50" />
                <ext:Checkbox ID="chkisHide" runat="server" FieldLabel="Hide" Width="50" />
             </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoSup}.getValue()" Mode="Raw" />
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
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />