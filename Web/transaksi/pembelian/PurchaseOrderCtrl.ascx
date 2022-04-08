<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PurchaseOrderCtrl.ascx.cs"
  Inherits="transaksi_pembelian_PurchaseOrderCtrl" %>

<script type="text/javascript">
  var validateRowsEditing = function(e) {
    if (Ext.isEmpty(e)) {
      return;
    }

    if (e.record.get('isProcess')) {
      e.cancel = true;
    }
  }
  var prepareCommands = function(rec, toolbar) {
    var vd = toolbar.items.get(0); // void button

    if (rec.get('isProcess')) {
      vd.setVisible(false);
    }
    else {
      vd.setVisible(true);
    }
  }
  var recalculateTotalPO = function(e, lblBruto, lblDisc, txExDisc, lblExDisc, lblPpn, lblNet) {
    if ((Ext.isEmpty(e)) && ((e.field != 'n_disc') && (e.field != 'n_salpri') && (e.field != 'customRecall'))) {
      return;
    }

    var allGrosGrid = 0,
      allNetGrid = 0,
      qty = 0,
      harga = 0,
      disc = 0,
      gross = 0,
      jml_net = 0;
    var store = e.grid.getStore();
    var r = '';
    var xdisc = 0;

    if (!Ext.isEmpty(txExDisc)) {
      xdisc = txExDisc.getValue();
    }

    if ((e.field == 'n_disc') || (e.field == 'n_salpri')) {
      e.record.set('l_modified', true);
    }

    for (n = 0, nTot = store.getCount(); n < nTot; n++) {
      r = store.getAt(n);
      if (!r.get('l_void')) {
        qty = r.get('n_qty');
        harga = r.get('n_salpri');
        disc = r.get('n_disc');

        gross = (qty * harga);
        jml_net = gross - (gross * (disc / 100));

        r.set('n_total_gross', gross);
        r.set('n_total_net', jml_net);

        allGrosGrid += gross;
        allNetGrid += jml_net;
      }
    }

    if (!Ext.isEmpty(lblBruto)) {
      lblBruto.setText(myFormatNumber(allGrosGrid));
    }

    disc = (allGrosGrid - allNetGrid);

    if (!Ext.isEmpty(lblDisc)) {
      lblDisc.setText(myFormatNumber(disc));
    }

    harga = (allNetGrid * (xdisc / 100));

    if (!Ext.isEmpty(lblExDisc)) {
      lblExDisc.setText(myFormatNumber(harga));
    }

    gross = ((allNetGrid - harga) * 0.1);

    if (!Ext.isEmpty(lblPpn)) {
      lblPpn.setText(myFormatNumber(gross));
    }

    jml_net = ((allNetGrid - harga) + gross);

    if (!Ext.isEmpty(lblNet)) {
      lblNet.setText(myFormatNumber(jml_net));
    }
  }
  var reCallRecalculateTotalPO = function(grid, lblBruto, lblDisc, txExDisc, lblExDisc, lblPpn, lblNet) {
    var e = {
      grid: grid,
      field: 'customRecall'
    };

    recalculateTotalPO(e, lblBruto, lblDisc, txExDisc, lblExDisc, lblPpn, lblNet);
  }
  var voidDataFromStore = function(rec) {
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

                  rec.reject();
                  rec.set('l_void', true);
                  rec.set('v_ket', txt);
                }
              });
          }
        });
    }
  }

  var kursValueSelected = function(r, tx) {
    if (!Ext.isEmpty(r)) {
      if (!Ext.isEmpty(tx)) {
        tx.setValue(r.get('n_currency'));
      }
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="725" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfPoNo" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="150" MaxHeight="150" Collapsible="false">
        <ext:Panel runat="server" Title="Header" Height="150" Padding="10" Layout="Column">
          <Items>
            <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
              <Items>
                <ext:Label ID="lbGudangHdr" runat="server" FieldLabel="Gudang" />
                <ext:Label ID="lbSuplierHdr" runat="server" FieldLabel="Pemasok" />
                <ext:TextField ID="txKeteranganHdr" runat="server" FieldLabel="Keterangan" MaxLength="100"
                  Width="235" />
                <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Kurs">
                  <Items>
                    <ext:ComboBox ID="cbKursHdr" runat="server" DisplayField="v_desc" ValueField="c_kurs"
                      Width="165" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                      AllowBlank="false">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2072" />
                            <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_desc" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_desc" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_desc" />
                                <ext:RecordField Name="c_kurs" />
                                <ext:RecordField Name="n_currency" />
                                <ext:RecordField Name="v_desc" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td>
                        <td class="body-panel">Nama</td><td class="body-panel">Kurs</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_kurs}</td><td>{c_desc}</td><td>{v_desc}</td><td>{n_currency}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Select Handler="kursValueSelected(record, #{txKursValue});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txKursValue" runat="server" AllowBlank="false" AllowDecimals="true"
                      AllowNegative="false" DecimalPrecision="2" Width="65" />
                  </Items>
                </ext:CompositeField>
              </Items>
            </ext:Panel>
            <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
              <Items>
                <ext:Label ID="lbNomorORHdr" runat="server" FieldLabel="Nomor OR" />
                <ext:Label ID="lbTglPOHdr" runat="server" FieldLabel="Tanggal PO" />
                <ext:Label ID="lbImport" runat="server" FieldLabel="Import" />
              </Items>
            </ext:Panel>
          </Items>
        </ext:Panel>
      </North>
      <Center MinHeight="150">
        <ext:Panel runat="server" Layout="Fit" Title="Daftar Items">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server" ClicksToEdit="1">
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
                    <ext:Parameter Name="model" Value="0018" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['gudang', '1', 'System.Char'],
                      ['pono', #{hfPoNo}.getValue(), 'System.String']]" Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_iteno" TotalProperty="d.totalRows" Root="d.records"
                      SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="n_disc" Type="Float" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="n_salpri" Type="Float" />
                        <ext:RecordField Name="n_sisa" Type="Float" />
                        <ext:RecordField Name="n_total_gross" Type="Float" />
                        <ext:RecordField Name="n_total_net" Type="Float" />
                        <ext:RecordField Name="isProcess" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="v_ket" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <SortInfo Field="v_itemdesc" Direction="DESC" />
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Jumlah" Format="0.000,00/i" Width="55" />
                  <ext:NumberColumn DataIndex="n_disc" Header="Pot. (%)" Format="0.000,00/i" Width="50">
                    <Editor>
                      <ext:NumberField runat="server" AllowDecimals="true" AllowNegative="false" MinValue="0"
                        MaxValue="100" DecimalPrecision="2" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_salpri" Header="Harga" Format="0.000,00/i" Width="80">
                    <Editor>
                      <ext:NumberField runat="server" AllowDecimals="true" AllowNegative="false" MinValue="0"
                        DecimalPrecision="2" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_total_gross" Header="Total" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_total_net" Header="Total (Net)" Format="0.000,00/i"
                    Width="75" />
                  <ext:Column DataIndex="v_ket" Header="Keterangan" Width="120">
                    <Editor>
                      <ext:TextField runat="server" />
                    </Editor>
                  </ext:Column>
                </Columns>
              </ColumnModel>
              <Listeners>
                <BeforeEdit Handler="validateRowsEditing(e)" />
                <AfterEdit Handler="recalculateTotalPO(e, #{lbBrutoBtm}, #{lbDiscountBtm}, #{txDiscPercNewBtm}, #{lbXDiscPercBtm}, #{lbPPNBtm}, #{lbNetBtm})" />
                <Command Handler="if (command == 'Void') { voidDataFromStore(record); }" />
              </Listeners>
            </ext:GridPanel>
          </Items>
          <FooterBar>
            <ext:Toolbar runat="server" Layout="Anchor" AnchorHorizontal="1">
              <Items>
                <ext:Panel runat="server">
                  <Items>
                    <ext:Hidden ID="hidXDiscOri" runat="server" />
                    <ext:Panel runat="server" Border="false" Header="false" Height="90" Padding="10"
                      Layout="Column">
                      <Items>
                        <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                          <Items>
                            <ext:Label ID="lbBrutoBtm" runat="server" FieldLabel="Bruto" />
                            <ext:Label ID="lbDiscountBtm" runat="server" FieldLabel="Discount" />
                            <ext:CompositeField runat="server" FieldLabel="Extra Discount">
                              <Items>
                                <ext:NumberField ID="txDiscPercNewBtm" runat="server" AllowDecimals="true" AllowNegative="false"
                                  DecimalPrecision="2" Width="50" MaxValue="100">
                                  <Listeners>
                                    <Change Handler="reCallRecalculateTotalPO(#{gridDetail}, #{lbBrutoBtm}, #{lbDiscountBtm}, #{txDiscPercNewBtm}, #{lbXDiscPercBtm}, #{lbPPNBtm}, #{lbNetBtm})" />
                                  </Listeners>
                                </ext:NumberField>
                                <ext:Label runat="server" Text="-" />
                                <ext:Label ID="lbXDiscPercBtm" runat="server" />
                              </Items>
                            </ext:CompositeField>
                          </Items>
                        </ext:Panel>
                        <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                          <Items>
                            <ext:Label ID="lbPPNBtm" runat="server" FieldLabel="PPN" />
                            <ext:Label ID="lbNetBtm" runat="server" FieldLabel="Net" />
                          </Items>
                        </ext:Panel>
                      </Items>
                    </ext:Panel>
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Toolbar>
          </FooterBar>
        </ext:Panel>
      </Center>
      <%--<South MinHeight="80" MaxHeight="80">
      </South>--%>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate">
          <Confirmation ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfPoNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnSend" runat="server" Icon="Mail" Text="Kirim">
      <DirectEvents>
        <Click OnEvent="Mail_OnSend">
          <Confirmation ConfirmRequest="true" Title="Kirim ?" Message="Anda yakin ingin mengirim data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfPoNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfPoNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="ExtraDisc" Value="#{txDiscPercNewBtm}.getValue()" Mode="Raw" />
            <ext:Parameter Name="ExtraDiscOri" Value="#{hidXDiscOri}.getValue()" Mode="Raw" />
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
