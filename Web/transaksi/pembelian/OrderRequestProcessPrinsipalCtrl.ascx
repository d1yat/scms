<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderRequestProcessPrinsipalCtrl.ascx.cs"
  Inherits="transaksi_pembelian_OrderRequestProcessPrinsipalCtrl" %>

<script type="text/javascript">
  var updateFieldSPAccProses = function(store, field, updateField, item, value) {
    var r = '';
    var idx = 0;

    do {
      idx = store.findExact(field, item, idx);
      if (idx == -1) {
        break;
      }
      else {
        r = store.getAt(idx);
        r.set(updateField, value);
        r.commit();

        idx++;
      }
    } while (idx != -1);
  }
  var calculateSPAccProses = function(store, minOrder, field, targetField, item, appendNumber) {
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
  /*
  var calculateSPAccProses = function(store, field, targetField, item, appendNumber) {
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
  idx = store.findExact(field, item);
  if (idx != -1) {
  r = store.getAt(idx);
  if (!Ext.isEmpty(r)) {
  var qminord = r.get('n_qminord');

        nTotal = calculateMoqDenomination(qminord, nTotal);
  }
  }

    updateFieldSPAccProses(store, field, 'n_spacc', item, nTotal);

    return nTotal;
  }
  */
  var storeToDetailGridProses = function(frm, grid, item, sp, quantity) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(sp) ||
          Ext.isEmpty(quantity)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [item.getValue(), sp.getValue()];
    var fieldX = ['c_iteno', 'NoID'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var qty = quantity.getValue();
      var itemNo = item.getValue();
      var spValue = sp.getValue();

      // Find MOQ
      var n_MoqQty = 0;
      var iStore = item.getStore();
      if (!Ext.isEmpty(iStore)) {
        nDup = iStore.findExact('c_iteno', itemNo);
        if (nDup != -1) {
          var r = iStore.getAt(nDup);
          if (!Ext.isEmpty(r)) {
            n_MoqQty = r.get('n_qminord');
          }
        }
      }

      // Calculate SPACC
      var n_spAcc = calculateSPAccProses(store, n_MoqQty, 'c_iteno', 'Quantity', itemNo, qty);

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'n_spacc': n_spAcc,
        'NoID': spValue,
        'NoRef': sp.getText(),
        'n_qminord': n_MoqQty,
        'Quantity': qty,
        'l_manual': true,
        'n_soh': 0,
        'n_sit': 0,
        'n_avgsls': 0,
        'l_spdtl': true
      }));

      item.reset();
      sp.reset();
      quantity.reset();
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }
  //  var processORP = function(g) {
  //    var store = g.getStore();
  //    if (Ext.isEmpty(store)) {
  //      ShowWarning("Objek store tidak terdefinisi.");
  //      return;
  //    }

  //    store.removeAll();
  //    store.reload();
  //  }
  var onChangeSpProses = function(c, v, t) {
    var store = c.getStore();
    if (!Ext.isEmpty(store)) {
      var idx = store.findExact('c_spno', v);
      if (idx != -1) {
        var r = store.getAt(idx);

        var nMax = r.get('n_sisa');
        if (t != null) {
          t.setRawValue(nMax);

          t.setMaxValue(nMax);
        }
      }
    }
  }
  var deleteRecordOnGridEProses = function(grid, rec) {
    var store = grid.getStore();

    ShowConfirm('Hapus ?',
              "Apakah anda yakin ingin menghapus data ini ?",
              function(btn) {
                if (btn == 'yes') {
                  if ((!Ext.isEmpty(store)) && (!Ext.isEmpty(rec))) {
                    var ite = rec.get('c_iteno');
                    var qty = rec.get('Quantity');
                    var minOrder = rec.get('n_qminord');

                    calculateSPAccProses(store, minOrder, 'c_iteno', 'Quantity', ite, -qty);

                    store.remove(rec);
                  }
                }
              });
  }
  //  var functionLoadProcess = function(w) {
  //    Ext.net.Mask.show({ el: w.body,  msg: 'Kalkulasi data..' });
  //  }
  var verifyHeaderAndDetailSaveProses = function(f, g, tip, gdg, divp, via, cust) {
    return verifyHeaderAndDetail(f, g);
    //    if (verifyHeaderAndDetail(f, g)) {
    //      var valTipe = tip.getValue();
    //      if (valTipe == '02') {
    //        if (Ext.isEmpty(gdg.getValue()) || Ext.isEmpty(cust.getValue()) || Ext.isEmpty(divp.getValue()) || Ext.isEmpty(via.getValue())) {
    //          ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
    //          return false;
    //        }
    //      }
    //      else {
    //        if (Ext.isEmpty(divp.getValue()) || Ext.isEmpty(via.getValue())) {
    //          ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
    //          return false;
    //        }
    //      }
    //    }
    //    else {
    //      return false;
    //    }
  }
  var onTipeProsesChanged = function(valu, cb) {
    if (!Ext.isEmpty(cb)) {
      if (valu == '02') {
        cb.allowBlank = false;
        cb.forceSelection = true;
        cb.reset();
      }
      else {
        cb.allowBlank = true;
        cb.forceSelection = false;
        cb.reset();
      }
    }
  }
</script>

<%@ Register Src="OrderRequestDetilInfo.ascx" TagName="OrderRequestDetilInfo" TagPrefix="uc" %>

<ext:Window ID="winDetail" runat="server" Height="480" Width="750" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="750" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfContentID" runat="server" />
    <uc:OrderRequestDetilInfo runat="server" ID="OrderRequestDetilInfo" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Height="175" AutoScroll="true"
          ButtonAlign="Center" Layout="Fit">
          <Items>
            <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
              <Items>
                <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <ext:ComboBox ID="cbSuplierHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
                      ValueField="c_nosup" Width="225" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" AllowBlank="false">
                      <Store>
                        <ext:Store ID="Store1" runat="server">
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
                                                ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplierHdr}), '']]"
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
                        <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});clearRelatedComboRecursive(true, #{cbDivPrincipalHdr}, #{cbItemDtl}, #{cbSPNoDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbTipeHdr" runat="server" FieldLabel="Tipe Transaksi" DisplayField="v_ket"
                      ValueField="c_type" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
                      MinChars="3" AllowBlank="false">
                      <Store>
                        <ext:Store ID="Store2" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '04', 'System.String'], 
                                    ['c_portal = @0', '3', 'System.Char'],
                                    ['c_type <> @0', '05', 'System.String']]" Mode="Raw" />
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
                      <Template ID="Template2" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 250px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_type}</td><td>{v_ket}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Listeners>
                        <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});clearRelatedComboRecursive(true, #{cbSPNoDtl});onTipeProsesChanged(newValue, #{cbCustomerHdr});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
                      ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
                      MinChars="3" ForceSelection="false" AllowBlank="true" CausesValidation="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
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
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 250px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                      </Triggers>
                      <Listeners>
                        <Select Handler="this.triggers[0].show();" />
                        <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                        <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); }" />
                        <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                      ValueField="c_cusno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" AllowBlank="true" ForceSelection="true">
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
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2011" />
                            <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                                             ['l_cabang = @0', true, 'System.Boolean'],
                                                             ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_cunam" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_cusno" />
                                <ext:RecordField Name="c_cab" />
                                <ext:RecordField Name="v_cunam" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template4" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Listeners>
                        <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});clearRelatedComboRecursive(true, #{cbItemDtl}, #{cbSPNoDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                  </Items>
                </ext:Panel>
                <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" ColumnWidth=".5"
                  Layout="Form">
                  <Items>
                    <ext:ComboBox ID="cbDivPrincipalHdr" runat="server" FieldLabel="Divisi Pemasok" DisplayField="v_nmdivpri"
                      ValueField="c_kddivpri" Width="225" ItemSelector="tr.search-item" PageSize="10"
                      ListWidth="400" MinChars="3" ForceSelection="false" AllowBlank="true" CausesValidation="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store5" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2051" />
                            <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                                             ['l_aktif = @0', true, 'System.Boolean'],
                                                             ['c_nosup = @0', paramValueGetter(#{cbSuplierHdr}), 'System.String'],
                                                             ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivPrincipalHdr}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_nmdivpri" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_kddivpri" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_kddivpri" />
                                <ext:RecordField Name="v_nmdivpri" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template5" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_kddivpri}</td><td>{v_nmdivpri}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                      </Triggers>
                      <Listeners>
                        <Select Handler="this.triggers[0].show();" />
                        <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                        <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); }" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
                      ValueField="c_type" Width="125" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
                      MinChars="3" ForceSelection="false" AllowBlank="true" CausesValidation="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="true" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store6" runat="server" RemotePaging="false" SkinID="OriginalExtStore">
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
                      <Template ID="Template6" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 250px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_type}</td><td>{v_ket}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Clear" HideTrigger="true" />
                      </Triggers>
                      <Listeners>
                        <Select Handler="this.triggers[0].show();" />
                        <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                        <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); }" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbTipeProdukHdr" runat="server" FieldLabel="Tipe Produk" DisplayField="v_ket"
                      ValueField="c_type" Width="100" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
                      MinChars="3" AllowBlank="false">
                      <Store>
                        <ext:Store ID="Store7" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '11', 'System.String'],
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
                      <Template ID="Template7" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 250px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_type}</td><td>{v_ket}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                    </ext:ComboBox>
                    <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100"
                      Width="225" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
          <Buttons>
            <ext:Button runat="server" Text="Proses" Icon="CogStart">
              <Listeners>
                <Click Handler="resetEntryWhenChange(#{gridDetail});" />
              </Listeners>
              <DirectEvents>
                <Click Before="return validasiProses(#{frmHeaders});" OnEvent="ProcessORP_Click">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="divSuplier" Value="#{cbDivPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="via" Value="#{cbViaHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="typeItem" Value="#{cbTipeProdukHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="suplier" Value="#{cbSuplierHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="tipeProcess" Value="#{cbTipeHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="gudang" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="customer" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="ContentID" Value="#{hfContentID}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
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
                                                                    ['l_combo = @0', false, 'System.Boolean'],
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbSPNoDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbSPNoDtl" runat="server" FieldLabel="Surat Pesanan" DisplayField="c_sp"
                      ValueField="c_spno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
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
                            <ext:Parameter Name="model" Value="8011" />
                            <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                                                    ['customer', paramValueGetter(#{cbCustomerHdr}), 'System.String'],
                                                                    ['item', paramValueGetter(#{cbItemDtl}), 'System.String'],
                                                                    ['tipeProcess', paramValueGetter(#{cbTipeHdr}), 'System.String'],
                                                                    ['gudang', paramValueGetter(#{cbGudangHdr}), 'System.String'],
                                                                    ['@contains.c_spno.Contains(@0) || @contains.c_sp.Contains(@0)', paramTextGetter(#{cbSPNoDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_spno" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_spno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_spno" />
                                <ext:RecordField Name="c_sp" />
                                <ext:RecordField Name="d_spdate" Type="Date" DateFormat="M$" />
                                <ext:RecordField Name="n_acc" Type="Float" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                                <ext:RecordField Name="n_sisa" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 400px">
                        <tr>
                        <td class="body-panel">Surat Pesanan</td><td class="body-panel">SP Cabang</td>
                        <%--<td class="body-panel">Tanggal</td>--%><td class="body-panel">Jumlah</td>
                        <td class="body-panel">Di Terima</td><td class="body-panel">Sisa</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_spno}</td><td>{c_sp}</td>
                        <%--<td>{d_spdate}</td>--%><td>{n_qty}</td>
                        <td>{n_acc}</td><td>{n_sisa}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="onChangeSpProses(this, newValue, #{txQtyDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Jumlah" AllowBlank="false"
                      AllowDecimals="true" AllowNegative="false" Width="75" MinValue="0.01" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGridProses(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbSPNoDtl}, #{txQtyDtl});" />
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
              <SaveMask ShowMask="true" />
              <LoadMask ShowMask="true" />
              <%--<Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGridEProses(record); } else if(command == 'Submit') { submitORDataToPO(record); }" />
              </Listeners>--%>
              <DirectEvents>
                <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                    <ext:Parameter Name="Parameter" Value="c_iteno" />
                    <ext:Parameter Name="PrimaryID" Value="record.data.c_iteno" Mode="Raw" />
                  </ExtraParams>
                </Command>
              </DirectEvents>
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
                    <ext:Parameter Name="model" Value="8091" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <%--<ext:Parameter Name="parameters" Value="[['divSuplier', #{cbDivPrincipalHdr}.getValue(), 'System.String'],
                      ['via', #{cbViaHdr}.getValue(), 'System.String'],
                      ['typeItem', #{cbTipeProdukHdr}.getValue(), 'System.String'],
                      ['suplier', #{cbSuplierHdr}.getValue(), 'System.String'],
                      ['tipeProcess', #{cbTipeHdr}.getValue(), 'System.String'],
                      ['gudang', #{cbGudangHdr}.getValue(), 'System.String'],
                      ['customer', #{cbCustomerHdr}.getValue(), 'System.String']]" Mode="Raw" />--%>
                    <ext:Parameter Name="parameters" Value="[[]]" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="n_avgsls" Type="Float" />
                        <ext:RecordField Name="n_index" Type="Float" />
                        <ext:RecordField Name="n_soh" Type="Float" />
                        <ext:RecordField Name="n_sit" Type="Float" />
                        <ext:RecordField Name="n_bo" Type="Float" />
                        <ext:RecordField Name="n_spacc" Type="Float" />
                        <ext:RecordField Name="n_box" Type="Float" />
                        <ext:RecordField Name="n_salpri" Type="Float" />
                        <ext:RecordField Name="n_pminord" Type="Float" />
                        <ext:RecordField Name="n_qminord" Type="Float" />
                        <ext:RecordField Name="n_bonus" Type="Float" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="c_via" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="c_kddivpri" />
                        <ext:RecordField Name="v_nmdivpri" />
                        <ext:RecordField Name="n_avgslsdivpri" Type="Float" />
                        <ext:RecordField Name="n_variabel" Type="Float" />
                        <ext:RecordField Name="n_idxp" Type="Float" />
                        <ext:RecordField Name="n_idxnp" Type="Float" />
                        <ext:RecordField Name="n_pareto" Type="Float" />
                        <ext:RecordField Name="n_ideal" Type="Float" />
                        <ext:RecordField Name="n_order" Type="Float" />
                        <ext:RecordField Name="n_deviasi" Type="Float" />
                        <ext:RecordField Name="NoID" />
                        <ext:RecordField Name="NoRef" />
                        <ext:RecordField Name="Quantity" Type="Float" />
                        <ext:RecordField Name="Acceptance" Type="Float" />
                        <ext:RecordField Name="QtySisa" Type="Float" />
                        <ext:RecordField Name="l_combo" Type="Boolean" />
                        <ext:RecordField Name="ItemCombo" />
                        <ext:RecordField Name="l_manual" Type="Boolean" />
                        <ext:RecordField Name="l_spdtl" Type="Boolean" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <%--<SortInfo Field="v_itnam" Direction="ASC" />--%>
                  <%--<Listeners>
                    <BeforeLoad Handler="Ext.net.Mask.show({ el: #{winDetail}.body,  msg: 'Kalkulasi data..' });" />
                    <Load Handler="Ext.net.Mask.hide();" />
                    <LoadException Handler="ShowError(response.toString());Ext.net.Mask.hide();" />
                    <Exception Handler="ShowError(response.toString());Ext.net.Mask.hide();" />
                  </Listeners>--%>
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
                  <ext:CommandColumn Width="50">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    </Commands>
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="200" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Qty" Format="0.000,00/i"
                    Width="75" />
                  <ext:NumberColumn DataIndex="n_spacc" Header="Total SP diterima" Format="0.000,00/i"
                    Width="75" />
                  <%--<ext:Column DataIndex="NoID" Header="Nomor" Width="100" />--%>
                  <%--<ext:Column DataIndex="NoRef" Header="Referensi" Width="100" />--%>
                  <%--<ext:NumberColumn DataIndex="Quantity" Header="Jumlah SP" Format="0.000,00/i" Width="75" />--%>
                  <ext:NumberColumn DataIndex="n_qminord" Header="Min. Pesan" Format="0.000,00/i" Width="75" />
                  <ext:Column DataIndex="c_via" Header="Via" Width="100" Hidden="true" />
                  <ext:Column DataIndex="c_type" Header="Tipe" Width="100" Hidden="true" />
                  <ext:Column DataIndex="c_kddivpri" Header="Kode Divisi Prinsipal" Width="100" Hidden="true" />
                  <ext:Column DataIndex="v_nmdivpri" Header="Nomor" Width="100" Hidden="true" />
                  <ext:NumberColumn DataIndex="n_avgsls" Header="Avg. Sales" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_index" Header="Index" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_soh" Header="SoH" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_sit" Header="SiT" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_bo" Header="Back Order" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_box" Header="Box" Format="0.000,00/i" Width="75" Hidden="false" />
                  <ext:NumberColumn DataIndex="n_salpri" Header="HNA" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_pminord" Header="Min. Harga Pesan" Format="0.000,00/i"
                    Width="75" Hidden="true" />
                  <ext:NumberColumn DataIndex="n_bonus" Header="Bonus" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_avgslsdivpri" Header="Avg. Sales Div. Prinsipal" Format="0.000,00/i"
                    Width="75" Hidden="true" />
                  <ext:NumberColumn DataIndex="n_variabel" Header="Variable" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_idxp" Header="Idxp" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_idxnp" Header="Idxnp" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_pareto" Header="Pareto" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_ideal" Header="Ideal" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_order" Header="Order" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_deviasi" Header="Deviasi" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="QtySisa" Header="Sisa" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:NumberColumn DataIndex="n_beli" Header="Beli" Format="0.000,00/i" Width="75"
                    Hidden="true" />
                  <ext:CheckColumn DataIndex="l_combo" Header="Combo" Width="50" Hidden="true" />
                  <ext:Column DataIndex="ItemCombo" Header="Kode Combo" Width="50" Hidden="true" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGridEProses(this, record); }" />
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
    <%--<ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate">
          <Confirmation ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfOrNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>--%>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetailSaveProses(#{frmHeaders},#{gridDetail}, #{cbTipeHdr}, #{cbGudangHdr}, #{cbDivPrincipalHdr}, #{cbViaHdr}, #{cbCustomerHdr});">
          <Confirmation Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." ConfirmRequest="true"  BeforeConfirm="return verifyHeaderAndDetailSaveProses(#{frmHeaders},#{gridDetail}, #{cbTipeHdr}, #{cbGudangHdr}, #{cbDivPrincipalHdr}, #{cbViaHdr}, #{cbCustomerHdr});"/>
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <%--<ext:Parameter Name="NumberID" Value="#{hfOrNo}.getValue()" Mode="Raw" />--%>
            <ext:Parameter Name="SuplierID" Value="#{cbSuplierHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SuplierName" Value="#{cbSuplierHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="TypeValue" Value="#{cbTipeHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TypeName" Value="#{cbTipeHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="Gudang" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Customer" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="DivPrincipal" Value="#{cbDivPrincipalHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Via" Value="#{cbViaHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TipeProduk" Value="#{cbTipeProdukHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
            <ext:Parameter Name="ContentID" Value="#{hfContentID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
      <DirectEvents>
        <Click OnEvent="ReloadBtn_Click">
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="ContentID" Value="#{hfContentID}.getValue()" Mode="Raw" />
          </ExtraParams>
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
