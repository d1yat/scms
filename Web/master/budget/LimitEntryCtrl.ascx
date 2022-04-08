<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LimitEntryCtrl.ascx.cs"
  Inherits="master_budget_LimitEntryCtrl" %>

<script language="javascript" type="text/javascript">
//  var selTextSBox = function(sb) {
//    var valu = '2010';
//    var rec = sb.findRecord('value', valu);
//    if (Ext.isEmpty(rec)) {
//      sb.addItem(valu, valu);
//    }
//    sb.setValueAndFireSelect(valu);
//  }
</script>

<ext:Window ID="winDetail" runat="server" Height="225" Width="325" Hidden="true"
  MinHeight="225" MinWidth="325" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfCode" runat="server" />
    <ext:Hidden ID="hfSuplier" runat="server" />
    <ext:Hidden ID="hfStoreHdrID" runat="server" />
    <ext:Hidden ID="hfStoreDtlID" runat="server" />
    <%--<ext:Hidden ID="hfTahun" runat="server" />
    <ext:Hidden ID="hfBulan" runat="server" />--%>
  </Content>
  <Items>
    <ext:FormPanel ID="frmHeaders" runat="server" Padding="5" >
      <%--<Defaults>
        <ext:Parameter Name="anchor" Value="100%" Mode="Value" />
        <ext:Parameter Name="msgTarget" Value="side" Mode="Value" />
      </Defaults>--%>
      <Items>
        <ext:Label ID="lbSuplier" runat="server" FieldLabel="Pemasok" />
        <ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
        <ext:SelectBox ID="cbBulan" runat="server" FieldLabel="Bulan" AllowBlank="false" />
        <ext:NumberField ID="txLimit" runat="server" FieldLabel="Anggaran" AllowBlank="false" />
        <ext:NumberField ID="txPersent" runat="server" FieldLabel="% Anggaran" Width="50"
          AllowBlank="true" AllowDecimals="true" AllowNegative="false" MinValue="0" MaxValue="100" />
      </Items>
      <Buttons>
        <%--<ext:Button ID="btnAddData" runat="server" Text="Tambah" Icon="Add">
        </ext:Button>
        <ext:Button ID="btnResetData" runat="server" Text="Hitung Ulang" Icon="CogStart">
          <DirectEvents>
            <Click OnEvent="ResetBtn_Click">
              <EventMask ShowMask="true" />
              <Confirmation ConfirmRequest="true" Title="Hitung Ulang ?" Message="Anda yakin mau menghitung ulang ?" />
            </Click>
          </DirectEvents>
        </ext:Button>--%>
        <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
          <DirectEvents>
            <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
              <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
                ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="StoreHdrID" Value="#{hfStoreHdrID}.getValue()" Mode="Raw" />
                <ext:Parameter Name="StoreDtlID" Value="#{hfStoreDtlID}.getValue()" Mode="Raw" />
                <ext:Parameter Name="CodeID" Value="#{hfCode}.getValue()" Mode="Raw" />
                <ext:Parameter Name="SuplierID" Value="#{hfSuplier}.getValue()" Mode="Raw" />
                <ext:Parameter Name="Tahun" Value="#{cbTahun}.getValue()" Mode="Raw" />
                <ext:Parameter Name="Bulan" Value="#{cbBulan}.getValue()" Mode="Raw" />
                <ext:Parameter Name="Limit" Value="#{txLimit}.getValue()" Mode="Raw" />
                <ext:Parameter Name="Persentase" Value="#{txPersent}.getValue()" Mode="Raw" />
              </ExtraParams>
            </Click>
          </DirectEvents>
        </ext:Button>
        <%--<ext:Button runat="server" Text="Testing">
          <Listeners>
            <Click Handler="selTextSBox(#{cbTahun});" />
          </Listeners>
        </ext:Button>--%>
        <ext:Button ID="btnHide" runat="server" Icon="Cancel" Text="Keluar">
          <Listeners>
            <Click Handler="#{winDetail}.hide();" />
          </Listeners>
        </ext:Button>
      </Buttons>
    </ext:FormPanel>
  </Items>
</ext:Window>
