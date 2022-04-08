<%@ Control Language="C#" AutoEventWireup="true" 
CodeFile="AdjVoucherCreditCtrl.ascx.cs" 
Inherits="transaksi_penyesuaian_AdjVoucherCreditCtrl" %>

<script type="text/javascript">
  var onChangeAdjCR = function(o, valu, hasil) {
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

  var storeToDetailGridCR = function(frm, grid, NoRef, Tipe, Val, KetD) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(NoRef) ||
          Ext.isEmpty(Tipe) ||
          Ext.isEmpty(Val)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [NoRef.getValue()];
    var fieldX = ['c_noref'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var Value = Val.getValue();
      var KetDet = KetD.getValue();
      var TipeVal = Tipe.getValue();
      var NoRefVal = NoRef.getValue();

      var recNoRefVal = NoRef.findRecord(NoRef.valueField, NoRef.getValue());
      var ValVC = recNoRefVal.get('n_sisa');

      store.insert(0, new Ext.data.Record({
        'c_noref': NoRefVal,
        'c_type': TipeVal,
        'v_ketTran': Tipe.getText(),
        'n_value':  Value,
        'n_sisa': ValVC - Value,
        'v_ket': KetDet,
        'l_new': true
      }));

      resetEntryWhenChangeIntCR();

    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  var resetEntryWhenChangeIntCR = function(g) {
    var Tipe = Ext.getCmp('<%= cbTipeCR.ClientID %>'),
      NoRef = Ext.getCmp('<%= cbNoRefDtlCR.ClientID %>'),
      Val = Ext.getCmp('<%= txValueCR.ClientID %>'),
      KetD = Ext.getCmp('<%= txKetDetCR.ClientID %>')

    if (!Ext.isEmpty(Tipe)) {
      Tipe.reset();
    }
    if (!Ext.isEmpty(NoRef)) {
      NoRef.reset();
    }
    if (!Ext.isEmpty(Val)) {
      Val.reset();
    }
    if (!Ext.isEmpty(KetD)) {
      KetD.reset();
    }
    if (!Ext.isEmpty(g)) {
      g.getStore().removeAll();
    }
  }
  var prepareCommandsCR = function(rec, toolbar, valX) {
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

  var voidInsertedDataFromStoreCR = function(rec) {
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
                      rec.set('v_ketD', txt);
                    }
                  });
              }
            });
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="750" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfADJCredit" runat="server" />
    <ext:Hidden ID="hfStoreIDCR" runat="server" />
    <ext:Hidden ID="hfTypeCR" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
     <North MinHeight="80" MaxHeight="80" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="80" Padding="10">
          <Items>
            <ext:TextField runat="server" ID="txKeteranganHdrCR" Width="450" FieldLabel="Keterangan" />
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
                  <ext:ComboBox ID="cbNoRefDtlCR" runat="server" FieldLabel="No Voucher" DisplayField="c_noref"
                    ValueField="c_noref" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                    MinChars="3" EmptyText="Pilihan..." AllowBlank="true" ForceSelection="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="true" />
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
                          <ext:Parameter Name="model" Value="0122" />
                          <ext:Parameter Name="parameters" Value="[['@contains.c_noref.Contains(@0)', paramTextGetter(#{cbNoRefDtlCR}), '']]" Mode="Raw" />
                          <ext:Parameter Name="sort" Value="c_noref" />
                          <ext:Parameter Name="dir" Value="Desc" />
                        </BaseParams>
                        <Reader>
                          <ext:JsonReader IDProperty="c_noref" Root="d.records" SuccessProperty="d.success"
                            TotalProperty="d.totalRows">
                            <Fields>
                              <ext:RecordField Name="c_noref" />
                              <ext:RecordField Name="n_sisa"/>
                              <ext:RecordField Name="d_vcdate" DateFormat="M$" Type="Date"/>
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                      </ext:Store>
                    </Store>
                    <Template ID="Template3" runat="server">
                      <Html>
                      <table cellpading="0" cellspacing="0" style="width: 300px">
                      <tr>
                      <td class="body-panel">No Voucher</td>
                      <td class="body-panel">Tanggal</td>
                      <td class="body-panel">Nilai</td>
                      </tr>
                      <tpl for="."><tr class="search-item">
                      <td>{c_noref}</td>
                      <td>{d_vcdate:this.formatDate}</td>
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
                      <Change Handler="onChangeAdjCR(this, newValue, #{txValueCR});" />
                    </Listeners>
                  </ext:ComboBox>
                  <ext:ComboBox ID="cbTipeCR" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                    ValueField="c_type" Width="150" TypeAhead="false" AllowBlank="true" ForceSelection="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="true" />
                    </CustomConfig>
                    <Store>
                      <ext:Store runat="server" RemotePaging="false" SkinID="OriginalExtStore">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                          <ext:Parameter Name="allQuery" Value="true" />
                          <ext:Parameter Name="model" Value="2001" />
                          <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                    ['c_notrans = @0', '54', 'System.String'],
                                                    ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbTipeCR}), '']]" Mode="Raw" />
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
                    <Listeners>
                      <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                    </Listeners>
                  </ext:ComboBox>
                  <ext:NumberField ID="txValueCR" runat="server" FieldLabel="Value"
                   Width="150" Format="0.000,00/i" />
                  <ext:TextField runat="server" ID="txKetDetCR" Width="250" FieldLabel="Keterangan" />
                  <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                    Icon="Add">
                    <Listeners>
                      <Click Handler="storeToDetailGridCR(#{frmpnlDetailEntry}, #{gridDetail}, #{cbNoRefDtlCR}, #{cbTipeCR}, #{txValueCR}, #{txKetDetCR});" />
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
                <ext:Store ID="Store5" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0123" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_adjno = @0', #{hfADJCredit}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_noref" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="v_ketTran" />
                        <ext:RecordField Name="v_ket" />
                        <ext:RecordField Name="n_sisa" Type="Float"/>
                        <ext:RecordField Name="n_value" Type="Float" />
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
                    <PrepareToolbar Handler="prepareCommandsCR(record, toolbar, #{hfADJCredit}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_noref" Header="No Referensi" Width="80" />
                  <ext:Column DataIndex="c_type" Header="Tipe" Width="50" />
                  <ext:Column DataIndex="v_ketTran" Header="Desc" Width="150" />
                  <ext:NumberColumn DataIndex="n_value" Header="Nilai Adjust" Width="150" Format="0.000,00/i" />
                  <ext:NumberColumn DataIndex="n_sisa" Header="Sisa" Width="150" />
                  <ext:Column DataIndex="v_ket" Header="Keterangan" Width="250" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreCR(record); }" />
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
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfADJCredit}.getValue()" Mode="Raw" />
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
            <ext:Parameter Name="NumberID" Value="#{hfADJCredit}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdrCR}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreIDCR}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
    </ext:Button>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
           