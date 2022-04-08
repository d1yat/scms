<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReceiveNoteGudangCtrl.ascx.cs"
  Inherits="transaksi_penerimaan_ReceiveNoteGudangCtrl" %>

<script type="text/javascript">
  var deleteRecordOnGridE = function(grid, rec) {
    var store = grid.getStore();

    ShowConfirm('Hapus ?',
              "Apakah anda yakin ingin menghapus data ini ?",
              function(btn) {
                if (btn == 'yes') {
                  if ((!Ext.isEmpty(store)) && (!Ext.isEmpty(rec))) {
                    store.remove(rec);
                  }
                }
              });
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGudang" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="135" MaxHeight="135" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Height="135" Padding="10" ButtonAlign="Center"
          Unstyled="true">
          <Items>
            <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang Tujuan" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
              MinChars="3" AllowBlank="false">
              <Store>
                <ext:Store runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[['@contains.v_gdgdesc.Contains(@0) || @contains.c_gdg.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
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
              <Template runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 250px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                <tpl for="."><tr class="search-item">
                <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Listeners>
                <Change Handler="clearRelatedComboRecursive(true, #{cbNomorSJHdr});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbNomorSJHdr" runat="server" FieldLabel="Nomor Surat" DisplayField="c_sjno"
              ValueField="c_sjno" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
              MinChars="3" AllowBlank="false">
              <Store>
                <ext:Store runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="10051" />
                    <ext:Parameter Name="parameters" Value="[['gudang', paramValueGetter(#{cbGudangHdr}), 'System.Char'],
                      ['@contains.c_sjno.Contains(@0)', paramTextGetter(#{cbNomorSJHdr}), '']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="d_sjdate" />
                    <ext:Parameter Name="dir" Value="DESC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_sjno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_sjno" />
                        <ext:RecordField Name="d_sjdate" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="v_gdgdesc" />
                        <ext:RecordField Name="c_pin" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template runat="server">
                <Html>
                <table cellpading="0" cellspacing="5" style="width: 250">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Tanggal</td><td class="body-panel">Gudang Asal</td><td class="body-panel">PIN</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_sjno}</td><td>{d_sjdate:this.formatDate}</td><td>{v_gdgdesc}</td><td>{c_pin}</td>
                </tr></tpl>
                </table>
                </Html>
                <Functions>
                  <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                </Functions>
              </Template>
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                <Change Handler="#{txPinCodeHdr}.reset();" />
              </Listeners>
            </ext:ComboBox>
            <ext:TextField ID="txPinCodeHdr" runat="server" FieldLabel="P I N" MaxLength="20"
              Width="125" InputType="Password" AllowBlank="false" />
          </Items>
          <Buttons>
            <ext:Button runat="server" Text="Proses" Icon="CogStart">
              <%--<Listeners>
                <Click Handler="validasiProses(#{frmHeaders}, #{gridDetail});" />
              </Listeners>--%>
              <DirectEvents>
                <Click Before="return validasiProses(#{frmHeaders});" 
                  OnEvent="ProcessRNG_Click">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gudang" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="suratID" Value="#{cbNomorSJHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="pinCode" Value="#{txPinCodeHdr}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0043" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <%--<ext:Parameter Name="parameters" Value="[['divSuplier', #{cbDivPrincipal}.getValue(), 'System.String'],
                      ['via', #{cbViaHdr}.getValue(), 'System.String'],
                      ['typeItem', #{cbTipeProduk}.getValue(), 'System.String'],
                      ['suplier', #{cbSuplierHdr}.getValue(), 'System.String'],
                      ['tipeProcess', #{cbTipeHdr}.getValue(), 'System.String'],
                      ['gudang', #{cbGudangHdr}.getValue(), 'System.String'],
                      ['customer', #{cbCustomerHdr}.getValue(), 'System.String']]" Mode="Raw" />--%>
                    <%--<ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />--%>
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <%--<Fields>
                        <ext:RecordField Name="headerExist" Type="Boolean" />
                        <ext:RecordField Name="detailExist" Type="Boolean" />
                        <ext:RecordField Name="c_gdg" />
                        <ext:RecordField Name="v_gdgdesc" />
                        <ext:RecordField Name="c_refno" />
                        <ext:RecordField Name="d_ref" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="c_addtno" />
                        <ext:RecordField Name="d_addtdate" />
                        <ext:RecordField Name="v_ket" />
                        <ext:RecordField Name="c_nosup" />
                        <ext:RecordField Name="l_float" Type="Boolean" />
                        <ext:RecordField Name="n_bea" Type="Float" />
                        <ext:RecordField Name="l_print" Type="Boolean" />
                        <ext:RecordField Name="l_status" Type="Boolean" />
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_item_desc" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_acc" Type="Float" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="n_bqty" Type="Float" />
                      </Fields>--%>
                      <Fields>
                        <ext:RecordField Name="c_gdg" />
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_item_desc" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_bqty" Type="Float" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="c_refno" />
                        <ext:RecordField Name="c_addtno" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <%--<SortInfo Field="v_itnam" Direction="ASC" />--%>
                  <%--<Listeners>
                    <BeforeLoad Handler="Ext.net.Mask.show({ el: #{winDetail}.body,  msg: 'Kalkulasi data..' });" />
                    <Load Handler="Ext.net.Mask.hide();" />
                    <LoadException Handler="ShowError(response.toString());Ext.net.Mask.hide();" />
                    <Exception Handler="ShowError(response.toString());Ext.net.Mask.hide();" />
                  </Listeners>--%>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />--%>
                    </Commands>
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_item_desc" Header="Nama Barang" Width="200" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Baik" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_bqty" Header="Rusak" Format="0.000,00/i" Width="75" />
                  <ext:Column DataIndex="c_refno" Header="No. Ref." Hidden="true" />
                  <ext:Column DataIndex="c_addtno" Header="No. Tambahan" Hidden="true" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGridE(this, record); }" />
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
      <%--<South MinHeight="80" MaxHeight="80">
      </South>--%>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="btnSimpan" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders}, #{gridDetail})">
          <Confirmation Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." ConfirmRequest="true"
            BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"/>
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Gudang" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
            <%--<ext:Parameter Name="GudangName" Value="#{cbGudangHdr}.getText()" Mode="Raw" />--%>
            <ext:Parameter Name="SuratID" Value="#{cbNomorSJHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="PIN" Value="#{txPinCodeHdr}.getValue()" Mode="Raw" />
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
