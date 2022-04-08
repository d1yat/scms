<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DOPrinsipalCtrl.ascx.cs"
  Inherits="transaksi_auto_DOPrinsipalCtrl" %>

<script type="text/javascript">
  var prepareCommands = function(rec, toolbar) {
    var del = toolbar.items.get(0); // delete button

    var isNew = false;

    if (!Ext.isEmpty(rec)) {
      isNew = rec.get('l_new');
    }

    if (isNew) {
      del.setVisible(true);
    }
    else {
      del.setVisible(false);
    }
  }
  var storeToDetailGrid = function(frm, grid, tipe, item, itemPrinc, nomorPo, batch, tglbatch, discQty, quantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(nomorPo) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(tglbatch) ||
          Ext.isEmpty(discQty) ||
          Ext.isEmpty(quantity)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [item.getValue(), tipe.getValue(), nomorPo.getValue(), batch.getValue()];
    var fieldX = ['c_iteno', 'c_type', 'c_pono', 'c_batch'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var qty = quantity.getValue();
      var disc = discQty.getValue();
      var itemNo = item.getValue();

      var idx = -1;
      var stor = '';
      var itenoPrinc = '';
      if (!Ext.isEmpty(itemPrinc)) {
        itenoPrinc = itemPrinc.getValue();
      }
      else {
        stor = item.getStore();

        idx = store.findExact('c_iteno', value.trim());
        if (idx != -1) {
          var rec = store.getAt(idx);
          if (!Ext.isEmpty(rec)) {
            itenoPrinc = rec.get('c_itenopri');
          }
        }
        else {
          itenoPrinc = '';
        }
      }

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'v_undes': '',
        'c_itenopri': itenoPrinc,
        'c_type': tipe.getValue(),
        'c_type_ket': tipe.getText(),
        'c_batch': batch.getValue(),
        'd_expired': tglbatch.getValue(),
        'c_pono': nomorPo.getValue(),
        'n_qty': qty,
        'n_disc': disc,
        'n_qty_sisa': qty,
        'l_claim': (tipe.getValue().trim() == '03' ? true : false),
        'l_pending': false,
        'l_new': true
      }));

      item.reset();
      itemPrinc.reset();
      nomorPo.reset();
      batch.reset();
      tglbatch.reset();
      quantity.reset();
      discQty.reset();
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  var onChangedItemDtl = function(store, value, txt) {
    var idx = store.findExact('c_iteno', value.trim());
    if (idx != -1) {
      var rec = store.getAt(idx);
      if (!Ext.isEmpty(rec)) {
        txt.setValue(rec.get('c_itenopri'));
      }
    }
    else {
      txt.setValue('');
    }
  }
  var onChangePoDtl = function(store, valu, tQty) {
    if ((!Ext.isEmpty(tQty)) && (!Ext.isEmpty(store))) {
      var idx = store.findExact('c_pono', valu);
      if (r != -1) {
        var r = store.getAt(idx);

        tQty.setRawValue(r.get('n_sisa'));
      }
      else {
        tQty.reset();
      }
    }
  }
  var onGridBeforeEdit = function(e) {
    var isNew = e.record.get('l_new');
    if (!isNew) {
      e.cancel = true;
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="850" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfSuplNo" runat="server" />
    <ext:Hidden ID="hfDOID" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="150" MaxHeight="150" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="175" Padding="10"
          AutoScroll="true" ButtonAlign="Center" Layout="Column">
          <Items>
            <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5"
              Layout="Form">
              <Items>
                <ext:ComboBox ID="cbSuplierHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
                  ValueField="c_nosup" Width="225" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                  MinChars="3" AllowBlank="false" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
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
                        <ext:Parameter Name="model" Value="10021" />
                        <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                          ['l_aktif = @0', true, 'System.Boolean'],
                          ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbSuplierHdr}), '']]"
                          Mode="Raw" />
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
                    <table cellpading="0" cellspacing="1" style="width: 300px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                    <tpl for="."><tr class="search-item">
                        <td>{c_nosup}</td><td>{v_nama}</td>
                    </tr></tpl>
                    </table>
                    </Html>
                  </Template>
                  <Listeners>
                    <Change Handler="resetEntryWhenChange(#{gridDetail});clearRelatedComboRecursive(true, #{cbItemDtl});" />
                  </Listeners>
                </ext:ComboBox>
                <ext:TextField ID="txDOHdr" runat="server" FieldLabel="No. DO" MaxLength="50" Width="200" />
                <ext:DateField ID="txDateDoHdr" runat="server" FieldLabel="Tanggal DO" Width="100"
                  Format="dd-MM-yyyy" AllowBlank="false" />
                <ext:TextField ID="txFJHdr" runat="server" FieldLabel="No. Faktur" MaxLength="50"
                  Width="200" />
                <ext:DateField ID="txDateFJHdr" runat="server" FieldLabel="Tanggal Faktur" Width="100"
                  Format="dd-MM-yyyy" AllowBlank="false" />
              </Items>
            </ext:Panel>
            <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" ColumnWidth=".5"
              Layout="Form">
              <Items>
                <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                  ValueField="c_cab" Width="225" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                  MinChars="3" ForceSelection="false" AllowBlank="true">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store2" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
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
                            <ext:RecordField Name="v_cunam" />
                            <ext:RecordField Name="c_cab" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Template ID="Template2" runat="server">
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
                <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
                  ValueField="c_type" Width="125" AllowBlank="false" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store3" runat="server" RemotePaging="false">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2001" />
                        <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '02', '']]" Mode="Raw" />
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
                </ext:ComboBox>
                <ext:TextField ID="txTaxHdr" runat="server" FieldLabel="No. Pajak" MaxLength="20" />
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center>
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbTypeDtl" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                      ValueField="c_type" AllowBlank="false" Width="65">
                      <Store>
                        <ext:Store ID="Store5" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '43', '']]" Mode="Raw" />
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
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="350" DisplayField="v_itnam" ValueField="c_iteno" AllowBlank="false"
                      MinChars="3">
                      <Store>
                        <ext:Store ID="Store4" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2061" />
                            <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                              ['l_hide = @0', false, 'System.Boolean'],
                              ['c_nosup = @0', #{cbSuplierHdr}.getValue(), 'System.String'],
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
                                <ext:RecordField Name="c_itenopri" />
                                <ext:RecordField Name="v_itnam" />
                                <ext:RecordField Name="v_undes" />
                                <%--<ext:RecordField Name="n_SumPending" Type="Float" />
                                <ext:RecordField Name="n_soh" Type="Float" />--%>
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr>
                          <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                          <td class="body-panel">Kode Pemasok</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                          <td>{c_iteno}</td><td>{v_itnam}</td>
                          <td>{c_itenopri}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="onChangedItemDtl(this.getStore(), newValue, #{txItemPrinDtl});clearRelatedComboRecursive(true, #{cbPODtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txItemPrinDtl" runat="server" FieldLabel="Kode Supl." MaxLength="6"
                      Width="60" />
                    <ext:ComboBox ID="cbPODtl" runat="server" FieldLabel="Nomor PO" DisplayField="c_pono"
                      ValueField="c_pono" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                      MinChars="3">
                      <Store>
                        <ext:Store ID="Store6" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="10011" />
                            <ext:Parameter Name="parameters" Value="[['tipe', '01', 'System.String'],
                                                                    ['nosup', paramValueGetter(#{cbSuplierHdr}), 'System.String'],
                                                                    ['item', paramValueGetter(#{cbItemDtl}), 'System.String'],
                                                                    ['@contains.c_pono.Contains(@0)', paramTextGetter(#{cbPODtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_podate" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_pono" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_pono" />
                                <ext:RecordField Name="d_podate" Type="Date" DateFormat="M$" />
                                <ext:RecordField Name="n_sisa" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template4" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 300px">
                        <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td><td class="body-panel">Sisa</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_pono}</td><td>{d_podate:this.formatDate}</td><td>{n_sisa}</td>
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
                        <Change Handler="onChangePoDtl(this.getStore(), newValue, #{txQtyDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txBatchDtl" runat="server" FieldLabel="Batch" AllowBlank="false"
                      MaxLength="15" Width="100" />
                    <ext:DateField ID="txExpiredDateDtl" runat="server" FieldLabel="Expired" Width="100"
                      Format="dd-MM-yyyy" AllowBlank="false" />
                    <ext:NumberField ID="txDiscDtl" runat="server" FieldLabel="Disc" Width="50" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" />
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Qty" Width="50" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbTypeDtl}, #{cbItemDtl}, #{txItemPrinDtl}, #{cbPODtl}, #{txBatchDtl}, #{txExpiredDateDtl}, #{txDiscDtl}, #{txQtyDtl});" />
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
            <ext:GridPanel ID="gridDetail" runat="server" ClicksToEdit="1">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store7" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0301" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['nosup', #{hfSuplNo}.getValue(), 'System.String'],
                      ['nodo', #{hfDOID}.getValue(), 'System.String']]" Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="c_itenopri" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="c_type_ket" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="c_pono" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="n_disc" Type="Float" />
                        <ext:RecordField Name="n_qty_sisa" Type="Float" />
                        <ext:RecordField Name="l_claim" Type="Boolean" />
                        <ext:RecordField Name="l_pending" Type="Boolean" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="v_ket" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <SortInfo Field="v_itnam" Direction="ASC" />
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="200" />
                  <ext:Column DataIndex="c_itenopri" Header="Kode Supl." Width="75" />
                  <ext:Column DataIndex="c_pono" Header="Nomor PO" />
                  <ext:Column DataIndex="c_batch" Header="Batch" Width="125" />
                  <ext:Column DataIndex="c_type_ket" Header="Tipe" Width="75" />
                  <ext:CheckColumn DataIndex="l_claim" Header="Klaim" Width="50" />
                  <ext:CheckColumn DataIndex="l_pending" Header="Tunda" Width="50" Editable="true" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Jumlah" Format="0.000,00/i" Width="55" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } " />
                <BeforeEdit Fn="onGridBeforeEdit" />
              </Listeners>
            </ext:GridPanel>
          </Items>
          <Buttons>
            <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                    ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfDOID}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="SuplierID" Value="#{hfSuplNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:Button ID="Button1" runat="server" Icon="Cancel" Text="Keluar">
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
