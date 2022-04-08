<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterDriverCtrl.ascx.cs" Inherits="master_driver_DriverCtrl" %>


  <script type="text/javascript">
 var cekTipe = function(cb, txnip) {
      if (Ext.isEmpty(cb)) {
          if (!Ext.isEmpty(txnip)) {
              txnip.disable();
        }
        return;
      }

      if (cb.getValue() == '01') {
          if (!Ext.isEmpty(txnip)) {
              txnip.enable();
        }
      }
      else {
          if (!Ext.isEmpty(txnip)) {
              txnip.disable();
        }
      }
    }
</script>

<ext:Window ID="winDetail" runat="server" Height="250" Width="500" Hidden="true"
  Maximizable="true" MinHeight="250" MinWidth="450" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGdgId" runat="server" />
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
                     <ext:ComboBox ID="cbTipeJenis" runat="server" FieldLabel="Tipe" DisplayField="v_ket" ValueField="c_type"
                        Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                        AllowBlank="true" ForceSelection="false">
                        <CustomConfig>
                         <ext:ConfigItem Name="allowBlank" Value="true" />
                        </CustomConfig>
                        <Store>
                          <ext:Store ID="Store2" runat="server">
                            <Proxy>
                              <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                CallbackParam="soaScmsCallback" />
                            </Proxy>
                            <BaseParams>
                              <ext:Parameter Name="start" Value="={0}" />
                              <ext:Parameter Name="limit" Value="={10}" />
                              <ext:Parameter Name="model" Value="2001" />
                              <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '002', 'System.String'],
                              ['c_portal = @0', '9', 'System.Char']]" Mode="Raw" />
                              <ext:Parameter Name="sort" Value="c_notrans" />
                              <ext:Parameter Name="dir" Value="ASC" />
                            </BaseParams>
                            <Reader>
                              <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                                TotalProperty="d.totalRows">
                                <Fields>
                                  <ext:RecordField Name="c_type" />
                                  <ext:RecordField Name="v_ket" />
                                </Fields>
                              </ext:JsonReader>
                            </Reader>
                          </ext:Store>
                        </Store>
                        <Template ID="Template2" runat="server">
                          <Html>
                             <table cellpading="0" cellspacing="1" style="width: 200px">
                             <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                             <tpl for="."><tr class="search-item">
                             <td>{c_type}</td><td>{v_ket}</td>
                             </tr></tpl>
                             </table>
                          </Html>
                        </Template>
                        <Listeners>
                            <Select Handler="cekTipe(this, #{txNip});" />
                        </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txNip" runat="server" MaxLength="7" FieldLabel="NIP" Width="85" AllowBlank="false" />
                    <ext:TextField ID="txNama" runat="server" MaxLength="50" FieldLabel="Nama" Width="250" AllowBlank="false" />
                    <ext:TextField ID="txNopol" runat="server" MaxLength="9" FieldLabel="No. Polisi" Width="85" AllowBlank="false" />
                    <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" />
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
            <ext:Parameter Name="NumberID" Value="#{hfGdgId}.getValue()" Mode="Raw" />
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




