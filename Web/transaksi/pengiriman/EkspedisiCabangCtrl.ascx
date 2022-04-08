<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EkspedisiCabangCtrl.ascx.cs"
  Inherits="transaksi_pengiriman_EkspedisiCabangCtrl" %>

<script type="text/javascript">
  var validasiJam = function(obj) {
    if (Ext.isEmpty(obj)) {
      return;
    }

    var valu = obj.getValue();
    var tgl = (Ext.isDate(valu) ? valu : Date.parseDate(valu, 'g:i:s'));

    obj.setValue(myFormatTime(tgl));
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="350" Width="400" Hidden="true"
  Maximizable="true" MinHeight="350" MinWidth="400" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfExpNoCab" runat="server" />
    <ext:Hidden ID="hfExpNo" runat="server" />
    <ext:Hidden ID="hfDateResi" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="145" MaxHeight="145" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="145" Padding="10">
          <Items>
            <ext:Label runat="server" ID="lblCabang" FieldLabel="Cabang" />
            <ext:Label runat="server" ID="lblEksDesc" FieldLabel="Ekspedisi" />
            <ext:Label runat="server" ID="lblNoResi" FieldLabel="No Resi" />
            <ext:Label runat="server" ID="lblDateResi" FieldLabel="Tgl Resi" />
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="75">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="75" Layout="Fit">
          <Items>
            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Border="false"
              Padding="5">
              <Items>
                <ext:DateField ID="txDayTerimaHdr" runat="server" FieldLabel="Tanggal Terima" AllowBlank="false"
                  Format="dd-MM-yyyy" />
                <ext:TextField ID="txTimeTerimaHdr" runat="server" FieldLabel="Jam Terima" MaxLength="8"
                  AllowBlank="false" Width="75">
                  <Listeners>
                    <Change Fn="validasiJam" />
                  </Listeners>
                  <Plugins>
                    <ux:InputTextMask Mask="99:99:99" />
                  </Plugins>
                </ext:TextField>
              </Items>
            </ext:FormPanel>
          </Items>
        </ext:Panel>
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
            <ext:Parameter Name="NumberID" Value="#{hfExpNoCab}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Day" Value="#{txDayTerimaHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Time" Value="#{txTimeTerimaHdr}.getValue()" Mode="Raw" />
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
