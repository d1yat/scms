<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReturSupplierConfCtrl.ascx.cs"
  Inherits="transaksi_retur_ReturSupplierConfCtrl" %>

<script type="text/javascript">
  var selectedItemBatchConf = function(rec, target1, target2, cbBatDtl, rej, rew, red) {
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

    var gqty = recBat.get('good');
    var bqty = recBat.get('bad');

    try {
      target1.setMinValue(0);
      target2.setMinValue(0);
      //      rew.setMinValue(0);
      //      red.setMinValue(0);

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

  var prepareCommandsConf = function(rec, toolbar, valX) {
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

  var storeToDetailGridConf = function(frm, grid, item, batch, gqty, bqty, rej, rew, red, NoRef) {
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

    var storBatch = batch.getStore();
    var bat = batch.getValue().trim();
    var godqty = gqty.getValue();
    var badqty = bqty.getValue();
    var reqQty = 0;
    var itemNo = item.getValue().trim();
    var nQty = godqty + badqty;
    var reject = rej.getValue();
    var rework = rew.getValue();
    var redress = red.getValue();
    var rsNo = NoRef.getValue();

    if (badqty > 0 && godqty > 0) {
      ShowWarning("Input Good atau Bad.");
      return;
    }
    else {
      if (badqty > 0) {
        if ((badqty) < (reject +  redress)) {
          ShowWarning("Jumlah kelebihan.");
          return;
        }
      }
      else {
        if ((godqty) < (reject +  redress)) {
          ShowWarning("Jumlah kelebihan.");
          return;
        }
      }
    }

    if (!isDup) {
      store.insert(0, new Ext.data.Record({
        'c_noref': rsNo,
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'c_batch': bat,
        'good': godqty,
        'bad': badqty,
        'nQty': nQty,
        'nReject': reject,
        'nRework': rework,
        'nRedress': redress,
        'l_alternate': true,
        'l_new': true
      }));

      item.reset();
      gqty.reset();
      bqty.reset();
      batch.reset();
      rej.reset();
      rew.reset();
      red.reset();

    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  var clearGridDetailConf = function(g) {
    if (Ext.isEmpty(g)) {
      return;
    }

    var store = g.getStore();
    if (!Ext.isEmpty(store)) {
      store.removeAll();
    }
  }

  var checkGridDetailConf = function(g) {
    if (Ext.isEmpty(g)) {
      ShowWarning('Grid tidak dapat dibaca.');
      return false;
    }

    var store = g.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning('Store.Grid tidak dapat dibaca.');
      return false;
    }

    var hasError = false;
    var nRej = 0,
      nRed = 0,
      nRew = 0,
      nQty = 0,
      nTotal = 0;

    store.each(function(r) {
      if (!hasError) {
        nQty = r.get('nQty');

        nRej = r.get('nReject');
        nRew = r.get('nRework');
        nRed = r.get('nRedress');

        nTotal = (nRed + nRew + nRej);

        if (nQty != nTotal) {
          hasError = true;
          return;
        }
      }
    });

    if (hasError) {
      ShowWarning('Terdapat isi grid yang tidak sesuai dengan jumlah quantity, silahkan diperbaiki.');
      return false;
    }

    return true;
  }

  var cekVerifyHeaderAndDetailConf = function(f, g) {
    var isOk = verifyHeaderAndDetail(f, g);
    if (isOk) {
      isOk = checkGridDetailConf(g);
    }

    return isOk;
  }
  var Test = function(editor, e) {
    if (e.value !== e.originalValue) {

    }
};

var setDefaultReject = function(chkDefault, store, chkRework,chkRedress) {
    var chk = chkDefault.getValue();

    if (chk) {
        store.each(function(r) {
            r.set('nReject', r.get('nQty'));
            r.set('nRework',0);
            r.set('nRedress', 0);
        });
        chkRework.setValue(false);
        chkRedress.setValue(false);
    }
    else {
        store.each(function(r) {
            r.set('nReject', 0);
        });
    }
}

var setDefaultRework = function(chkDefault, store, chkReject, chkRedress) {
    var chk = chkDefault.getValue();

    if (chk) {
        store.each(function(r) {
            r.set('nRework', r.get('nQty'));
            r.set('nReject', 0);
            r.set('nRedress', 0);
        });
        chkReject.setValue(false);
        chkRedress.setValue(false);
    }
    else {
        store.each(function(r) {
            r.set('nRework', 0);
        });
    }
}

var setDefaultRedress = function(chkDefault, store, chkReject, chkRework) {
    var chk = chkDefault.getValue();

    if (chk) {
        store.each(function(r) {
            r.set('nRedress', r.get('nQty'));
            r.set('nReject', 0);
            r.set('nRework', 0);
        });
        chkReject.setValue(false);
        chkRework.setValue(false);
    }
    else {
        store.each(function(r) {
            r.set('nRedress', 0);
        });
    }
}
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="885" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfRSNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="220" MaxHeight="220" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="220" Padding="10"
          ButtonAlign="Center">
          <Items>
            <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
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
              <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 250px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                <tpl for="."><tr class="search-item">
                <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                </tr></tpl>
                </table>
                </Html>
              </Template>
              <Listeners>
                <Change Handler="clearGridDetailConf(#{gridDetail});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="false" ForceSelection="false">
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
                <Change Handler="clearGridDetailConf(#{gridDetail});clearRelatedComboRecursive(true, #{cbNoRefDtl});" />
              </Listeners>
            </ext:ComboBox>
            <ext:TextField runat="server" ID="txCPRNo" FieldLabel="Ref. Pemasok" Width="300" MaxLength="100" />
            <%--<ext:TextField FieldLabel="No. RS" runat="server" ID="txNoRS" />--%>
            <ext:ComboBox ID="cbNoRefDtl" runat="server" FieldLabel="No. RS" DisplayField="c_noref"
              ValueField="c_noref" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="235"
              MinChars="3" AllowBlank="true" ForceSelection="false">
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
                    <ext:Parameter Name="model" Value="0132" />
                    <ext:Parameter Name="parameters" Value="[['gdg',#{cbGudangHdr}.getValue(),'System.Char'],
                                                           ['supl',#{cbPrincipalHdr}.getValue(),'System.String'],
                                                           ['@contains.c_noref.Contains(@0)', paramTextGetter(#{cbNoRefDtl}), '']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_noref" />
                    <ext:Parameter Name="dir" Value="Desc" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_noref" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_noref" />
                        <ext:RecordField Name="d_rsdate" DateFormat="M$" Type="Date" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template2" runat="server">
                <Html>
                <table cellpading="0" cellspacing="0" style="width: 235px">
              <tr>
              <td class="body-panel">Faktur</td>
              <td class="body-panel">Tanggal</td>
              </tr>
              <tpl for="."><tr class="search-item">
              <td>{c_noref}</td>
              <td>{d_rsdate:this.formatDate}</td>
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
            <ext:TextField runat="server" ID="txKeterangan" FieldLabel="Keterangan" Width="200" MaxLength="50" />
          </Items>
          <Buttons>
            <ext:Button runat="server" Text="Proses" Icon="CogStart">
              <DirectEvents>
                <Click Before="return validasiProses(#{frmHeaders});" OnEvent="OnEvenAddGrid">
                  <EventMask ShowMask="true" />
                  <%--<ExtraParams>
                    <ext:Parameter Name="Parameter" Value="c_rsno" />
                    <ext:Parameter Name="PrimaryID" Value="#{txNoRS}.getValue()" Mode="Raw" />                    
                  </ExtraParams>--%>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
        </ext:FormPanel>
      </North>
      <Center>
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <%--<ext:ComboBox ID="cbNoRefDtl" runat="server" FieldLabel="No. RS" DisplayField="c_noref"
                      ValueField="c_noref" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="235"
                      MinChars="3" AllowBlank="true" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="0132" />
                            <ext:Parameter Name="parameters" Value="[['gdg',#{cbGudangHdr}.getValue(),'System.Char'],
                                                                   ['supl',#{cbPrincipalHdr}.getValue(),'System.String'],
                                                                   ['@contains.c_noref.Contains(@0)', paramTextGetter(#{cbNoRefDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_noref" />
                            <ext:Parameter Name="dir" Value="Desc" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_noref" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_noref" />
                                <ext:RecordField Name="d_rsdate" DateFormat="M$" Type="Date" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template2" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 235px">
                      <tr>
                      <td class="body-panel">Faktur</td>
                      <td class="body-panel">Tanggal</td>
                      </tr>
                      <tpl for="."><tr class="search-item">
                      <td>{c_noref}</td>
                      <td>{d_rsdate:this.formatDate}</td>
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
                    </ext:ComboBox>--%>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="400" DisplayField="v_itnam" ValueField="c_iteno">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0134" />
                            <ext:Parameter Name="parameters" Value="[['rsno', #{cbNoRefDtl}.getValue(), 'System.String'],
                                                                     ['gdg', #{cbGudangHdr}.getValue(), 'System.Char'],
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
                      PageSize="10" Width="150" ListWidth="350" DisplayField="c_batch" ValueField="c_batch">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0135" />
                            <ext:Parameter Name="parameters" Value="[['rsno', #{cbNoRefDtl}.getValue(), 'System.String'],
                                                                    ['gdg', #{cbGudangHdr}.getValue(), 'System.Char'],
                                                                    ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                                    ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_batch" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_batch" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="good" />
                                <ext:RecordField Name="bad" />
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
                          <td class="body-panel">Good Qty</td>
                          <td class="body-panel">Bad Qty</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{good}</td>
                            <td>{bad}</td>
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
                        <Select Handler="selectedItemBatchConf(record, #{txGQtyDtl}, #{txBQtyDtl}, #{cbBatDtl}, #{txQRejectDtl}, #{txQReworkDtl}, #{txQRedressDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txGQtyDtl" runat="server" FieldLabel="Good Qty" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:NumberField ID="txBQtyDtl" runat="server" FieldLabel="Bad Qty" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:NumberField ID="txQRejectDtl" runat="server" FieldLabel="Reject" AllowNegative="false"
                      Width="50" AllowBlank="false" />
                    <ext:NumberField ID="txQReworkDtl" runat="server" FieldLabel="Rework" AllowNegative="false"
                      Width="50" Hidden="true" />
                    <ext:NumberField ID="txQRedressDtl" runat="server" FieldLabel="Redress" AllowNegative="false"
                      Width="50" AllowBlank="false"/>
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGridConf(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtl}, #{txGQtyDtl}, #{txBQtyDtl}, #{txQRejectDtl}, #{txQReworkDtl}, #{txQRedressDtl}, #{cbNoRefDtl});" />
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
                <ext:ToolbarSeparator />
                <ext:Checkbox runat="server" ID="chkAllReject" FieldLabel="Set All Reject">
                    <Listeners>
                        <Check Handler="setDefaultReject(this, #{gridDetail}.getStore(),#{chkAllRework},#{chkAllRedress});" />
                    </Listeners>
                </ext:Checkbox>
                <ext:ToolbarSeparator />                
                <ext:Checkbox runat="server" ID="chkAllRework" FieldLabel="Set All Rework">
                    <Listeners>
                        <Check Handler="setDefaultRework(this, #{gridDetail}.getStore(),#{chkAllReject},#{chkAllRedress});" />
                    </Listeners>
                </ext:Checkbox>
                <ext:ToolbarSeparator />                
                <ext:Checkbox runat="server" ID="chkAllRedress" FieldLabel="Set All Redress">
                    <Listeners>
                        <Check Handler="setDefaultRedress(this, #{gridDetail}.getStore(),#{chkAllReject},#{chkAllRework});" />
                    </Listeners>
                </ext:Checkbox>
              </Items>
            </ext:Toolbar>
          </TopBar>
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <%--<ext:Parameter Name="model" Value="0053" />--%>
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <%--<ext:Parameter Name="parameters" Value="[['c_rsno = @0', #{hfRSNo}.getValue(), 'System.String']]"
                      Mode="Raw" />--%>
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_rsno" />
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="good" Type="Float" />
                        <ext:RecordField Name="bad" Type="Float" />
                        <ext:RecordField Name="nQty" Type="Float" />
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
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommandsConf(record, toolbar, #{hfRSNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_rsno" Header="N. RS" Width="50" Hidden="true" />
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch " />
                  <ext:NumberColumn DataIndex="good" Header="Good Qty" Format="0.000,00/i" Width="75"
                    Hidden="true">
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="bad" Header="Bad Qty" Format="0.000,00/i" Width="75"
                    Hidden="true">
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="nQty" Header="Qty" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="nReject" Header="Reject" Format="0.000,00/i" Width="75">
                    <Editor>
                      <ext:NumberField runat="server" EmptyText="0.00" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="nRework" Header="Rework" Format="0.000,00/i" Width="75">
                    <Editor>
                      <ext:NumberField runat="server" EmptyText="0.00" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="nRedress" Header="Redress" Format="0.000,00/i" Width="75">
                    <Editor>
                      <ext:NumberField runat="server" EmptyText="0.00" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                <%--<AfterEdit Fn="Test" />--%>
              </Listeners>
            </ext:GridPanel>
          </Items>
          <Buttons>
            <%--<ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
              <DirectEvents>
                <Click>
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>--%>
            <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                  <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini."
                    BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore(), true)"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfRSNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="GudangID" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="GudangDesc" Value="#{cbGudangHdr}.getText()" Mode="Raw" />
                    <ext:Parameter Name="PemasokID" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="PemasokDesc" Value="#{cbPrincipalHdr}.getText()" Mode="Raw" />
                    <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="NoCPR" Value="#{txCPRNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="NoRS1" Value="#{cbNoRefDtl}.getValue()" Mode="Raw" />
                    <%--<ext:Parameter Name="NoRS2" Value="#{cbNoRefDtl}.getValue()" Mode="Raw" />--%>
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:Button ID="Button2" runat="server" Icon="Reload" Text="Bersihkan">
            </ext:Button>
            <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
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
