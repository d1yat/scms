<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DO_STTCtrl.ascx.cs" Inherits="transaksi_penjualan_DO_STTCtrl" %>

<script type="text/javascript">
  var prepareCommandsDoStt = function(rec, toolbar, valX) {
    var del = toolbar.items.get(0); // delete button
    var vd = toolbar.items.get(1); // void button

    var isNew = false;

    if (!Ext.isEmpty(rec)) {
      isNew = rec.get('l_new');
    }

    if (Ext.isEmpty(valX) || isNew) {
      del.setVisible(true);
      vd.setVisible(false);
    }
    else {
      del.setVisible(false);
      vd.setVisible(true);
    }
  }

  var voidInsertedDataFromStoreDoStt = function(rec) {
    if (Ext.isEmpty(rec)) {
      return;
    }

    var isVoid = rec.get('l_void');

    if (isVoid) {
      ShowWarning('Item ini telah di batalkan.');
    }
    else {
      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
              function(btnP, txt) {
                if (btnP == 'ok') {
                  if (txt.trim().length < 1) {
                    txt = 'Kesalahan pemakai.';
                  }
                  rec.set('l_void', true);
                  rec.set('v_ket', txt);
                }
              });
          }
        });
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <%--<ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />--%>
    <ext:Hidden ID="hfDONoStt" runat="server" />
    <ext:Hidden ID="hfSTTNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfConfMode" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="195" Padding="10">
          <Items>
            <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
              MinChars="3">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Template ID="Template2" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 250px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                <tpl for="."><tr class="search-item">
                <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Store>
                <ext:Store runat="server" RemotePaging="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[['c_gdg != @0', '3', 'System.Char']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_gdg" />
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
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                <Select Handler="clearRelatedComboRecursive(this, #{cbSTTSampleHdr})" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox FieldLabel="Customer" runat="server" ID="cbCustomerHdr" DisplayField="v_cunam"
              ValueField="c_cusno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server" ID="storeID">
                  <Proxy>
                    <ext:ScriptTagProxy CallbackParam="soaScmsCallback" Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson"
                      Timeout="10000000" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2011" />
                    <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                          ['l_cabang = @0', true, 'System.Boolean'],
                          ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="v_cunam" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_cusno" />
                        <ext:RecordField Name="c_cab" />
                        <ext:RecordField Name="v_cunam" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template3" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 400px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
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
            <ext:ComboBox ID="cbSTTSampleHdr" runat="server" FieldLabel="STT Sample" DisplayField="c_stno"
              ValueField="c_stno" Width="150" PageSize="10" AllowBlank="false" ItemSelector="tr.search-item"
              ForceSelection="false" ListWidth="250">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server" RemotePaging="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="6002" />
                    <ext:Parameter Name="parameters" Value="[['gdg', #{cbGudangHdr}.getValue(), 'System.Char'],
                      ['@contains.c_stno.Contains(@0)', paramTextGetter(#{cbSTTSampleHdr}), '']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_stno" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_stno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_stno" />
                        <ext:RecordField Name="c_mtno" />
                        <ext:RecordField Name="d_stdate" Type="Date" DateFormat="M$" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 250px">
                <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_stno}</td><td>{d_stdate:this.formatDate}</td>
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
              </Listeners>
              <DirectEvents>
                <Change OnEvent="OnEvenAddGrid">
                  <ExtraParams>
                    <ext:Parameter Name="Parameter" Value="c_stno" />
                    <ext:Parameter Name="PrimaryID" Value="#{cbSTTSampleHdr}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Change>
              </DirectEvents>
            </ext:ComboBox>
            <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
              ValueField="c_type" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server" RemotePaging="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2001" />
                    <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '02', ''],
                                              ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaHdr}), '']]"
                      Mode="Raw" />
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
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
              </Listeners>
            </ext:ComboBox>
            <ext:TextField runat="server" ID="txKeterangan" FieldLabel="Keterangan" Width="300" />
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <LoadMask ShowMask="true" />
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
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <%--<ext:Parameter Name="parameters" Value="[['c_dono = @0', #{hfDONoStt}.getValue(), 'System.String']]"
                      Mode="Raw" />--%>
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="v_ket" />
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
                    <PrepareToolbar Handler="prepareCommandsDoStt(record, toolbar, #{hfDONoStt}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Quantity" Format="0.000,00/i" Width="75" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreDoStt(record); }" />
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfDONoStt}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfDONoStt}.getValue()" Mode="Raw" />
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
<ext:Window ID="wndDown" runat="server" Hidden="true" />
