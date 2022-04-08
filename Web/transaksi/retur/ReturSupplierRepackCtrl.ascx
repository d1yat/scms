<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReturSupplierRepackCtrl.ascx.cs"
  Inherits="transaksi_retur_ReturSupplierRepackCtrl" %>

<script type="text/javascript">
    var storeToDetailGrid = function(frm, grid, item, batch, gqty, bqty, ket, kpr, cabang, outlet, reason) {
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
      var vcabang = cabang.getValue().trim();
      var voutlet = outlet.getValue().trim();
      var vreason = reason.getValue().trim();
      
      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'c_batch': bat,
        'n_gqty': godqty,
        'n_bqty': badqty,
        'v_ket': vket,
        'c_cprno': vkpr,
        'c_cusno': vcabang,
        'v_cunam': cabang.getText(),
        'c_outlet': voutlet,
        'v_outlet': outlet.getText(),
        'c_reason': vreason,
        'v_reason': reason.getText(),
        'l_new': true
      }));

      item.reset();
      gqty.reset();
      bqty.reset();
      batch.reset();
      ket.reset();
      kpr.reset();
      cabang.reset();
      outlet.reset();
      reason.reset();
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

    var prepareCommandsRepack = function(rec, toolbar, valX) {
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

  var voidInsertedDataFromStoreRepack = function(rec) {
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

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="750" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfTypeName" runat="server" />
    <ext:Hidden ID="hfRSNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="140" MaxHeight="140" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="140" Padding="10">
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
            <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
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
                    <ext:Parameter Name="model" Value="2021" />
                    <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                ['l_hide = @0', false, 'System.Boolean'],
                                ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]"
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
              <Template ID="Template6" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 350px">
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
              </Listeners>
            </ext:ComboBox>
            <ext:TextField runat="server" ID="txKeterangan" FieldLabel="Keterangan" Width="300" />
             <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="0"
              AllowBlank="false">
              <Items>
                <ext:ListItem Value="01" Text="PDF" />
                <ext:ListItem Value="02" Text="Excel Data Only" />
                <ext:ListItem Value="03" Text="Excel" />
              </Items>
            </ext:SelectBox>
          </Items>
        </ext:FormPanel>
      </North>
      <Center>
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar ID="tbPnlGridDetail" runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="150" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="400" DisplayField="v_itnam" ValueField="c_iteno" MinChars="3">
                      <Store>
                        <ext:Store ID="Store2" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0055" />
                            <ext:Parameter Name="parameters" Value="[['supl', #{cbPrincipalHdr}.getValue(), 'System.String'],
                          ['gudang', #{hfGudang}.getValue(), 'System.Char'],
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
                                <ext:RecordField Name="v_itnam" Type="String" />
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
                      <tpl for="."><tr class="search-item">
                          <td>{c_iteno}</td><td>{v_itnam}</td>
                      </tr></tpl>
                      </table>
                        </Html>
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
                      PageSize="10" Width="100" ListWidth="350" DisplayField="c_batch" ValueField="c_batch" MinChars="3">
                      <Store>
                        <ext:Store ID="Store3" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0056" />
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
                                <ext:RecordField Name="N_GSISA" />
                                <ext:RecordField Name="N_BSISA" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template5" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr>
                          <td class="body-panel">Batch</td>
                          <td class="body-panel">Kadaluarsa</td>
                          <td class="body-panel">Good Qty</td>
                          <td class="body-panel">Bad Qty</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{d_expired:this.formatDate}</td>
                            <td>{N_GSISA}</td>
                            <td>{N_BSISA}</td>
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
                        <Select Handler="selectedItemBatch(record, #{txGQtyDtl}, #{txBQtyDtl}, #{cbBatDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txGQtyDtl" runat="server" FieldLabel="Good Qty" AllowNegative="false"
                      Width="50" AllowBlank="false" />
                    <ext:NumberField ID="txBQtyDtl" runat="server" FieldLabel="Bad Qty" AllowNegative="false"
                      Width="50" AllowBlank="false" />
                    <ext:ComboBox ID="cbCustomer" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                      ValueField="c_cusno" Width="100" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" ForceSelection="false" AllowBlank = "true">
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
                            <ext:Parameter Name="model" Value="2011" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), '']]"
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
                        <Select Handler="clearRelatedComboRecursive(this, #{cbOutlet})" />                    
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbOutlet" runat="server" FieldLabel="Outlet" DisplayField="v_outlet"
                      ValueField="c_outlet" Width="100" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" ForceSelection="false" AllowBlank="true" >
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store11" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2181" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_outlet.Contains(@0) || @contains.v_outlet.Contains(@0)', paramTextGetter(#{cbOutlet}), ''],
                                                                        ['c_cusno = @0', #{cbCustomer}.getValue(), ''],
                                                                        ['v_outlet != @0', ' ', '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_outlet" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_outlet" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_outlet" />
                                <ext:RecordField Name="v_outlet" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                            <tr><td class="body-panel">Kode</td><td class="body-panel">Outlet</td></tr>
                            <tpl for="."><tr class="search-item">
                                <td>{c_outlet}</td><td>{v_outlet}</td>
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
                    <ext:ComboBox ID="cbReason" runat="server" FieldLabel="Alasan" DisplayField="v_reason"
                      ValueField="c_reason" Width="100" ListWidth="400" ItemSelector="tr.search-item"
                      MinChars="3" ForceSelection="false" AllowBlank="true" >
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store4" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2191" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_reason.Contains(@0) || @contains.v_reason.Contains(@0)', paramTextGetter(#{cbReason}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_reason" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_reason" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_reason" />
                                <ext:RecordField Name="v_reason" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template4" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr>
                          <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                          <td>{c_reason}</td><td>{v_reason}</td>
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
                    <ext:TextField ID="txKetDtl" runat="server" FieldLabel="Keterangan" Width="75" AllowBlank="false" />
                    <ext:TextField ID="txKPRDtl" runat="server" FieldLabel="No. KPR" Width="75" AllowBlank="true" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtl}, #{txGQtyDtl}, #{txBQtyDtl}, #{txKetDtl}, #{txKPRDtl}, #{cbCustomer}, #{cbOutlet} , #{cbReason});" />
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
                    <ext:Parameter Name="model" Value="0054" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_rsno = @0', #{hfRSNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="Nn_gqty" Type="Float" />
                        <ext:RecordField Name="n_bqty" Type="Float" />
                        <ext:RecordField Name="n_gqtyH" Type="Float" />
                        <ext:RecordField Name="n_bqtyH" Type="Float" />
                        <ext:RecordField Name="v_ket" />
                        <ext:RecordField Name="c_cprno" />
                        <ext:RecordField Name="c_cusno" />
                        <ext:RecordField Name="v_cunam" />
                        <ext:RecordField Name="c_outlet" />
                        <ext:RecordField Name="v_outlet" />
                        <ext:RecordField Name="c_reason" />
                        <ext:RecordField Name="v_reason" />
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
                    <PrepareToolbar Handler="prepareCommandsRepack(record, toolbar, #{hfRSNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch " />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Good Qty" Format="0.000,00/i" Width="75">
                    <%--<Editor>
                      <ext:NumberField DataIndex="n_gqty" runat="server" Header="Good Qty" Width="75" />
                    </Editor>--%>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_bqty" Header="Bad Qty" Format="0.000,00/i" Width="75">
                    <%--<Editor>
                      <ext:NumberField DataIndex="n_bqty" runat="server" Header="Bad Qty" Width="75" />
                    </Editor>--%>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_gqtyH" Format="0.000,00/i" Hidden="true" Width="75" />
                  <ext:NumberColumn DataIndex="n_bqtyH" Format="0.000,00/i" Hidden="true" Width="75" />
                  <ext:Column DataIndex="v_ket" Header="Keterangan">
                    <%--<Editor>
                      <ext:TextField runat="server" DataIndex="v_ket" Header="Keterangan" />
                    </Editor>--%>
                  </ext:Column>
                  <ext:Column DataIndex="c_cprno" Header="No KPR"></ext:Column>
                  <ext:Column DataIndex="v_cunam" Header="Cabang"></ext:Column>
                  <ext:Column DataIndex="v_outlet" Header="Outlet"></ext:Column>
                  <ext:Column DataIndex="v_reason" Header="Alasan"></ext:Column>
                    <%--<Editor>
                      <ext:TextField runat="server" DataIndex="c_cprno" Header="No KPR" />
                    </Editor>--%>
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                  <ext:CheckColumn DataIndex="l_modified" Header="Modif" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreRepack(record); }" />
                <%--<Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); }" />--%>
              </Listeners>
            </ext:GridPanel>
          </Items>
          <Buttons>
            <ext:Button ID="btnPrintUpload" runat="server" Icon="Printer" Text="Cetak Upload">
              <DirectEvents>
                <Click OnEvent="Report_Upload_OnGenerate">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                    ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="GudangId" Value="#{hfGudang}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfRSNo}.getValue()" Mode="Raw" />      
                    <ext:Parameter Name="supplierID" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />                               
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>      
            <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
              <DirectEvents>
                <Click OnEvent="Report_OnGenerate">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                    ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="GudangId" Value="#{hfGudang}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfRSNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="OutputRpt" Value="#{cbRptTypeOutput}.getValue()" Mode="Raw" />                    
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click">
                  <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfRSNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="GudangDesc" Value="#{hfGudangDesc}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="GudangId" Value="#{hfGudang}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="PrinsipalDesc" Value="#{cbPrincipalHdr}.getText()" Mode="Raw" />
                    <ext:Parameter Name="PrinsipalID" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="TypeName" Value="#{hfTypeName}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
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
