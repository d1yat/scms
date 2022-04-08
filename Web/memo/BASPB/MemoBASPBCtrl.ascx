<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemoBASPBCtrl.ascx.cs"
  Inherits="transaksi_memo_BASPBCtrl" %>

<script type="text/javascript">

    var storeToDetailGrid = function(frm, grid, item, cbtipe, quantity) {
        if (!frm.getForm().isValid()) {
            ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
            return;
        }

        if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(quantity) ||
          Ext.isEmpty(cbtipe)) {
            ShowWarning("Objek tidak terdefinisi.");
            return;
        }

        var store = grid.getStore();
        if (Ext.isEmpty(store)) {
            ShowWarning("Objek store tidak terdefinisi.");
            return;
        }

        var valX = [item.getValue()];
        var fieldX = ['c_iteno'];

        var isDup = findDuplicateEntryGrid(store, fieldX, valX);
        var typeDesc = cbtipe.findRecord(cbtipe.valueField, cbtipe.getValue());
        var itemDesc = item.findRecord(item.valueField, item.getValue());

        if (!isDup) {
            var qty = quantity.getValue();
            var itemNo = item.getValue();
            var tipe = cbtipe.getValue();
            var ketInpt = typeDesc.get('v_ket');
            var qtydo = itemDesc.get('n_qty');

            var selisih = qty - qtydo;


            store.insert(0, new Ext.data.Record({
                'c_iteno': itemNo,
                'v_itnam': item.getText(),
                'n_gqty': qty,
                'n_qtydo': qtydo,
                'n_qtydiff': selisih,
                'c_claimtype': tipe,
                'v_ket_type': ketInpt,
                'l_new': true
            }));

            item.reset();
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
    var rt = toolbar.items.get(2); // void button

    var isNew = false,
      isShowReset = ((rec.get('n_sisa') || 0) > 0 ? true : false);

    if (!Ext.isEmpty(rec)) {
      isNew = rec.get('l_new');
    }

    if (Ext.isEmpty(valX) || isNew) {
      del.setVisible(true);
      vd.setVisible(false);
      rt.setVisible(false);
    }
    else {
      del.setVisible(false);
      vd.setVisible(true);
      rt.setVisible(isShowReset);
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
  var focusOnAccDetail = function(o, val) {
    o.setMaxValue(val);
  }
  var resetSisaSPQtyCmd = function(rec) {
    if (rec.get('l_new') || rec.get('l_void')) {
      return;
    }
    var sisa = rec.get('n_sisa');
    var reqQty = rec.get('n_QtyRequest');
    var inUseQty = (reqQty - sisa);
    var qtyTotal = (rec.get('n_totalAcc') || 0) - inUseQty;

    ShowConfirm('Hapus ?', 'Apakah anda yakin ingin mereset jumlah sisa data ini menjadi \'0\' ?',
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
                    function(btnP, txt) {
                      if (btnP == 'ok') {
                        if (txt.trim().length < 1) {
                          txt = 'Kesalahan pemakai.';
                        }

                        rec.set('l_modified', true);
                        rec.set('l_reset', true);
                        rec.set('n_sisa', 0);
                        rec.set('n_totalAcc', qtyTotal);
                        rec.set('n_QtyApprove', inUseQty);
                        rec.set('n_QtyOriginal', reqQty);

                        rec.set('v_keterangan', txt);
                      }
                    });
          }
        });
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfBASPNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeNameCtrl" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="195" Padding="10">
          <Items>
            <ext:ComboBox ID="cbGudangFromHdr" runat="server" FieldLabel="Dari" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="200"
              MinChars="3">
              <Store>
                <ext:Store ID="Store2" runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[['c_gdg = @0', '2', 'System.Char']]" Mode="Raw" />
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
                <table cellpading="0" cellspacing="1" style="width: 200px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                <tpl for="."><tr class="search-item">
                <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Listeners>
                <Change Handler="filterStoreNE(#{cbGudangFromHdr}.getValue(), #{cbGudangToHdr}, 'c_gdg');" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbGudangToHdr" runat="server" FieldLabel="Tujuan" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="200"
              MinChars="3">
              <Store>
                <ext:Store ID="Store3" runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[['c_gdg = @0', '1', 'System.Char']]" Mode="Raw" />
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
              <Template ID="Template3" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 200px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                <tpl for="."><tr class="search-item">
                <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
            </ext:ComboBox>
            <ext:ComboBox ID="cbSJHdr" runat="server" FieldLabel="SJ" DisplayField="c_sjno"
              ValueField="c_sjno" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="150"
              MinChars="3">
              <Store>
                <ext:Store runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="10062" />
                    <ext:Parameter Name="parameters" Value="[['@contains.c_sjno.Contains(@0)', paramTextGetter(#{cbSJHdr}), '']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_sjno" />
                    <ext:Parameter Name="dir" Value="DESC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_sjno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_sjno" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 150px">
                <tr>
                  <td class="body-panel">c_sjno</td>
                </tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_sjno}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});" />
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
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                      ValueField="c_iteno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="500"
                      MinChars="3">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="10063" />
                            <ext:Parameter Name="parameters" Value="[['sjno', #{cbSJHdr}.getValue(), 'System.String'],
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
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
                        <tr>
                        <td class="body-panel">Kode</td>
                        <td class="body-panel">Nama</td>
                        <td class="body-panel">Qty</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_iteno}</td><td>{v_itnam}</td>
                        <td>{n_qty}</td>
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
                    <ext:ComboBox ID="cbTipeDtl" runat="server" DisplayField="v_ket" FieldLabel="Tipe" ValueField="c_type"
                        Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                        AllowBlank="true" ForceSelection="false">
                        <CustomConfig>
                          <ext:ConfigItem Name="allowBlank" Value="true" />
                        </CustomConfig>
                        <Store>
                          <ext:Store ID="Store1" runat="server">
                            <Proxy>
                              <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                CallbackParam="soaScmsCallback" />
                            </Proxy>
                            <BaseParams>
                              <ext:Parameter Name="start" Value="={0}" />
                              <ext:Parameter Name="limit" Value="={10}" />
                              <ext:Parameter Name="model" Value="2001" />
                              <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '63', 'System.String'],
                                ['c_portal = @0', '3', 'System.Char']]" Mode="Raw" />
                              <ext:Parameter Name="sort" Value="c_notrans" />
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
                        <Template ID="Template1" runat="server">
                          <Html>
                          <table cellpading="0" cellspacing="1" style="width: 200px">
                          <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                          <tpl for="."><tr class="search-item">
                          <td>{c_type}</td><td>{v_ket}</td>
                          </tr></tpl>
                          </table>
                          </Html>
                        </Template>
                    </ext:ComboBox>
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Jumlah" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" Width="75" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbTipeDtl}, #{txQtyDtl});" />
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
                    <ext:Parameter Name="model" Value="05005" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['baspbno', #{hfBASPNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_iteno" TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="n_qtydo" Type="Float" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="n_qtydiff" Type="Float" />
                        <ext:RecordField Name="c_claimtype" />
                        <ext:RecordField Name="v_ket_type" />
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
                  <ext:CommandColumn Width="50">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                      <ext:GridCommand CommandName="Reset" Icon="BasketRemove" ToolTip-Title="Command" ToolTip-Text="Reset Sisa Menjadi 0" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfBASPNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="200" />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Qty Terima" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_qtydo" Header="Qty SJ" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_qtydiff" Header="Selisih" Format="0.000,00/i" Width="75" />
                  <ext:Column DataIndex="v_ket_type" Header="Keterangan" Width="150" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); } else if (command == 'Reset') { resetSisaSPQtyCmd(record); }" />
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
    <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfBASPNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfBASPNo}.getValue()" Mode="Raw" />
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
