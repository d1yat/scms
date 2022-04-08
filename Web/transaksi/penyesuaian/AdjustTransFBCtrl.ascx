<%@ Control Language="C#" AutoEventWireup="true" 
CodeFile="AdjustTransFBCtrl.ascx.cs" Inherits="transaksi_penyesuaian_AdjustTransFBCtrl" %>

<script type="text/javascript">

  var prepareCommandsFB = function(rec, toolbar, valX) {
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

  var onChangeAdjFB = function(o, valu, hasil) {
    var store = o.getStore();

    if ((!Ext.isEmpty(hasil)) && (!Ext.isEmpty(store))) {
      var idx = store.findExact('c_noref', valu);
      if (r != -1) {
        var r = store.getAt(idx);

        hasil.setRawValue(r.get('n_sisa'));
      }
      else {
        hasil.reset();
      }
    }
  }

  var storeToDetailGridFB = function(frm, grid, cbNoRef, Val, keteranganDet) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(cbNoRef) ||
          Ext.isEmpty(Val) ||
          Ext.isEmpty(keteranganDet)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [cbNoRef.getValue()];
    var fieldX = ['c_noref'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var Ref = cbNoRef.getValue();
      var Value = Val.getValue();
      var Keter = keteranganDet.getValue();

      store.insert(0, new Ext.data.Record({
        'c_noref': Ref,
        'n_value': Value,
        'v_ket': Keter,
        'l_new': true
      }));

      resetEntryWhenChangeIntFB();

      /*
      item.reset();
      quantity.reset();
      nomorPo.reset();
      batch.reset();
      tglbatch.reset();
      */
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }
  var resetEntryWhenChangeIntFB = function(g) {
    var NoRefDtl = Ext.getCmp('<%= cbNoRefDtlFB.ClientID %>'),
      ValDet = Ext.getCmp('<%= txValueDtl.ClientID %>'),
      KetDtl = Ext.getCmp('<%= txKeteranganDtl.ClientID %>')

    if (!Ext.isEmpty(NoRefDtl)) {
      NoRefDtl.reset();
    }
    if (!Ext.isEmpty(ValDet)) {
      ValDet.reset();
    }
    if (!Ext.isEmpty(KetDtl)) {
      KetDtl.reset();
    }
    if (!Ext.isEmpty(g)) {
      g.getStore().removeAll();
    }
  }

  var voidInsertedDataFromStoreFB = function(rec) {
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
                  rec.set('v_ketDet', txt);
                }
              });
          }
        });
    }
  }

</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="1000" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfADJTransFB" runat="server" />
    <ext:Hidden ID="hfAdjTypeFB" runat="server" />
    <ext:Hidden ID="hfStoreIDFB" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="130" MaxHeight="130" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="130" Padding="10">
          <Items>
            <ext:ComboBox ID="cbTypeFB" runat="server" FieldLabel="Type" DisplayField="v_ket"
              ValueField="c_type" Width="150" PageSize="10" ListWidth="200" 
              MinChars="3" AllowBlank="false" ForceSelection="false" ItemSelector="tr.search-item" >
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2001" />
                    <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '57', ''],
                                              ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbTypeFB}), '']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="v_ket" />
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
              <Template ID="Template2" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 200px">
                      <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                      <tpl for=".">
                        <tr class="search-item">
                          <td>{c_type}</td><td>{v_ket}</td>
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
                <%--<Change Handler="clearRelatedComboRecursive(true, #{cbNoRefDtlFB});" />--%>
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbSuplierHdrFB" runat="server" DisplayField="v_nama" ValueField="c_nosup"
              Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
              AllowBlank="false" FieldLabel="Pemasok" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server">
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
                                                            ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplierHdrFB}), '']]" Mode="Raw" />
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
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 400px">
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbNoRefDtlFB});" />
              </Listeners>
            </ext:ComboBox>
            <ext:TextField ID="txKeteranganHdrFB" runat="server" FieldLabel="Keterangan" MaxLengthText="100"
              Width="400" />
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbNoRefDtlFB" runat="server" FieldLabel="No Faktur" DisplayField="c_noref"
                      ValueField="c_noref" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
                      MinChars="3" EmptyText="Pilihan..." AllowBlank="true" ForceSelection="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store runat="server" >
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0089" />
                            <ext:Parameter Name="parameters" Value="[['Type', #{cbTypeFB}.getValue(), 'System.String'],
                                                                     ['Supplier', #{cbSuplierHdrFB}.getValue(), 'System.String'],
                                                                     ['@contains.c_noref.Contains(@0)', paramTextGetter(#{cbNoRefDtlFB}), '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_noref" />
                            <ext:Parameter Name="dir" Value="Desc" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_noref" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_noref" />
                                <ext:RecordField Name="n_sisa"/>
                                <ext:RecordField Name="d_fbdate" DateFormat="M$" Type="Date"/>
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 400px">
                        <tr>
                        <td class="body-panel">Faktur</td>
                        <td class="body-panel">Tanggal</td>
                        <td class="body-panel">Value</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_noref}</td>
                        <td>{d_fbdate:this.formatDate}</td>
                        <td>{n_sisa}</td>
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
                        <Change Handler="onChangeAdjFB(this, newValue, #{txValueDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txValueDtl" runat="server" FieldLabel="Qty" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:TextField ID="txKeteranganDtl" runat="server" FieldLabel="Keterangan" MaxLengthText="100"
                      Width="400" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGridFB(#{frmpnlDetailEntry}, #{gridDetail}, #{cbNoRefDtlFB}, #{txValueDtl}, #{txKeteranganDtl});" />
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
                <ext:Store ID="Store3" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0090" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_adjno = @0', #{hfADJTransFB}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_noref" />
                        <ext:RecordField Name="n_value" />
                        <ext:RecordField Name="n_sisa" />
                        <ext:RecordField Name="v_ket" />
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
                    <PrepareToolbar Handler="prepareCommandsFB(record, toolbar, #{hfADJTransFB}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_noref" Header="No Referensi" Width="80" />
                  <ext:NumberColumn DataIndex="n_value" Header="Value" Width="50" Format="0.000,00/i" />
                  <%--<ext:NumberColumn DataIndex="n_sisa" Header="Sisa" Format="0.000,00/i" Width="250" />--%>
                  <ext:Column DataIndex="v_ket" Header="Keterangan" Width="200" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreFB(record); }" />
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
        <Click>
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfADJBatchNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfADJTransFB}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Type" Value="#{cbTypeFB}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TypeDesc" Value="#{cbTypeFB}.getText()" Mode="Raw" />
            <ext:Parameter Name="Supplier" Value="#{cbSuplierHdrFB}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SupplierDesc" Value="#{cbSuplierHdrFB}.getText()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdrFB}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreIDFB}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
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
