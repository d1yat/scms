<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ClaimBonusProcessCtrl.ascx.cs" 
Inherits="transaksi_bonus_ClaimBonusProcessCtrl" %>

<script language="javascript">
  
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfClaimNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
   </Content>
   <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
        <North MinHeight="120" MaxHeight="120" Collapsible="false">
          <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="180" Padding="10">
            <Items>
              <%--<ext:NumberField runat="server" ID="txYearHdr" MaxLength="4" FieldLabel="Year" Width="100" />
              <ext:ComboBox ID="cbMonthHdr" runat="server" FieldLabel="Bulan" Width="150">
                <Items>
                  <ext:ListItem Text="January" Value="01" />
                  <ext:ListItem Text="February" Value="02" />
                  <ext:ListItem Text="Meret" Value="03" />
                  <ext:ListItem Text="April" Value="04" />
                  <ext:ListItem Text="Mei" Value="05" />
                  <ext:ListItem Text="Juni" Value="06" />
                  <ext:ListItem Text="juli" Value="07" />
                  <ext:ListItem Text="Agustus" Value="08" />
                  <ext:ListItem Text="September" Value="09" />
                  <ext:ListItem Text="Oktober" Value="10" />
                  <ext:ListItem Text="Nopember" Value="11" />
                  <ext:ListItem Text="Desember" Value="12" />
                </Items>
              </ext:ComboBox>--%>
              
              <ext:CompositeField runat="server" FieldLabel="Periode">
                <Items>
                  <ext:SelectBox ID="txYearHdr" runat="server" Width="75" AllowBlank="false" FieldLabel="Tahun" />
                  <ext:SelectBox ID="cbMonthHdr" runat="server" AllowBlank="false" FieldLabel="Bulan"/>
                </Items>
              </ext:CompositeField>
              
              <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <Store>
                <ext:Store runat="server" >
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2021" />
                    <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                ['l_hide = @0', false, 'System.Boolean'],
                                ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]" Mode="Raw" />
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
              <Template ID="Template6" runat="server">
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbDivPrinsipal});" />
              </Listeners>
            </ext:ComboBox>
            
              <ext:ComboBox ID="cbDivPrinsipalHdr" runat="server" FieldLabel="Divisi Pemasok" DisplayField="v_nmdivpri"
              ValueField="c_kddivpri" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="true">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="true" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store1" runat="server" >
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2051" />
                    <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                ['l_hide = @0', false, 'System.Boolean'],
                                ['c_nosup = @0', #{cbPrincipalHdr}.getValue(), 'System.String']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="v_nmdivpri" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_kddivpri" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_kddivpri" />
                        <ext:RecordField Name="v_nmdivpri" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 350px">
                  <tr><td class="body-panel">Kode</td><td class="body-panel">Divisi Pemasok</td></tr>
                  <tpl for="."><tr class="search-item">
                      <td>{c_kddivpri}</td><td>{v_nmdivpri}</td>
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
            
              <ext:TextField runat="server" ID="txKeteranganHdr" Width="200" FieldLabel="Keterangan"></ext:TextField>
            
            </Items>
            <Buttons>
            <ext:Button ID="Button1" runat="server" Text="Proses" Icon="CogStart">
              <DirectEvents>
                <Click Before="return validasiProses(#{frmHeaders});" OnEvent="ProcessORP_Click">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="Tahun" Value="#{txYearHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Bulan" Value="#{cbMonthHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Supplier" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="divSupplier" Value="#{cbDivPrinsipalHdr}.getValue()" Mode="Raw" />                    
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
          </ext:FormPanel>
        </North>
        <Center>
          <ext:Panel ID="pnlGridDetail" runat="server" Title="List Claim" Height="150" Layout="Fit">
            <TopBar>
              <ext:Toolbar runat="server">
                <Items>
                  <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                    LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  </ext:FormPanel>
                </Items>
              </ext:Toolbar>
            </TopBar>
            <Items>
              <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store2" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0069" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[[]]" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_nosup" />
                        <ext:RecordField Name="v_nama" />
                        <ext:RecordField Name="v_cunam" />
                        <ext:RecordField Name="c_cusno" />
                        <ext:RecordField Name="v_cunam" />
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="n_salpri" Type="Float" />
                        <ext:RecordField Name="qty" Type="Float" />
                        <ext:RecordField Name="gret" Type="Float" />
                        <ext:RecordField Name="bret" Type="Float" />
                        <ext:RecordField Name="neto" Type="Float" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                    </Commands>
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_cusno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_cunam" Header="Nama Cabang" Width="200" />
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="200" />
                  <ext:NumberColumn DataIndex="n_salpri" Header="Harga" Format="0.000,00/i" Width="75"/>
                  <ext:NumberColumn DataIndex="qty" Header="Sales" Format="0.000,00/i" Width="75" >
                    <%--<Editor>
                      <ext:NumberField runat="server" DataIndex="qty" Header="Sales" Format="0.000,00/i" Width="75" />
                    </Editor>--%>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="gret" Header="Good Retur" Format="0.000,00/i" Width="75">
                    <%--<Editor>
                      <ext:NumberField  runat="server" DataIndex="gret" Header="Sales" Format="0.000,00/i" Width="75" />
                    </Editor>--%>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="bret" Header="Bad Retur" Format="0.000,00/i" Width="75" >
                    <%--<Editor>
                      <ext:NumberField runat="server" DataIndex="bret" Header="Bad Retur" Format="0.000,00/i" Width="75" />
                    </Editor>--%>
                 </ext:NumberColumn>
                 <ext:NumberColumn DataIndex="neto" Header="Bersih" Format="0.000,00/i" Width="75" >
                    <%--<Editor>
                      <ext:NumberField runat="server" DataIndex="bret" Header="Bad Retur" Format="0.000,00/i" Width="75" />
                    </Editor>--%>
                 </ext:NumberColumn>
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGridE(this, record); }" />
              </Listeners>
            </ext:GridPanel>
            </Items>
            <Buttons>
            <ext:Button runat="server" Icon="Disk" Text="Simpan">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click">
                  <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfClaimNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Year" Value="#{txYearHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Month" Value="#{cbMonthHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Prinsipal" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="SuplierDesc" Value="#{cbPrincipalHdr}.getText()" Mode="Raw" />
                    <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:Button ID="Button3" runat="server" Icon="Reload" Text="Bersihkan">
              <DirectEvents>
                <Click OnEvent="ReloadBtn_Click">
                  <EventMask ShowMask="true" />
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:Button ID="Button4" runat="server" Icon="Cancel" Text="Keluar">
              <Listeners>
                <Click Handler="#{winDetail}.hide();" />
              </Listeners>
            </ext:Button>
          </Buttons>
          </ext:Panel>
        </Center>
      </ext:BorderLayout>
   </Items>
 </ext:Window>