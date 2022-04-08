<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TransferGudangAuto.ascx.cs" Inherits="transaksi_transfer_TransferGudangAuto" %>
<%@ Register Src="TransferGudangCtrl.ascx" TagName="TransferGudangCtrl" TagPrefix="uc" %>
<script type="text/javascript">

</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfConfMode" runat="server" />
    <ext:Hidden ID="hfSJNo" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North Collapsible="false" MinHeight="180" MaxHeight="180">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="200" Padding="10"
          ButtonAlign="Center">
          <Items>
            <ext:Label ID="lbGudangFrom" runat="server" FieldLabel="Asal" />
            <ext:ComboBox ID="cbToHdrAuto" runat="server" FieldLabel="Tujuan" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
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
                    <%--<ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />--%>
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0176" />
                    <ext:Parameter Name="parameters" Value="[['c_gdg != @0', #{hfGudang}.getValue(), 'System.Char']]"
                      Mode="Raw" />
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
                <table cellpading="0" cellspacing="0" style="width: 200px">
                      <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                      <tpl for=".">
                        <tr class="search-item">
                          <td>{c_gdg}</td><td>{v_gdgdesc}</td>
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbPrincipalHdr});" />
              </Listeners>
            </ext:ComboBox>
            
            <ext:TextField ID="txRnNoAuto" runat="server" FieldLabel="No RN" MaxLength="10"
              Width="100">
              <DirectEvents>
                <Change OnEvent="OnEvenAddGrid">
                  <ExtraParams>
                    <ext:Parameter Name="Parameter" Value="c_rnno" />
                    <ext:Parameter Name="PrimaryID" Value="#{txRnNoAuto}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Change>
              </DirectEvents>
            </ext:TextField>
            <ext:TextField ID="txKeteranganAuto" runat="server" FieldLabel="Keterangan" MaxLength="100"
              Width="250" />
            
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store6" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <%--<ext:Parameter Name="model" Value="0004" />--%>
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <%--<ext:Parameter Name="parameters" Value="[['c_sjno = @0', #{hfSJNo}.getValue(), 'System.String']]"
                      Mode="Raw" />--%>
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_spgno" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_adjust" />
                        <ext:RecordField Name="v_ket_type_dc" />
                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                        <ext:RecordField Name="n_booked" Type="Float" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
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
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommandsTGCtrl(record, toolbar, #{hfSJNo}.getValue(), #{hfConfMode}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_spgno" Header="SP Gudang" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />
                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Terpenuhi" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_adjust" Header="Adjustment" Format="0.000,00/i" Width="75" />
                  <ext:Column DataIndex="v_ket_type_dc" Header="Batal" Width="100">
                    <Editor>
                      <ext:ComboBox ID="cbTypeDcGrd" runat="server" DisplayField="v_ket" ValueField="c_type"
                        ForceSelection="false" MinChars="3" AllowBlank="true">
                        <CustomConfig>
                          <ext:ConfigItem Name="allowBlank" Value="true" />
                        </CustomConfig>
                        <Store>
                          <ext:Store ID="Store7" runat="server" RemotePaging="false">
                            <Proxy>
                              <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                CallbackParam="soaScmsCallback" />
                            </Proxy>
                            <BaseParams>
                              <ext:Parameter Name="allQuery" Value="true" />
                              <ext:Parameter Name="model" Value="2001" />
                              <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '60', '']]" Mode="Raw" />
                              <ext:Parameter Name="sort" Value="c_type" />
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
                      </ext:ComboBox>
                    </Editor>
                  </ext:Column>
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreTGCtrl(record); }" />
                <AfterEdit Handler="afterEditDataConfirm(e, #{cbTypeDcGrd});" />
                <BeforeEdit Handler="beforeEditDataConfirm(e);" />
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button runat="server" Icon="Disk" Text="Simpan" ID="btnSave">
      <DirectEvents>
        <Click OnEvent="btnSimpan_OnClick" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini."
            BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfSJNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangID" Value="#{hfGudang}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{hfGudangDesc}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangID2" Value="#{cbToHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc2" Value="#{cbToHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Supplier" Value="paramTextGetter(#{cbPrincipalHdr})" Mode="Raw" />
            <ext:Parameter Name="TypeCategory" Value="#{cbKategori}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button1" runat="server" Icon="Reload" Text="Bersihkan">
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
<uc:TransferGudangCtrl runat="server" ID="TransferGudangCtrl" />
<ext:Window ID="wndDown" runat="server" Hidden="true" />