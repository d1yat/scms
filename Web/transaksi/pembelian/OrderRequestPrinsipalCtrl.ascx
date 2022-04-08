<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderRequestPrinsipalCtrl.ascx.cs"
  Inherits="transaksi_pembelian_OrderRequestPrinsipalCtrl" %>

<script type="text/javascript">
  var calculateSPAcc = function(store, minOrder, field, targetField, item, appendNumber) {
    var nTotal = 0;

    var idx = 0;
    var r = '';

    for (var x = 0, l = store.getCount(); x < l; x++) {
      idx = store.findExact(field, item, idx);
      if (idx == -1) {
        break;
      }
      else {
        r = store.getAt(idx);
        nTotal += r.get(targetField);
        idx++;
        if (idx > l) {
          break;
        }
      }
    }

    nTotal += appendNumber;

    // Denomination
    if (minOrder > 0) {
      nTotal = calculateMoqDenomination(minOrder, nTotal);
    }

    return nTotal;
  }
  var storeToDetailGrid = function(frm, grid, item, quantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(quantity)) {
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

    var n_MoqQty = 0,
      n_salpri = 0;
    if (!isDup) {
      var qty = quantity.getValue();
      var itemNo = item.getValue();

      var iStore = item.getStore();
      if (!Ext.isEmpty(iStore)) {
        nDup = iStore.findExact('c_iteno', itemNo);
        if (nDup != -1) {
          var r = iStore.getAt(nDup);
          if (!Ext.isEmpty(r)) {
            n_MoqQty = r.get('n_qminord');
            n_salpri = r.get('n_salpri');
          }
        }
      }

      var n_spAcc = calculateSPAcc(store, n_MoqQty, 'c_iteno', 'Quantity', itemNo, qty);
      
      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itemdesc': item.getText(),
        'n_Qty': n_spAcc,
        'n_QtyOrd': n_spAcc,
        'n_salpri': n_salpri,
        'n_qminord': n_MoqQty,
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

<ext:Window ID="winDetail" runat="server" Height="480" Width="725" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfOrNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeNameCtrl" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="95" MaxHeight="95" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="95" Padding="10">
          <Items>
            <ext:ComboBox ID="cbSuplierHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
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
                    <ext:Parameter Name="model" Value="2021" />
                    <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                                ['l_hide = @0', false, 'System.Boolean'],
                                                ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplierHdr}), '']]" Mode="Raw" />
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
              <Template runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 400px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
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
                <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});clearRelatedComboRecursive(true, #{cbItemDtl});" />
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
                      ValueField="c_iteno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="700"
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
                            <ext:Parameter Name="model" Value="8001" />
                            <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                                                    ['l_hide = @0', false, 'System.Boolean'],
                                                                    ['c_nosup = @0', paramValueGetter(#{cbSuplierHdr}), 'System.String'],
                                                                    ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
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
                                <ext:RecordField Name="n_salpri" Type="Float" />
                                <ext:RecordField Name="n_pminord" Type="Float" />
                                <ext:RecordField Name="n_qminord" Type="Float" />
                                <ext:RecordField Name="n_index" Type="Float" />
                                <ext:RecordField Name="v_nmdivpri" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 700px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                        <td class="body-panel">Index</td><td class="body-panel">Price Min-Ord</td>
                        <td class="body-panel">Quantity Min-Ord</td><td class="body-panel">Divisi Pemasok</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_iteno}</td><td>{v_itnam}</td>
                        <td>{n_index}</td><td>{n_pminord}</td>
                        <td>{n_qminord}</td><td>{v_nmdivpri}</td>
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
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Jumlah" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" Width="75" MinValue="0.01" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{txQtyDtl});" />
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
                    <ext:Parameter Name="model" Value="0014" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['orno', #{hfOrNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="n_Qty" Type="Float" />
                        <ext:RecordField Name="n_salpri" Type="Float" />
                        <ext:RecordField Name="n_QtyOrd" Type="Float" />
                        <ext:RecordField Name="n_QtyOriginal" Type="Float" />
                        <ext:RecordField Name="n_qminord" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="v_ket" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <SortInfo Field="n_QtyOrd" Direction="DESC" />
                  <Listeners>
                    <BeforeLoad Handler="resetTotalOR(#{sbDtlPanel});" />
                    <Exception Handler="resetTotalOR(#{sbDtlPanel});" />
                    <Load Handler="recalculateTotalOR(this, #{sbDtlPanel});" />
                    <Added Handler="recalculateTotalOR(this, #{sbDtlPanel});" />
                    <Removed Handler="recalculateTotalOR(this, #{sbDtlPanel});" />
                  </Listeners>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfOrNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                  <ext:NumberColumn DataIndex="n_salpri" Header="Harga" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_Qty" Header="Jumlah" Format="0.000,00/i" Width="55" />
                  <ext:NumberColumn DataIndex="n_QtyOrd" Header="Sisa Pesan" Format="0.000,00/i" 
                    Width="80" />
                  <ext:NumberColumn DataIndex="n_qminord" Header="Min. Pesan" Format="0.000,00/i"
                    Width="80" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>
            </ext:GridPanel>
          </Items>
          <BottomBar>
            <ext:StatusBar ID="sbDtlPanel" runat="server" DefaultText="Total: 0" />
          </BottomBar>
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
            <ext:Parameter Name="NumberID" Value="#{hfOrNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" ConfirmRequest="true" Title="Simpan ?"
            Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfOrNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SuplierID" Value="#{cbSuplierHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SuplierName" Value="#{cbSuplierHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TypeName" Value="#{hfTypeNameCtrl}.getValue()" Mode="Raw" />
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
