<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReturCustomerCtrl.ascx.cs"
  Inherits="transaksi_retur_ReturCustomerCtrl" %>

<script type="text/javascript">
  var selectedItemBatch = function(rec, target) {
    if (Ext.isEmpty(target)) {
      ShowWarning("Objek target tidak terdefinisi.");
      return;
    }

    if (Ext.isEmpty(rec)) {
      ShowWarning(String.format("Record '{0}' tidak dapat di baca dari store.", value));
      return;
    }

    var qtyDo = rec.get('n_qty');

    try {
      target.setMinValue(0);

      if (Ext.isNumber(qtyDo)) {
        target.setMaxValue(qtyDo);
        target.setValue(qtyDo);
      }
      else {
        target.setMaxValue(Number.MAX_VALUE);
        target.setValue(qtyDo);
      }
    }
    catch (e) {
      ShowError(e.toString());
    }
  }

  //var storeToDetailGrid = function(frm, grid, item, batch, cbDO, cbRN, tipe, quantity) {
  var storeToDetailGrid = function(frm, grid, item, batch, cbDO, tipe, quantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(batch) ||
          Ext.isEmpty(quantity) ||
          Ext.isEmpty(tipe)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    //    var recItem = item.findRecord(item.valueField, item.getValue());
    //    if (Ext.isEmpty(recItem)) {
    //      ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", item.getText()));
    //      return;
    //    }

    //    var recbatch = batch.findRecord(batch.valueField, batch.getValue());
    //    if (Ext.isEmpty(recbatch)) {
    //      ShowWarning(String.format("Record Batch '{0}' tidak dapat di baca dari store.", batch.getText()));
    //      return;
    //    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var rnNo = '',
      doNo = '',
      rec = '';

    if (cbDO.getValue() == '') {
      doNo = 'DOXXXXXXXX';
      rnNo = 'RNXXXXXXXX';
    }
    else {
      doNo = cbDO.getText().trim();
      rnNo = cbDO.getValue().trim();
    };

    var tipeCode = tipe.getValue().trim();
    var tipeName = tipe.getText().trim();
    var bat = batch.getValue().trim();
    var itemNo = item.getValue().trim();

    if (bat.length < 1) {
      ShowWarning("Batch tidak valid.");
      return;
    }
    
    var valX = [itemNo, bat, doNo, rnNo, tipeCode];
    var fieldX = ['c_iteno', 'c_batch', 'c_dono', 'c_rnno', 'c_type'];

    // Find Duplicate entry
    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var qty = quantity.getValue();
      //      var tip = tipe.getValue().trim();
      var totalQty = 0;

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'c_dono': doNo,
        'c_rnno': rnNo,
        'c_batch': bat,
        'c_type': tipeCode,
        'v_ket_trans': tipeName,
        'n_qty': qty,
        'l_new': true
      }));

      item.reset();
      cbDO.reset();
      //tipe.reset();
      batch.reset();

      quantity.setMaxValue(Number.MAX_VALUE);
      
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

<ext:Window ID="winDetail" runat="server" Height="510" Width="875" Hidden="true"
  Maximizable="true" MinHeight="510" MinWidth="850" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfRCNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="190" MaxHeight="190" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="190" Padding="10">
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
                    <ext:Parameter Name="parameters" Value="[['c_gdg != @0', '3', 'System.Char']]" Mode="Raw" />
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
            <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
              ValueField="c_cusno" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
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
                    <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), ''],
                                              ['(l_cabang == null ? false : l_cabang) = @0', 'false' , 'System.Boolean']]"
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
              <Template ID="Template1" runat="server">
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
            <ext:TextField ID="txPBBRHdr" runat="server" FieldLabel="PBB / PBR" MaxLengthText="20"
              Width="400" AllowBlank="false" MaxLength="50" />
            <ext:ComboBox ID="cbBaspbNo" runat="server" FieldLabel="No.Baspb" DisplayField="c_baspbno"
              ValueField="c_baspbno" Width="200" ItemSelector="tr.search-item" ListWidth="200"
              MinChars="3" ForceSelection="false" >
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="true" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store7" runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={20}" />
                    <ext:Parameter Name="model" Value="3501" />
                    <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                ['@contains.c_baspbno.Contains(@0)', paramTextGetter(#{c_baspbno}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_baspbno" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_baspbno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_baspbno" />
                        <ext:RecordField Name="c_cusno" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template2" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 150px">
                <tr><td class="body-panel">No.Baspb</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_baspbno}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
            </ext:ComboBox>
            <ext:ComboBox ID="cbPLno" runat="server" FieldLabel="No. PL" DisplayField="c_plphar"
              ValueField="c_po_outlet" Width="200" ItemSelector="tr.search-item" ListWidth="200"
              MinChars="3" ForceSelection="false" >
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="true" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store8" runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={-1}" />
                    <ext:Parameter Name="model" Value="3502" />
                    <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                ['@contains.c_plphar.Contains(@0)', paramTextGetter(#{cbPLno}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_plphar" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_plphar" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_plphar" />
                        <ext:RecordField Name="c_po_outlet" />
                        <ext:RecordField Name="v_outlet" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template8" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 250px">
                <tr><td class="body-panel">No. PL</td><td class="body-panel">Outlet</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_plphar}</td>
                    <td>{v_outlet}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
            </ext:ComboBox>
            <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLengthText="100"
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
                      PageSize="10" ListWidth="400" DisplayField="v_itnam" ValueField="c_iteno" 
                      AllowBlank="true" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="2061" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
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
                      <Template ID="Template3" runat="server">
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
                      PageSize="10" ListWidth="350" DisplayField="c_batch" ValueField="c_batch" AllowBlank="true"
                      ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="2111" />
                            <ext:Parameter Name="parameters" Value="[['c_iteno = @0', #{cbItemDtl}.getValue(), 'System.String'],
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
                                <%--<ext:RecordField Name="n_qtybatch" />--%>
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
                          <%--<td class="body-panel">Quantity/batch</td>--%>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{d_expired:this.formatDate}</td>
                            <%--<td>{n_qtybatch}</td>--%>
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
                        <Select Handler="clearRelatedComboRecursive(true, #{cbDO});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbDO" runat="server" FieldLabel="No. DO" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="250" Width="150" DisplayField="c_dono" ValueField="c_rnno"
                      AllowBlank="true" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="0035" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['cusno',#{cbCustomerHdr}.getValue(), 'System.string'],
                                                          ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                          ['batch', #{cbBatDtl}.getValue(), 'System.String'],
                                                          ['@contains.c_dono.Contains(@0)', paramTextGetter(#{cbDO}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_dodate" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_dono" />
                                <ext:RecordField Name="d_dodate" DateFormat="M$" Type="Date" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                                <ext:RecordField Name="c_rnno" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 250px">
                        <tr>
                          <td class="body-panel">No. DO</td>
                          <td class="body-panel">Tanngal DO</td>
                          <td class="body-panel">No. Receive</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_dono}</td>
                            <td>{d_dodate:this.formatDate}</td>
                            <td>{c_rnno}</td>
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
                        <%--<Change Handler="clearRelatedComboRecursive(true, #{cbRN});" />--%>
                        <Select Handler="selectedItemBatch(record, #{txQtyDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <%--<ext:ComboBox ID="cbRN" runat="server" FieldLabel="No. RN" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="150" Width="150" DisplayField="c_rnno" ValueField="c_rnno"
                      AllowBlank="true" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="0036" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                            ['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                                            ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                            ['batch', #{cbBatDtl}.getValue(), 'System.String'],
                                                            ['dono', #{cbDO}.getValue(), 'System.String'],
                                                            ['@contains.c_rnno.Contains(@0)', paramTextGetter(#{cbRN}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_rnno" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_rnno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_rnno" />
                                <ext:RecordField Name="n_qty" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template4" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                          <tr>
                            <td class="body-panel">No. RN</td>
                          </tr>
                          <tpl for="."><tr class="search-item">
                              <td>{c_rnno}</td>
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
                        <Select Handler="selectedItemBatch(this, record, #{txQtyDtl}, #{cbRN})" />
                      </Listeners>
                    </ext:ComboBox>--%>
                    <ext:ComboBox ID="cbTipeDtl" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                      ValueField="c_type" Width="150" AllowBlank="false" ForceSelection="false">
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
                                              ['c_notrans = @0', '10', ''],
                                              ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbTipeDtl}), '']]"
                              Mode="Raw" />
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
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Quantity" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <%--<Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtl}, #{cbDO}, #{cbRN}, #{cbTipeDtl}, #{txQtyDtl});" />--%>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtl}, #{cbDO}, #{cbTipeDtl}, #{txQtyDtl});" />
                      </Listeners>
                    </ext:Button>
                    <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                      Icon="Cancel">
                      <Listeners>
                        <Click Handler="#{frmpnlDetailEntry}.getForm().reset();quantity.setMaxValue(Number.MAX_VALUE);" />
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
                    <ext:Parameter Name="model" Value="0032" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_rcno = @0', #{hfRCNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="c_dono" />
                        <ext:RecordField Name="c_rnno" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="v_ket_trans" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
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
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfRCNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch " />
                  <ext:Column DataIndex="v_ket_trans" Header="Tipe " />
                  <ext:Column DataIndex="c_dono" Header="No. DO" />
                  <ext:Column DataIndex="c_rnno" Header="No. RN" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Quantity" Format="0.000,00/i" Width="75">
                  </ext:NumberColumn>
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
            <ext:Parameter Name="NumberID" Value="#{hfRCNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" ID="btnprosesrs" Icon="Disk" Text="Proses menjadi RS" Hidden = "true">
      <DirectEvents>
        <Click OnEvent="SaveBtnrs_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation ConfirmRequest="true" Title="Proses ?" Message="Anda yakin ingin proses data ini." 
            BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"/>
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfRCNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangId" Value="#{hfGudang}.getValue()" Mode="Raw" />
            <ext:Parameter Name="CustomerID" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="POOUTLET" Value="#{cbPLno}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" ID="btnprosesst" Icon="Hourglass" Text="Proses Dokumen Serah Terima">
        <DirectEvents>
            <Click OnEvent="prosesST" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                <Confirmation ConfirmRequest="true" Title="Proses ?" Message="Anda Yakin ingin proses data ini." BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"/>
                <EventMask ShowMask="true" />
                <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="#{hfRCNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="GudangId" Value="#{hfGudang}.getValue()" Mode="Raw" />
                </ExtraParams>
            </Click>
        </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." 
            BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"/>
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfRCNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{hfGudangDesc}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangId" Value="#{hfGudang}.getValue()" Mode="Raw" />
            <ext:Parameter Name="CustomerDesc" Value="#{cbCustomerHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="CustomerID" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="PBBRNO" Value="#{txPBBRHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
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