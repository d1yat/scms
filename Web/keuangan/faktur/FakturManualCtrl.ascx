<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FakturManualCtrl.ascx.cs" Inherits="faktur_manualCtrl" %>


  <script type="text/javascript">

  var calculateDPP = function(dpp) {

      if (Ext.isEmpty(dpp)) {
          return;
      }

      var txPPN = Ext.getCmp('<%= txPPN.ClientID %>');
      var txTotal = Ext.getCmp('<%= txTotal.ClientID %>');

      var ppn = dpp * 0.1;

      txPPN.setValue(ppn);
      txTotal.setValue(dpp + ppn);
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="350" Width="500" Hidden="true"
  Maximizable="true" MinHeight="250" MinWidth="450" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfFMno" runat="server" />
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
                     <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
                      ValueField="c_nosup" Width="250" ItemSelector="tr.search-item" ListWidth="350"
                      MinChars="3" AllowBlank="false" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store4" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="14014" />
                            <ext:Parameter Name="parameters" Value="[['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_nama" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_nosup" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_nosup" />
                                <ext:RecordField Name="v_nama" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template2" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_nosup}</td><td>{v_nama}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="No. Pajak">
                      <Items>
                        <ext:TextField ID="txTaxNoHdr" runat="server" AllowBlank="true" FieldLabel="No. Pajak"
                          Width="150" MaxLength="20" />
                        <ext:DateField ID="txTaxDateHdr" runat="server" AllowBlank="false" FieldLabel="Tanggal Pajak"
                          Width="100" Format="dd-MM-yyyy" />
                      </Items>
                    </ext:CompositeField>
                    <ext:NumberField ID="txDPP" runat="server" AllowBlank="false" FieldLabel="DPP"
                        Width="150" AllowDecimals="true" AllowNegative="false"> 
                      <Listeners>
                        <Change Handler="calculateDPP(#{txDPP}.getValue());" />
                      </Listeners>
                    </ext:NumberField>
                    <ext:NumberField ID="txPPN" runat="server" AllowBlank="false" FieldLabel="PPN"
                        Width="150" AllowDecimals="true" AllowNegative="false" />
                    <ext:NumberField ID="txTotal" runat="server" AllowBlank="false" FieldLabel="Total"
                    Width="150" AllowDecimals="true" AllowNegative="false" />
                    <ext:TextArea ID="txKet" runat="server" AllowBlank="true" FieldLabel="Deskripsi"
                          Width="250" Height="100" MaxLength="100" />
                    <ext:TextField ID="txRef" runat="server" AllowBlank="true" FieldLabel="Referensi"
                          Width="150" MaxLength="50" />
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
            <ext:Parameter Name="NumberID" Value="#{hfFMno}.getValue()" Mode="Raw" />
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




