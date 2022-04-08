<%@ Control Language="C#" AutoEventWireup="true" 
CodeFile="TransferGudangRepackConfirmCtrl.ascx.cs" 
Inherits="transaksi_transfer_TransferGudangRepackConfirmCtrl" %>

<script type="text/javascript">

  var selectedItemBatchConfirm = function(combo, rec, target1, target2) {
    if (Ext.isEmpty(target1)) {
      ShowWarning("Objek target tidak terdefinisi.");
      return;
    }
    if (Ext.isEmpty(target2)) {
      ShowWarning("Objek target tidak terdefinisi.");
      return;
    }

    var gsisa = rec.get('N_GSISA');
    var bsisa = rec.get('N_BSISA');

    try {
      target1.setMinValue(0);
      target2.setMinValue(0);

      if (Ext.isNumber(gsisa)) {
        target1.setMaxValue(gsisa);
      }
      else {
        target1.setMaxValue(Number.MAX_VALUE);
      }
      if (Ext.isNumber(bsisa)) {
        target2.setMaxValue(bsisa);
      }
      else {
        target2.setMaxValue(Number.MAX_VALUE);
      }
      if (Ext.isNumber(gsisa)) {
        target1.setValue(gsisa);
      }
      if (Ext.isNumber(bsisa)) {
        target2.setValue(bsisa);
      }
    }
    catch (e) {
      ShowError(e.toString());
    }
  }

  var validasiTotalPermintaanConfirm = function(store, fieldItem, fieldSpg, itemCode, spCode, inpQty, totalSoh) {
    var total = inpQty;
    var idx = 0;
    var rec = '';
    var spc = '';

    do {
      idx = store.findExact(fieldItem, itemCode, idx);
      if (idx != -1) {
        rec = store.getAt(idx);
        if (!Ext.isEmpty(rec)) {
          spc = rec.get('c_spgno').trim();
          if (spc == spCode) {
            total += rec.get('n_qty');
          }
        }
        idx++;
      }
    } while (idx != -1);

    if (total > totalReq) {
      return false;
    }
    else if (total > totalSoh) {
      return false;
    }

    return true;
  }


  var prepareCommandsConfirm = function(rec, toolbar, valX) {
    var del = toolbar.items.get(0); // delete button
    //    var vd = toolbar.items.get(1); // void button

    var isNew = false;

    if (!Ext.isEmpty(rec)) {
      isNew = rec.get('l_new');
    }

    if (Ext.isEmpty(valX) || isNew) {
      del.setVisible(true);
      //      vd.setVisible(false);
    }
    else {
      del.setVisible(false);
      //      vd.setVisible(true);
    }
  }

  var voidInsertedDataFromStoreConfirm = function(rec) {
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

  var afterEdit = function(e) {

  e.record.set('l_modified', true);
  
  };

//  var beforeEditDataComboGrid = function(e) {
//    if (e.field == 'v_ket_type_dc') {
//      if (e.record.get('l_new') || e.record.get('l_void')) {
//        e.cancel = true;
//      }
//      else if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && (e.record.get('n_gqty') <= 0) && (e.record.get('n_bqty') <= 0)) {
//        e.cancel = true;
//      }
//    }
//    else if (e.field == 'n_gqty') {
//      if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && (e.value <= 0)) {
//        e.cancel = true;
//      }
//    }
//  }

//  var afterEditDataComboGrid = function(e, cb) {
//    e.record.set('n_gqtyH', e.record.json.n_gqty);
//    e.record.set('n_bqtyH', e.record.json.n_bqty);
//    
//    if (e.field == 'v_ket_type_dc') {
//      var stor = cb.getStore();
//      if (!Ext.isEmpty(stor)) {
//        var rec = stor.getById(e.value);

//        if (!Ext.isEmpty(rec)) {
//          switch (e.value) {
//            case '01':
//            case '03':
//            case '04':
//              e.record.set('n_gqty', 0);
//              e.record.set('n_bqty', 0);
//              break;
//          }
//          e.record.set('c_type_dc', e.value);
//          e.record.set('v_ket_type_dc', rec.get('v_ket'));
//          e.record.set('l_modified', true);

//          return;
//        }
//      }
//      e.record.set('c_type_dc', '');
//      e.record.set('v_ket_type_dc', '');
//    }
  //  }
  var beforeEditDataConfirm = function(e) {
    if (e.field == 'v_ket_type_dc') {
      if (e.record.get('l_new') || e.record.get('l_void')) {
        e.cancel = true;
      }
      else if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && ((e.record.get('n_gqty') == 0) && (e.record.get('n_bqty') == 0))) {
        e.cancel = true;
      }
    }
    else if (e.field == 'n_gqty') {
      if ((!e.record.get('l_new')) && (!e.record.get('l_void')) && (!e.record.get('l_modified')) && (e.value <= 0)) {
        e.cancel = true;
      }
    }
  }

  var afterEditDataConfirm = function(e, cb) {
    if (e.field == 'v_ket_type_dc') {
      var stor = cb.getStore();

      if (!Ext.isEmpty(stor)) {
        var rec = stor.getById(e.value);

        if (!Ext.isEmpty(rec)) {
          switch (e.value) {
            case '03':
            case '04':
              e.record.set('n_gqty', 0);
              e.record.set('n_bqty', 0);
              break;
          }
          e.record.set('c_type_dc', e.value);
          e.record.set('v_ket_type_dc', rec.get('v_ket'));
          e.record.set('l_modified', true);

          return;
        }
      }

      e.record.set('c_type_dc', '');
      e.record.set('v_ket_type_dc', '');
    }
    else if (e.field == 'n_gqty') {
      var nBook = e.record.get('n_booked');
      if (e.value > nBook) {
        e.value = nBook;
        e.record.set('n_gqty', nBook);
      }
      else if (e.value < 0) {
        e.value = nBook;
        e.record.set('n_gqty', nBook);
      }
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="640">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfConfMode" runat="server" />
    <ext:Hidden ID="hfSJNoConf" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North Collapsible="false" MinHeight="175" MaxHeight="350">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Width="400" Height="175"
          Padding="10">
          <Items>
            <%--<ext:ComboBox ID="cbFromHdr" runat="server" FieldLabel="From" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store runat="server" RemotePaging="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbFromHdr}), '']]"
                      Mode="Raw" />
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
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 200px">
                      <tr><td class="body-panel">Kode</td><td class="body-panel">From</td></tr>
                      <tpl for=".">
                        <tr class="search-item">
                          <td>{c_gdg}</td><td>{v_gdgdesc}</td>
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
              </Listeners>
            </ext:ComboBox>--%>
            <ext:Label ID="lbGudangFrom" runat="server" FieldLabel="Asal" />
            <ext:ComboBox ID="cbToHdr" runat="server" FieldLabel="Tujuan" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
              MinChars="3" AllowBlank="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store1" runat="server" RemotePaging="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <%--<ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={10}" />--%>
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0176" />
                    <ext:Parameter Name="parameters" Value="[['c_gdg != @0', #{hfGudang}.getValue(), 'System.Char']]"
                      Mode="Raw" />
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
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 200px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                <tpl for=".">
                  <tr class="search-item">
                    <td>{c_gdg}</td><td>{v_gdgdesc}</td>
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
                <Change Handler="resetEntryWhenChange(#{gridDetail});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="false" ForceSelection="false">
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
            <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100"
              Width="400" />
            <ext:Checkbox ID="chkConfirm" runat="server" FieldLabel="Confirm" />
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="Panel1" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="500" DisplayField="v_itnam" ValueField="c_iteno" AllowBlank="false">
                      <Store>
                        <ext:Store ID="Store2" runat="server" AutoLoad="false">
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
                                  ['c_nosup = @0', #{cbPrincipalHdr}.getValue(), 'System.String'],
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
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Pemasok</td>
                        <%--<td class="body-panel">SG Pending</td><td class="body-panel">Stock</td>--%>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_iteno}</td><td>{v_itnam}</td>
                            <%--<td>{n_sgpending}</td><td>{n_soh}</td>--%>
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
                      PageSize="10" ListWidth="500" DisplayField="c_batch" ValueField="c_batch" AllowBlank="false">
                      <Store>
                        <ext:Store ID="StoreBatch" runat="server" AutoLoad="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="4302" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudang}.getValue(), 'System.Char'],
                                                          ['iteno', #{cbItemDtl}.getValue(), 'System.String'],
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
                                <ext:RecordField Name="N_BSISA" />
                                <ext:RecordField Name="N_GSISA" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template5" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 500px">
                        <tr>
                          <td class="body-panel">Batch</td>
                          <td class="body-panel">Kadaluarsa</td>
                          <td class="body-panel">Good</td>
                          <td class="body-panel">Bad</td>
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
                        <Select Handler="selectedItemBatchConfirm(this, record, #{txGQtyDtl}, #{txBQtyDtl})" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txGQtyDtl" runat="server" FieldLabel="Good" AllowNegative="false"
                      AllowDecimals="true" DecimalPrecision="2" Width="75" AllowBlank="false" />
                    <ext:NumberField ID="txBQtyDtl" runat="server" FieldLabel="Bad" AllowNegative="false"
                      AllowDecimals="true" DecimalPrecision="2" Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtl}, #{txGQtyDtl}, #{txBQtyDtl});" />
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
            <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store6" runat="server" RemotePaging="false" RemoteSort="false" AutoLoad="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0004" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_sjno = @0', #{hfSJNoConf}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_itnam" Type="String" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                        <ext:RecordField Name="n_QtyRequestBad" Type="Float" />
                        <ext:RecordField Name="n_gqty" Type="Float" />
                        <ext:RecordField Name="n_bqty" Type="Float" />
                        <ext:RecordField Name="n_booked" Type="Float" />
                        <ext:RecordField Name="n_booked_bad" Type="Float" />
                        <ext:RecordField Name="c_type_dc" />
                        <ext:RecordField Name="v_ket_type_dc" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
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
                      <%--<ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />--%>
                    </Commands>
                    <PrepareToolbar Handler="prepareCommandsConfirm( record, toolbar, #{hfSJNoConf}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />
                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi Baik" Format="0.000,00/i"  Width="100" />
                  <ext:NumberColumn DataIndex="n_booked_bad" Header="Alokasi Buruk" Format="0.000,00/i" Width="100" />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Terpenuhi Baik" Format="0.000,00/i" Width="100">
                    <Editor>
                      <ext:NumberField ID="NumberField1" DataIndex="n_gqty" runat="server" Header="Good Qty" Width="75" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_bqty" Header="Terpenuhi Buruk" Format="0.000,00/i" Width="100">
                    <Editor>
                      <ext:NumberField ID="NumberField2" DataIndex="n_bqty" runat="server" Header="Bad Qty" Width="75" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_gqtyH" Format="0.000,00/i" Hidden="true" Width="75" />
                  <ext:NumberColumn DataIndex="n_bqtyH" Format="0.000,00/i" Hidden="true" Width="75" />
                  <ext:Column ColumnID="TypeBatal" DataIndex="v_ket_type_dc" Header="Batal" Width="100">
                    <Editor>
                      <ext:ComboBox ID="cbTypeDcGrd" runat="server" DisplayField="v_ket" ValueField="c_type"
                        ForceSelection="false" MinChars="3" AllowBlank="true">
                        <CustomConfig>
                          <ext:ConfigItem Name="allowBlank" Value="true" />
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
                                              ['c_notrans = @0', '60', '']]" Mode="Raw" />
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
                    </Editor>
                  </ext:Column>
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                <%--<AfterEdit Fn="afterEdit" />--%>
                <AfterEdit Handler="afterEditDataConfirm(e, #{cbTypeDcGrd});" />
                <BeforeEdit Handler="beforeEditDataConfirm(e);" />
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
            <ext:Parameter Name="NumberID" Value="#{hfSJNoConf}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="btnSimpan_OnClick">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfSJNoConf}.getValue()" Mode="Raw" />
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
<ext:Window ID="wndDown" runat="server" Hidden="true" />

