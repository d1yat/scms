<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterPelangganCtrl.ascx.cs"
  Inherits="master_pelanggan_MasterPelangganCtrl" %>
<%--<script language="javascript" type="text/javascript">
  var onComboSectorClick = function(r, chk) {
    if (r.get('c_type') == '01') {
      if (!Ext.isEmpty(chk)) {
        chk.setValue(true);
        chk.disable();
      }
    }
  }
</script>--%>

<ext:Window ID="winDetail" runat="server" Height="400" Width="800" Hidden="true"
  Maximizable="true" MinHeight="350" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfNoCus" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:FormPanel ID="frmHeaders" runat="server" Layout="Fit" BodyBorder="false" Frame="false">
      <Items>
        <ext:TabPanel runat="server" ActiveTabIndex="0" Frame="false" Border="false" DeferredRender="false">
          <Items>
            <ext:Panel ID="pnlHeaders1" runat="server" Title="Umum" Height="195" Padding="10"
              Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form"
                  LabelAlign="Left">
                  <Items>
                    <ext:TextField runat="server" FieldLabel="Nama" ID="txName" AllowBlank="false" Width="200"/>
                    <ext:TextField runat="server" FieldLabel="Pemilik" ID="txPemilik" />
                    <ext:TextArea runat="server" FieldLabel="Alamat 1" ID="txAddr1" AllowBlank="false" />
                    <ext:TextArea runat="server" FieldLabel="Alamat 2" ID="txAddr2" />
                    <ext:TextField runat="server" FieldLabel="Kode Pos" ID="txPos" AllowBlank="false" />
                  </Items>
                </ext:Panel>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form"
                  LabelAlign="Left">
                  <Items>
                    <ext:TextField runat="server" FieldLabel="Kota" ID="txKota" AllowBlank="false" />
                    <ext:TextField runat="server" FieldLabel="Telp 1" ID="txTelp1" AllowBlank="false" />
                    <ext:TextField runat="server" FieldLabel="Telp 2" ID="txTelp2" />
                    <ext:TextField runat="server" FieldLabel="Fax" ID="txFax" />
                    <ext:TextField runat="server" FieldLabel="Account" ID="txAccount" />
                    <ext:TextField runat="server" FieldLabel="Nama Bank" ID="txNmBank" />
                    <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
                      ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="200"
                      MinChars="3" AllowBlank="false">
                      <Store>
                        <ext:Store ID="Store1" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2031" />
                            <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_gdgdesc" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_gdg" />
                                <ext:RecordField Name="v_gdgdesc" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template1" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 200px">
                      <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                      <tpl for="."><tr class="search-item">
                      <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                      </tr></tpl>
                      </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                    </ext:ComboBox>                  
                    <ext:ComboBox ID="cbSektor" runat="server" FieldLabel="Sektor" DisplayField="v_ket"
                      ValueField="c_type" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="200"
                      MinChars="3" AllowBlank="true">
                      <Store>
                        <ext:Store ID="Store2" runat="server" >
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                    ['c_notrans = @0', '59', '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_type" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
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
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                    </ext:ComboBox>
                    <ext:TextField runat="server" FieldLabel="Lead Time Internal" ID="txDaysInt" AllowBlank="true" />                    
                    <ext:TextField runat="server" FieldLabel="Lead Time Ekspedisi" ID="txDaysEksp" AllowBlank="true" />                    
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
            <ext:Panel ID="pnlHeaders2" runat="server" Title="Pajak" Height="195" Padding="10"
              Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form"
                  LabelAlign="Left">
                  <Items>
                    <ext:TextField runat="server" FieldLabel="Nama Tax" ID="txTaxName" AllowBlank="false" Width="200" />
                    <ext:TextArea runat="server" FieldLabel="Alamat Tax 1" ID="txAddrTax1" AllowBlank="false" />
                    <ext:TextArea runat="server" FieldLabel="Alamat Tax 2" ID="txAddrTax2" />
                    <ext:TextField runat="server" FieldLabel="Kota Tax" ID="txKotaTax" AllowBlank="false" />
                    <ext:DateField runat="server" FieldLabel="Tgl Buka" ID="dtOpen" AllowBlank="true"
                      Width="125" Format="dd-MM-yyyy"/>
                  </Items>
                </ext:Panel>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form"
                  LabelAlign="Left">
                  <Items>
                    <ext:TextField runat="server" FieldLabel="Kode Pos Tax" ID="txPosTax" AllowBlank="false" />
                    <ext:TextField runat="server" FieldLabel="No NPWP" ID="txNPWP" />
                    <ext:TextField runat="server" FieldLabel="No NPKP" ID="txNPKP" />
                    <ext:DateField runat="server" FieldLabel="Tgl NPKP" ID="dtNPKP" AllowBlank="false"
                      Width="125" Format="dd-MM-yyyy" />
                    <ext:DateField runat="server" FieldLabel="Tgl Tutup" ID="dtClose" AllowBlank="true"
                      Width="125" Format="dd-MM-yyyy" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
            <ext:Panel ID="pnlHeaders3" runat="server" Title="Finance" Height="195" Padding="10"
              Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form"
                  LabelAlign="Left">
                  <Items>
                    <ext:TextField runat="server" FieldLabel="Tagih" ID="txTagih" />
                    <ext:NumberField runat="server" FieldLabel="Fee" ID="txFee" AllowBlank="false" />
                    <ext:NumberField runat="server" FieldLabel="Kredit Limit" ID="txLimit" AllowBlank="false" />
                    <ext:NumberField runat="server" FieldLabel="Top" ID="txTOP" AllowBlank="false" />
                    <ext:NumberField runat="server" FieldLabel="Top Panjang" ID="txTOPPjg" AllowBlank="false" />
                    <ext:Checkbox runat="server" FieldLabel="Askes" ID="chkAskes" />
                    <ext:Checkbox runat="server" FieldLabel="Materai" ID="chkMaterai" />
                  </Items>
                </ext:Panel>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form"
                  LabelAlign="Left">
                  <Items>
                    <ext:Checkbox runat="server" FieldLabel="Dispen" ID="chkDispen" />
                    <ext:Checkbox runat="server" FieldLabel="Status" ID="chkStatus" />
                    <%--<ext:Checkbox runat="server" FieldLabel="Cabang" ID="chkCabang" />--%>
                    <ext:Checkbox runat="server" FieldLabel="Hide" ID="chkHide" />
                    <ext:TextField runat="server" FieldLabel="Kode Cabang" ID="txKodeCab" AllowBlank="false"
                      Width="50" MaxLength="2" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
        </ext:TabPanel>
      </Items>
    </ext:FormPanel>
  </Items>
  <Items>
    <ext:Label runat="server" ID="lblstatus" Text=""></ext:Label>
  </Items>
  <Buttons>
    <ext:Button runat="server" ID="btnApprove" Icon="Disk" Text="Approve">
      <DirectEvents>
        <Click OnEvent="ApproveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
            Title="Simpan ?" Message="Anda yakin ingin Approve data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoCus}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" ID="btnCancel" Icon="Disk" Text="Reject">
      <DirectEvents>
        <Click OnEvent="CancelBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
            Title="Simpan ?" Message="Anda yakin ingin Reject data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoCus}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
            Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoCus}.getValue()" Mode="Raw" />
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
