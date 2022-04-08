<%@ Control Language="C#" AutoEventWireup="true" CodeFile="STTCtrl.ascx.cs" Inherits="transaksi_stt_STTCtrl" %>

<script type="text/javascript">
    var selectedItemBatch = function(combo, rec, target, cbItm, grid) {
        if (Ext.isEmpty(target)) {
            ShowWarning("Objek target tidak terdefinisi.");
            return;
        }

        if (Ext.isEmpty(rec)) {
            ShowWarning(String.format("Record '{0}' tidak dapat di baca dari store.", value));
            combo.clearValue();
            return;
        }

        var recItem = cbItm.findRecord(cbItm.valueField, cbItm.getValue());
        if (Ext.isEmpty(recItem)) {
            ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", cbItm.getText()));
            return;
        }

        var batCode = rec.get('c_batch');
        var qtyBat = (rec.get('n_gsisa') || 0);

        if (qtyBat <= 0.00) {
            ShowWarning(String.format("Batch '{0}' tidak dapat di pergunakan karena <= 0.00", batCode));
            combo.clearValue();
            return false;
        }
        var qtySisaReal = recItem.get('n_sisa');
        var nQty = recItem.get('n_qty');
        var itm = recItem.get('c_iteno');
        var store = grid.getStore();
        var qtyexist = 0;
        var qtyakhir = 0;
        var qtysisacalc = 0;

        var qtySisa = (qtyBat > qtySisaReal ? qtySisaReal : qtyBat);

        try {
            for (nLen = 0; nLen < store.data.length; nLen++) {
                if (store.data.items[nLen].data.c_iteno == itm) {
                    qtyexist += store.data.items[nLen].data.n_gqty;
                }
            }

            //            if (qtyexist == 0) {
            //                target.setMinValue(0);

            //                if (Ext.isNumber(qtySisa)) {
            //                    target.setMaxValue(qtySisa);
            //                }
            //                else {
            //                    target.setMaxValue(Number.MAX_VALUE);
            //                }

            //                if (Ext.isNumber(qtySisa)) {
            //                    target.setValue(qtySisa);
            //                }
            //                else {
            //                    target.setValue(0);
            //                }
            //            }
            if (qtyexist < nQty) {
                target.setMinValue(0);
                qtysisacalc = nQty - qtyexist;

                if (qtysisacalc <= qtyBat) {
                    qtyakhir = qtysisacalc;
                }
                else {
                    qtyakhir = qtyBat;
                }

                target.setMaxValue(qtyakhir);
                target.setValue(qtyakhir);
            }
            else if (qtyexist == nQty) {
                ShowWarning("Jumlah item sudah terpenuhi");
                combo.clearValue();
                return false;
            }
            else {
                ShowWarning(String.format("Batch '{0}' tidak dapat di pergunakan karena qty tidak sesuai", batCode));
                combo.clearValue();
                return false;
            }
        }
        catch (e) {
            ShowError(e.toString());
        }
    }

  var storeToDetailGrid = function(frm, grid, item, batch, quantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
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
      var qty = quantity.getValue();
      var itemNo = item.getValue().trim();
      var totalQty = quantity.getValue;

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'c_batch': bat,
        'n_gqty': qty,
        'l_new': true
      }))

      item.reset();
      batch.reset();
      quantity.reset();
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

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

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfTypeTrx" runat="server" />
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfSTTNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="125" Padding="10">
          <Items>
            <%--<ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
              MinChars="3">
              <Store>
                <ext:Store ID="Store1" runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
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
            </ext:ComboBox>--%>
            <ext:Label ID="lbGudang" runat="server" FieldLabel="Gudang" />
            <ext:ComboBox FieldLabel="No Memo" runat="server" ID="cbMemoHdr" DisplayField="c_memo"
              ValueField="c_mtno" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="500"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy CallbackParam="soaScmsCallback" Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson"
                      Timeout="10000000" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="10" />
                    <ext:Parameter Name="model" Value="9022" />
                    <ext:Parameter Name="parameters" Value="[['c_type = @0', #{hfTypeTrx}.getValue(), 'System.String'],
                                   ['@contains.c_mtno.Contains(@0) || @contains.c_memo.Contains(@0)', paramTextGetter(#{cbMemoHdr}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_mtno" />
                    <ext:Parameter Name="dir" Value="DESC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_mtno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_mtno" />
                        <ext:RecordField Name="c_memo" />
                        <ext:RecordField Name="d_mtdate" Type="Date" DateFormat="M$" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="tmpPL" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 500px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Memo</td><td class="body-panel">Tanggal</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_mtno}</td>
                    <td>{c_memo}</td>
                    <td>{d_mtdate:this.formatDate}</td>
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
              </Listeners>
            </ext:ComboBox>
            <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100"
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
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="500" DisplayField="v_itnam" ValueField="c_iteno" AllowBlank="false">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="9023" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['memo', #{cbMemoHdr}.getValue(), 'System.String'],
                                                          ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_itnam" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_iteno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="v_itnam" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                                <ext:RecordField Name="n_sisa" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template1" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                        <td class="body-panel">Jumlah</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_iteno}</td><td>{v_itnam}</td>
                           <td>{n_sisa:this.formatNumber}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                        <Functions>
                          <ext:JFunction Name="formatNumber" Fn="myFormatNumber" />
                        </Functions>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbBatDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBatDtl" runat="server" FieldLabel="Batch" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="350" DisplayField="c_batch" ValueField="c_batch">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="10" />
                            <ext:Parameter Name="model" Value="9024" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                          ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_expired" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_batch" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="d_expired" DateFormat="M$" Type="Date" />
                                <ext:RecordField Name="n_gsisa" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr>
                          <td class="body-panel">Batch</td>
                          <td class="body-panel">Kadaluarsa</td>
                          <td class="body-panel">Jumlah</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{d_expired:this.formatDate}</td>
                            <td>{n_gsisa:this.formatNumber}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                        <Functions>
                          <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                          <ext:JFunction Name="formatNumber" Fn="myFormatNumber" />
                        </Functions>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Select Handler="selectedItemBatch(this, record, #{txQtyDtl}, #{cbItemDtl}, #{gridDetail})" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Quantity" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtl}, #{txQtyDtl});" />
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
                    <ext:Parameter Name="model" Value="0025" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_stno = @0', #{hfSTTNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
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
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfSTTNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Terpenuhi" Format="0.000,00/i" Width="75" />
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
        <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfSTTNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TypeCode" Value="#{hfTypeTrx}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <%--<Listeners>
            <Click Handler ="createSJ(#{cbFromHdr},#{cbToHdr},#{txKeterangan},#{cbPrincipalHdr});" />
        </Listeners>--%>
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <EventMask ShowMask="true" />
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini."
            BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfSTTNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TypeCode" Value="#{hfTypeTrx}.getValue()" Mode="Raw" />
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
