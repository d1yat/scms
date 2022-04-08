<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterEstimasiExpedisiCtrl.ascx.cs"
  Inherits="master_expedisi_MasterEstimasiExpedisiCtrl" %>

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



  var storeToDetailGrid = function(frm, grid, custom, udara, darat, ice, impor) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(custom) ||
          Ext.isEmpty(udara.getValue()) ||
          Ext.isEmpty(darat.getValue()) ||
          Ext.isEmpty(impor.getValue())) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    if ((udara.getValue() == 0) &&
    (darat.getValue() == 0) &&
    (impor.getValue() == 0) && (ice.getValue() == 0)) {
      ShowWarning("Waktu estimasi tidak boleh 0 semua.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [custom.getValue()];
    var fieldX = ['c_cusno'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var tudara = udara.getValue();
      var tdarat = darat.getValue();
      var timpor = impor.getValue();
      var tice = ice.getValue();
      var cusno = custom.getValue();


      store.insert(0, new Ext.data.Record({
        'c_cusno': cusno,
        'v_cunam': custom.getText(),
        't_udara': tudara,
        't_daratlaut': tdarat,
        't_icepack': tice,
        't_import': timpor,
        'l_new': true
      }));

      custom.reset();
      udara.reset();
      darat.reset();
      impor.reset();
      ice.reset();

    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  var afterEdit = function(e) {
    if (!e.record.get('l_new')) {
      e.record.set('l_modified', true);
    }
  };

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

  var fillDefaultEntry = function(t1, t2, t3, t4) {
    if (!Ext.isEmpty(t1)) {
      t1.setValue(0);
    }
    if (!Ext.isEmpty(t2)) {
      t2.setValue(0);
    }
    if (!Ext.isEmpty(t3)) {
      t3.setValue(0);
    }
    if (!Ext.isEmpty(t4)) {
      t4.setValue(0);
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="400" Width="750" Hidden="true"
  Maximizable="true" MinHeight="400" MinWidth="700" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfNoExpEst" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="50" MaxHeight="75" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="75" Padding="10">
          <Items>
            <ext:ComboBox ID="cbEks" runat="server" FieldLabel="Ekspedisi" DisplayField="v_ket"
              ValueField="c_exp" Width="250" ItemSelector="tr.search-item" MinChars="3" PageSize="10"
              ListWidth="300" AllowBlank="false" ForceSelection="false">
              <DirectEvents>
                <Change OnEvent="cbeks_OnChange">
                  <EventMask ShowMask="true" />
                </Change>
              </DirectEvents>
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store3" runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <AutoLoadParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={20}" />
                  </AutoLoadParams>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2081" />
                    <ext:Parameter Name="parameters" Value="[['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbEks}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="v_ket" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_exp" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_exp" />
                        <ext:RecordField Name="v_ket" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 400px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_exp}</td><td>{v_ket}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                <Change Handler="clearRelatedComboRecursive(true, #{cbCustomer});" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Cabang" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbCustomer" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                      ValueField="c_cusno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                      MinChars="3" AllowBlank="false" ForceSelection="false">
                      <%--<DirectEvents>
                        <Change OnEvent="cbCustom_Change">
                          <EventMask ShowMask="true" />
                        </Change>
                      </DirectEvents>--%>
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store6" runat="server" SkinID="OriginalExtStore">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="10" />
                            <ext:Parameter Name="model" Value="2011" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), ''],
                                                                    ['exp', #{cbEks}.getValue(), 'System.String']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_cunam" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_cusno" />
                                <ext:RecordField Name="v_cunam" />
                                <ext:RecordField Name="c_cab" />
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
                        <Change Handler="fillDefaultEntry(#{txUdara}, #{txDarat}, #{txIcePack}, #{txImport});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField runat="server" ID="txUdara" FieldLabel="Udara" Width="80" />
                    <ext:NumberField runat="server" ID="txDarat" FieldLabel="Darat/ Laut" Width="80" />
                    <ext:NumberField runat="server" ID="txIcePack" FieldLabel="Ice Pack" Width="80" />
                    <ext:NumberField runat="server" ID="txImport" FieldLabel="Import" Width="80" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbCustomer}, #{txUdara}, #{txDarat}, #{txIcePack}, #{txImport});" />
                      </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                      Icon="Cancel">
                      <Listeners>
                        <Click Handler="#{frmpnlDetailEntry}.getForm().reset()" />
                      </Listeners>
                    </ext:Button>
                  </Items>
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
                <ext:Store runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0146" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_exp = @0', #{hfNoExpEst}.getValue(), 'System.String'],
                      ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_cusno" />
                        <ext:RecordField Name="v_cunam" />
                        <ext:RecordField Name="t_udara" Type="Float" />
                        <ext:RecordField Name="t_daratlaut" Type="Float" />
                        <ext:RecordField Name="t_icepack" Type="Float" />
                        <ext:RecordField Name="t_import" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
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
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfNoExpEst}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_cusno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_cunam" Header="Nama Barang" Width="250" />
                  <ext:NumberColumn DataIndex="t_udara" Header="Udara" Format="0.000,00/i" Width="75"
                    Editable="true">
                    <Editor>
                      <ext:NumberField runat="server" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="t_daratlaut" Header="Darat / Laut" Format="0.000,00/i"
                    Width="75" Editable="true">
                    <Editor>
                      <ext:NumberField ID="NumberField1" runat="server" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="t_icepack" Header="Ice Pack" Format="0.000,00/i" Width="75"
                    Editable="true">
                    <Editor>
                      <ext:NumberField ID="NumberField3" runat="server" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="t_import" Header="Import" Format="0.000,00/i" Width="75"
                    Editable="true">
                    <Editor>
                      <ext:NumberField ID="NumberField2" runat="server" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                <AfterEdit Fn="afterEdit" />
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
            Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfNoExpEst}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
            <ext:Parameter Name="expId" Value="#{cbEks}.getValue()" Mode="Raw" />
            <ext:Parameter Name="expDesc" Value="#{cbEks}.getText()" Mode="Raw" />
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
    <ext:Button ID="Button2" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
