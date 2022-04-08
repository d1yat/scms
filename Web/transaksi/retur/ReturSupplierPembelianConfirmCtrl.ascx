<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReturSupplierPembelianConfirmCtrl.ascx.cs"
  Inherits="transaksi_retur_ReturSupplierPembelianConfirmCtrl" %>
<%--
<script type="text/javascript">
  var storeToDetailGrid = function(frm, grid, item, batch, gqty, bqty, ket, kpr) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(gqty) ||
          Ext.isEmpty(bqty)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [item.getValue(), batch.getValue()];
    var fieldX = ['c_iteno', 'c_batch'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var storBatch = batch.getStore();
      var bat = batch.getValue().trim();
      var godqty = gqty.getValue();
      var badqty = bqty.getValue();
      var reqQty = 0;
      var itemNo = item.getValue().trim();
      var vket = ket.getValue().trim();
      var vkpr = kpr.getValue().trim();

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'c_batch': bat,
        'n_gqty': godqty,
        'n_bqty': badqty,
        'v_ket': vket,
        'c_cprno': vkpr,
        'l_new': true
      }));

      item.reset();
      gqty.reset();
      bqty.reset();
      batch.reset();
      ket.reset();
      kpr.reset();

    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  var selectedItemBatch = function(rec, target1, target2, cbBatDtl) {
    if (Ext.isEmpty(target1)) {
      ShowWarning("Objek target tidak terdefinisi.");
      return;
    }
    if (Ext.isEmpty(target2)) {
      ShowWarning("Objek target tidak terdefinisi.");
      return;
    }

    var recBat = cbBatDtl.findRecord(cbBatDtl.valueField, cbBatDtl.getValue());
    if (Ext.isEmpty(recBat)) {
      ShowWarning(String.format("Record Batch '{0}' tidak dapat di baca dari store.", cbBatDtl.getText()));
      return;
    }

    var gqty = recBat.get('N_GSISA');
    var bqty = recBat.get('N_BSISA');

    try {
      target1.setMinValue(0);
      target2.setMinValue(0);

      if (Ext.isNumber(gqty) || Ext.isNumber(bqty)) {
        target1.setMaxValue(gqty);
        target1.setValue(gqty);
        target2.setMaxValue(bqty);
        target2.setValue(bqty);
      }
      else {
        target1.setMaxValue(Number.MAX_VALUE);
        target1.setValue(gqty);
        target2.setMaxValue(Number.MAX_VALUE);
        target2.setValue(bqty);
      }
    }
    catch (e) {
      ShowError(e.toString());
    }
  }

  //  var prepareCommands = function(rec, toolbar, valX) {
  //    var del = toolbar.items.get(0); // delete button
  //    var vd = toolbar.items.get(1); // void button

  //    var isNew = false;

  //    if (!Ext.isEmpty(rec)) {
  //      isNew = rec.get('l_new');
  //    }

  //    if (Ext.isEmpty(valX) || isNew) {
  //      del.setVisible(true);
  //      vd.setVisible(false);
  //    }
  //    else {
  //      del.setVisible(false);
  //      vd.setVisible(true);
  //    }
  //  }

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
                  rec.set('ketDel', txt);
                }
              });
          }
        });
    }
  }
</script>
--%>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="750" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfTypeName" runat="server" />
    <ext:Hidden ID="hfRSNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="150" MaxHeight="150" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="150" Padding="10">
          <Items>
            <ext:Label ID="lbGudangHdr" runat="server" FieldLabel="Gudang" />
            <ext:Label ID="lbPrincipalHdr" runat="server" FieldLabel="Pemasok" />
            <ext:Label ID="lbCprNo" runat="server" FieldLabel="Ref. Pemasok" />
            <ext:Label ID="lbKeteranganHdr" runat="server" FieldLabel="Keterangan" />
          </Items>
        </ext:FormPanel>
      </North>
      <Center>
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
                    <ext:Parameter Name="model" Value="0137" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_rsno = @0', #{hfRSNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="NoRef" />
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="nReject" Type="Float" />
                        <ext:RecordField Name="nRework" Type="Float" />
                        <ext:RecordField Name="nRedress" Type="Float" />
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
                  <%--<ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfRSNo}.getValue());" />
                  </ext:CommandColumn>--%>
                  <ext:Column DataIndex="NoRef" Header="Ref RS." Width="100" />
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch " />
                  <ext:Column DataIndex="v_undes" Header="Desc " Width="100" />
                  <ext:NumberColumn DataIndex="nReject" Header="Rejected" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="nRework" Header="Reworked" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="nRedress" Header="Redressed" Format="0.000,00/i" Width="75" />
                  <%--<ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />--%>
                  <%--<ext:CheckColumn DataIndex="l_modified" Header="Modif" Width="50" />--%>
                </Columns>
              </ColumnModel>
              <%--<Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>--%>
            </ext:GridPanel>
          </Items>
          <Buttons>
            <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
              <DirectEvents>
                <Click OnEvent="Report_OnGenerate">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                    ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="GudangId" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfRSNo}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
            <%--<ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click">
                  <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfRSNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="GudangDesc" Value="#{cbGudangHdr}.getText()" Mode="Raw" />
                    <ext:Parameter Name="GudangId" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="PrinsipalDesc" Value="#{cbPrincipalHdr}.getText()" Mode="Raw" />
                    <ext:Parameter Name="PrinsipalID" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="TypeName" Value="#{hfTypeName}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>--%>
            <%--<ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
            </ext:Button>--%>
            <ext:Button runat="server" Icon="Cancel" Text="Keluar">
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
<ext:Window ID="wndDown" runat="server" Hidden="true" />
