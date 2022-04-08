<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WaktuPelayananCtrl.ascx.cs" 
Inherits="transaksi_wp_WaktuPelayananCtrl" %>

<ext:Window ID="winDetail" runat="server" Height="200" Width="400" Hidden="true"
  Maximizable="true" MinHeight="200" MinWidth="400" Layout="Fit">
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="125" MaxHeight="125" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Penyerah" Height="75" Padding="10" Width="400">
          <Items>
            <ext:ComboBox ID="cbPenyerah" runat="server" FieldLabel="Penyerah" DisplayField="v_nama"
              ValueField="c_nip" Width="250" PageSize="10" ListWidth="250" ItemSelector="tr.search-item"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store1" runat="server" RemotePaging="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="10" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2121" />
                    <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                    ['@contains.v_nama.Contains(@0) || @contains.c_nip.Contains(@0)', paramTextGetter(#{cbKryHdr}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="v_nama" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_nip" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_nip" />
                        <ext:RecordField Name="v_nama" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template2" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 250px">
                <tr><td class="body-panel">NIP</td><td class="body-panel">Nama</td></tr>
                <tpl for=".">
                  <tr class="search-item">
                    <td>{c_nip}</td><td>{v_nama}</td>
                  </tr>
                </tpl>
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
          </Items>
        </ext:FormPanel>  
      </North>
      <Center MinHeight="150">
        <ext:FormPanel ID="FormPanel1" runat="server" Title="Penerima" Height="75" Padding="10" Width="400">
          <Items>
            <ext:ComboBox ID="cbPenerima" runat="server" FieldLabel="Penerima" DisplayField="v_nama"
              ValueField="c_nip" Width="250" PageSize="10" ListWidth="250" ItemSelector="tr.search-item"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store2" runat="server" RemotePaging="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="10" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2121" />
                    <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                    ['@contains.v_nama.Contains(@0) || @contains.c_nip.Contains(@0)', paramTextGetter(#{cbKryHdr}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="v_nama" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_nip" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_nip" />
                        <ext:RecordField Name="v_nama" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 250px">
                <tr><td class="body-panel">NIP</td><td class="body-panel">Nama</td></tr>
                <tpl for=".">
                  <tr class="search-item">
                    <td>{c_nip}</td><td>{v_nama}</td>
                  </tr>
                </tpl>
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
          </Items>
        </ext:FormPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="btnSimpan" runat="server" ToolTip="Simpan" Icon="Disk">
      <DirectEvents>
        <Click OnEvent="btnSimpan_Click">
            <ExtraParams>
                <ext:Parameter Name="Penyerah" Value="#{cbPenyerah}.getValue()" Mode="Raw" />
                <ext:Parameter Name="Penerima" Value="#{cbPenerima}.getValue()" Mode="Raw" />
            </ExtraParams>
            <EventMask ShowMask="true" />
        </Click>
    </DirectEvents>
    </ext:Button>
  </Buttons>
</ext:Window>