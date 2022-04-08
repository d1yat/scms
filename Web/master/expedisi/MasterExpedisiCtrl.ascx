<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterExpedisiCtrl.ascx.cs"
  Inherits="master_expedisi_MasterExpedisiCtrl" %>
  
<script type="text/javascript" language="javascript">
  var changeTipeReport = function(itm, comField) {
    var isCek = itm;
    if (isCek) {
        comField.show();
    }
    else {
        comField.hide();
    }    
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="250" Width="375" Hidden="true"
  Maximizable="true" MinHeight="235" MinWidth="375" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfNoExp" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:FormPanel ID="frmHeaders" runat="server" Height="225"
      Padding="10">
      <Items>
        <ext:TextField ID="txName" runat="server" FieldLabel="Nama" Width="225" AllowBlank="false" />
        <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" Width="50" />
        <ext:Checkbox ID="chkUdara" runat="server" FieldLabel="Udara" Width="50" />
        <ext:Checkbox ID="chkDarat" runat="server" FieldLabel="Darat" Width="50" />
        <ext:Checkbox ID="chkLaut" runat="server" FieldLabel="Laut" Width="50" />
        <ext:Checkbox ID="chkImport" runat="server" FieldLabel="Import" Width="50" />
        <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="NPWP">
          <Items>
            <ext:Checkbox ID="chkNpwp" runat="server"  Width="50" >
                <Listeners>
                  <Check Handler="changeTipeReport(checked, #{txNpwp});" />
                </Listeners>
            </ext:Checkbox>
            <ext:TextField ID="txNpwp" runat="server" Hidden="true"/>
          </Items>
        </ext:CompositeField>
        
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
            Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoExp}.getValue()" Mode="Raw" />
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
    <ext:Button ID="Button2" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
