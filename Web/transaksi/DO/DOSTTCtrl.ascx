<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DOSTTCtrl.ascx.cs" 
Inherits="transaksi_DO_DOSTTCtrl" %>

<%@ Register Src="DOSTTCtrlPrint.ascx" TagName="DOSTTCtrlPrint" TagPrefix="uc1" %>

<script type="text/javascript">
  var prepareCommands = function(rec, toolbar, valX) {
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

  var voidInsertedDataFromStore = function(rec) {
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

<ext:Window ID="winDetil" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfPlNo" runat="server" />
    <ext:Hidden ID="hfDONo" runat="server" />
    <ext:Hidden ID="hfSTTNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfConfMode" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="195" Padding="10">
          <Items>
            <ext:ComboBox FieldLabel="Customer" runat="server" ID="txCustomerHeader" DisplayField="v_cunam" ValueField="c_cusno"
            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
            AllowBlank="false" ForceSelection="false">
              <Store>
                <ext:Store runat="server" ID="storeID" >
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy CallbackParam="soaScmsCallback" 
                    Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" 
                    Timeout="10000000" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2011" />
                    <ext:Parameter Name="parameters" 
                                   Value="[['l_hide = @0', false, 'System.Boolean'],
                                   ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{Customer}), '']]"
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
            
            <ext:ComboBox ID="cbGudang" runat="server" DisplayField="v_gdgdesc" FieldLabel="Gudang"
            ValueField="c_gdg" Width="300" AllowBlank="false" ForceSelection="false" TypeAhead="false">
              <Store>
                <ext:Store runat="server" RemotePaging="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudang}), '']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_gdg" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
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
              </Listeners>
            </ext:ComboBox>
            
            <ext:ComboBox ID="cbSTTSample" runat="server" FieldLabel="STT Sample" DisplayField="c_stno"
              ValueField="c_stno" Width="350" MinWidth="350" PageSize="10" AllowBlank="false" ForceSelection="false" 
              TypeAhead="false" ItemSelector="tr.search-item">
              <Store>
                <ext:Store runat="server" RemotePaging="false" SkinID="OriginalExtStore">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="6002" />
                    <ext:Parameter Name="parameters" Value="[['gdg', #{cbGudang}.getValue(), 'System.Char'],
                    ['@contains.c_stno.Contains(@0)|| @contains.c_mtno.Contains(@0)', paramTextGetter(#{cbSTTSample}), '']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_stno" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_stno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_stno" />
                        <ext:RecordField Name="c_mtno" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 400px">
                <tr><td class="body-panel">No STT</td><td class="body-panel">No Memo</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_stno}</td><td>{c_mtno}</td>
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
              <DirectEvents>
                <Change OnEvent="OnEvenAddGrid">
                  <ExtraParams>
                    <ext:Parameter Name="Parameter" Value="c_stno" />
                    <ext:Parameter Name="PrimaryID" Value="#{cbSTTSample}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Change>
              </DirectEvents>
            </ext:ComboBox>
            
            <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
              ValueField="c_type" Width="250" TypeAhead="false" AllowBlank="false" 
              ForceSelection="false">
              <Store>
                <ext:Store runat="server" RemotePaging="false" SkinID="OriginalExtStore">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2001" />
                    <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '02', ''],
                                              ['@contains.c_type.Contains(@0) || @contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaHdr}), '']]" Mode="Raw" />
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
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store runat="server" RemotePaging="false" RemoteSort="false" >
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
                    <%--<ext:Parameter Name="parameters" Value="[['c_dono = @0', #{hfDONo}.getValue(), 'System.String']]"
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
                      <ext:GridCommand CommandName="Delete" Icon="Delete" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfDONo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Quantity" Format="0.000,00/i" Width="75" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
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
        <Click >
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfDONo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button2" runat="server" Icon="Reload" Text="Bersihkan">
    </ext:Button>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>

<uc1:DOSTTCtrlPrint ID="DOSTTCtrlPrint" runat="server" />
