<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GudangCtrl.ascx.cs" Inherits="master_gudang_GudangCtrl" %>
<ext:Window ID="winDetail" runat="server" Height="250" Width="800" Hidden="true"
  Maximizable="true" MinHeight="250" MinWidth="750" Layout="Fit">
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
                    <ext:TextField ID="txDesc" runat="server" MaxLength="20" FieldLabel="Deskripsi" Width="150" />
                    <ext:TextField ID="txNama" runat="server" MaxLength="50" FieldLabel="Nama" Width="250" />
                    <ext:TextField ID="txAlamat" runat="server" MaxLength="50" FieldLabel="Alamat" Width="250" />
                    <ext:TextField ID="txRT" runat="server" MaxLength="50" FieldLabel="RT" Width="250" />
                    <ext:TextField ID="txRW" runat="server" MaxLength="50" FieldLabel="RW" Width="250" />
                    <ext:TextField ID="txLurah" runat="server" MaxLength="50" FieldLabel="Kelurahan" Width="250" />
                  </Items>
                </ext:Panel>
                <ext:Panel ID="pnlKanan" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <ext:TextField ID="txCamat" runat="server" MaxLength="50" FieldLabel="Kecamatan" Width="250" />
                    <ext:ComboBox ID="cbKota" runat="server" FieldLabel="Kota" DisplayField="v_desc"
                      ValueField="c_kota" ListWidth="400" AllowBlank="true" ForceSelection="false" Width="200"
                      ItemSelector="tr.search-item" PageSize="10" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store1" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2131" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_kota.Contains(@0) || @contains.v_desc.Contains(@0) || @contains.v_Province.Contains(@0) || @contains.v_Nationality.Contains(@0)', paramTextGetter(#{cbKota}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_desc" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_kota" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_kota" />
                                <ext:RecordField Name="v_desc" />
                                <ext:RecordField Name="v_Province" />
                                <ext:RecordField Name="v_Nationality" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template1" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Kota</td><td class="body-panel">Provinsi</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_kota}</td><td>{v_desc}</td><td>{v_Province}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                    </ext:ComboBox>
                    <ext:TextField ID="txKodePos" runat="server" MaxLength="5" FieldLabel="Kode Pos"
                      Width="50" />
                    <ext:TextField ID="txTelp" runat="server" MaxLength="11" FieldLabel="Telpon" />
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
