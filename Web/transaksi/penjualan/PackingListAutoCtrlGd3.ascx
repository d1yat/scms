<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PackingListAutoCtrlGd3.ascx.cs"
  Inherits="transaksi_penjualan_PackingListAutoCtrlGd3" %>

<script type="text/javascript">
  var storeToDetailGridAuto = function(frm, grid, rnDo, Item, SpNo, Bat, Qty) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(Item) ||
          Ext.isEmpty(SpNo) ||
          Ext.isEmpty(Bat) ||
          Ext.isEmpty(Qty) ||
          Ext.isEmpty(rnDo)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var noRnDo = rnDo.getValue();
    var valX = [noRnDo, Item.getValue(), Bat.getValue(), SpNo.getValue()];
    var fieldX = ['c_rnno', 'c_iteno', 'c_batch', 'c_spno'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var KdItem = Item.getValue();
      var KdSpNo = SpNo.getValue();
      var KdSpNoCab = SpNo.getText();
      var Batch = Bat.getValue();

      var recSpNo = SpNo.findRecord(SpNo.valueField, SpNo.getValue());

      var sisaSP = recSpNo.get('n_spsisa');

      var QtyRN = Qty.getValue();
      var QtyAdj = (QtyRN - sisaSP);


      if (QtyAdj < 0) {
        QtyAdj = QtyAdj * 0;
      }

      store.insert(0, new Ext.data.Record({
        'c_rnno': noRnDo,
        'c_iteno': KdItem,
        'v_itnam': Item.getText(),
        'c_batch': Batch,
        'n_qtyrn': QtyRN,
        'c_sp': KdSpNoCab,
        'c_spno': KdSpNo,
        'c_typesp': "01",
        'n_qtysp': sisaSP,
        'n_qtysp_adj': 0,
        'l_new': true
      }));

      resetEntryWhenChangeInt();

      /*
      item.reset();
      quantity.reset();
      nomorPo.reset();
      batch.reset();
      tglbatch.reset();
      */
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }
  var resetEntryWhenChangeInt = function(g) {
    var cbItemDtl = Ext.getCmp('<%= cbItemDtl.ClientID %>'),
      cbSpcDtl = Ext.getCmp('<%= cbSpcDtl.ClientID %>'),
      cbBatDtl = Ext.getCmp('<%= cbBatDtl.ClientID %>'),
      txQtyDtl = Ext.getCmp('<%= txQtyDtl.ClientID %>')

    if (!Ext.isEmpty(cbItemDtl)) {
      cbItemDtl.reset();
    }
    if (!Ext.isEmpty(cbSpcDtl)) {
      cbSpcDtl.reset();
    }
    if (!Ext.isEmpty(cbBatDtl)) {
      cbBatDtl.reset();
    }
    if (!Ext.isEmpty(txQtyDtl)) {
      txQtyDtl.reset();
    }
    if (!Ext.isEmpty(g)) {
      g.getStore().removeAll();
    }
  }
  //  var prepareCommandsAuto = function(rec, toolbar, valX) {
  //    var del = toolbar.items.get(0); // delete button
  //    var vd = toolbar.items.get(1); // void button

  //    var isNew = false;

  //    if (!Ext.isEmpty(rec)) {
  //      isNew = rec.get('l_new');
  //    }

  //    if (Ext.isEmpty(valX) || isNew) {
  //      del.setVisible(true);
  //      vd.setVisible(false);
  //    }
  //    else {
  //      del.setVisible(false);
  //      vd.setVisible(true);
  //    }
  //  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="850" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudangAuto" runat="server" />
    <ext:Hidden ID="hfGudangDescAuto" runat="server" />
    <%--<ext:Hidden ID="hfPlNo" runat="server" />--%>
    <ext:Hidden ID="hfStoreIDAuto" runat="server" />
    <%--<asp:HiddenField ID="hfStoreIDAuto" runat="server" />--%>
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="100" MaxHeight="100" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="235" Padding="10"
          ButtonAlign="Center">
          <Items>
            <ext:ComboBox ID="cbTipe" runat="server" FieldLabel="Tipe" 
            Width="125" AllowBlank="false" ForceSelection="false">
              <SelectedItem Text="Auto" Value="02" />
              <Items>
                <ext:ListItem Text="Auto" Value="02" />
                <ext:ListItem Text="Cross Docking" Value="06" />
              </Items>
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                <Change Handler="clearRelatedComboRecursive(true, #{cbSuplierHdr});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
              ValueField="c_type" Width="125" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store2" runat="server" RemotePaging="false">
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
            <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
              ValueField="c_cusno" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
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
                    <ext:Parameter Name="model" Value="2011" />
                    <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbNoDoKhususHdr});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbSuplierHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="225" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
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
                    <ext:Parameter Name="model" Value="2021-a" />
                    <ext:Parameter Name="parameters" Value="[['q.l_hide = @0', false, 'System.Boolean'],
                          ['q.l_aktif = @0', true, 'System.Boolean'],
                          ['@contains.q.c_nosup.Contains(@0) || @contains.q.v_nama.Contains(@0)', paramTextGetter(#{cbSuplierHdr}), ''],
                          ['(qDSA.l_pl == @0) && (qDSA.l_do == @0)', true, 'System.Boolean'],
                          ['TipePL', #{cbTipe}.getValue(), 'System.String']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="q.v_nama" />
                    <ext:Parameter Name="dir" Value="ASC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="q.c_nosup" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_nosup" Mapping="q.c_nosup" />
                        <ext:RecordField Name="v_nama" Mapping="q.v_nama" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template2" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 350px">
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
                <Change Handler="clearRelatedComboRecursive(true, #{cbNoDoKhususHdr});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbNoDoKhususHdr" runat="server" FieldLabel="No Receive" DisplayField="c_dono"
              ValueField="c_rnno" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
              MinChars="3" AllowBlank="false" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
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
                    <ext:Parameter Name="model" Value="0138" />
                    <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudangAuto}.getValue(), 'System.Char'],
                        ['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                        ['nosup', #{cbSuplierHdr}.getValue(), 'System.String'],
                        ['@contains.c_rnno.Contains(@0) || @contains.c_dono.Contains(@0)', paramTextGetter(#{cbNoDoKhusus}), 'System.String']]"
                      Mode="Raw" />
                    <ext:Parameter Name="sort" Value="d_rndate" />
                    <ext:Parameter Name="dir" Value="DESC" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                      TotalProperty="d.totalRows">
                      <Fields>
                        <ext:RecordField Name="c_rnno" />
                        <ext:RecordField Name="d_rndate" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="c_dono" />
                        <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template ID="Template6" runat="server">
                <Html>
                <table cellpading="0" cellspacing="1" style="width: 400px">
                <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td>
                <td class="body-panel">Delivery</td><td class="body-panel">Tanggal</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_rnno}</td><td>{d_rndate:this.formatDate}</td>
                    <td>{c_dono}</td><td>{d_dodate:this.formatDate}</td>
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
                <Change Handler="resetEntryWhenChangeInt(#{gridDetail});clearRelatedComboRecursive(true, #{cbItemDtl});" />
              </Listeners>
            </ext:ComboBox>
            <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="60" Width="250" />
          </Items>
          <Buttons>
            <ext:Button runat="server" Text="Proses" Icon="CogStart">
              <DirectEvents>
                <Click OnEvent="ProcessPL_Click" Before="return validasiProses(#{frmHeaders});">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="Gudang" Value="#{hfGudangAuto}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Customer" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Via" Value="#{cbViaHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="noRN" Value="#{cbNoDoKhususHdr}.getValue()" Mode="Raw" />
                    <%--<ext:Parameter Name="StoreID" Value="#{hfStoreIDAuto}.getValue()" Mode="Raw" />--%>
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
        </ext:FormPanel>
      </North>
      <Center>
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Packing List Auto" Height="150"
          Layout="Fit">
          <TopBar>
            <ext:Toolbar runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="350" DisplayField="v_itnam" ValueField="c_iteno" ForceSelection="false"
                      AllowBlank="true">
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
                            <ext:Parameter Name="model" Value="0130" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudangAuto}.getValue(), 'System.Char'],
                                                          ['NoRN', #{cbNoDoKhususHdr}.getValue(), 'System.String'],
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
                                <ext:RecordField Name="v_nama" />
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
                          <%--<td class="body-panel">Jumlah Qty SP Tertunda</td><td class="body-panel">Stok Gudang</td>--%>
                        </tr>
                        <tpl for="."><tr class="search-item">
                          <td>{c_iteno}</td><td>{v_itnam}</td>
                          <%--<td>{n_SumPending}</td><td>{n_soh}</td>--%>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbSpcDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbSpcDtl" runat="server" FieldLabel="SP Cabang" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="450" DisplayField="c_sp" ValueField="c_spno" AllowBlank="true"
                      ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="0129" />
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                                            ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                            ['@contains.c_spno.Contains(@0)', paramTextGetter(#{cbSpcDtl}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_spdate" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_spno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_spno" />
                                <ext:RecordField Name="c_sp" />
                                <ext:RecordField Name="d_spdate" DateFormat="M$" Type="Date" />
                                <ext:RecordField Name="v_SpType" />
                                <ext:RecordField Name="n_spsisa" Type="Float" />
                                <ext:RecordField Name="n_spqty" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template4" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 450px">
                          <tr>
                            <td class="body-panel">No SP</td><td class="body-panel">SP Cabang</td>
                            <td class="body-panel">Tanggal</td>
                            <td class="body-panel">Sisa</td><td class="body-panel">Qty</td>
                          </tr>
                          <tpl for="."><tr class="search-item">
                              <td>{c_spno}</td><td>{c_sp}</td>
                              <td>{d_spdate:this.formatDate}</td>
                              <td>{n_spsisa}</td><td>{n_spqty}</td>
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbBatDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:Hidden ID="hfTypDtl" runat="server" Text="01" />
                    <ext:ComboBox ID="cbBatDtl" runat="server" FieldLabel="Batch" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="350" DisplayField="c_batch" ValueField="c_batch" AllowBlank="true"
                      ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="0131" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGudangAuto}.getValue(), 'System.Char'],
                                ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                ['NoRN', #{cbNoDoKhususHdr}.getValue(), 'System.String'],
                                ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatDtl}), '']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="d_expired" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_batch" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="d_expired" DateFormat="M$" Type="Date" />
                                <ext:RecordField Name="n_qtybatch" Type="Float" />
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
                            <td class="body-panel">Jumlah</td>
                          </tr>
                          <tpl for="."><tr class="search-item">
                              <td>{c_batch}</td>
                              <td>{d_expired:this.formatDate}</td>
                              <td>{n_qtybatch:this.formatNumber}</td>
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
                        <Select Handler="selectedItemBatch(this, record, #{txQtyDtl}, #{cbItemDtl}, #{cbSpcDtl})" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Quantity" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGridAuto(#{frmpnlDetailEntry}, #{gridDetail}, #{cbNoDoKhususHdr}, #{cbItemDtl}, #{cbSpcDtl}, #{cbBatDtl}, #{txQtyDtl});" />
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
                <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false">
                  <%--<Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>--%>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0128" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <%--<ext:Parameter Name="parameters" Value="[['c_plno = @0', #{hfPlNo}.getValue(), 'System.String']]"
                      Mode="Raw" />--%>
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_rnno" />
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_qtyrn" Type="Float" />
                        <ext:RecordField Name="c_sp" />
                        <ext:RecordField Name="c_spno" />
                        <ext:RecordField Name="c_typesp" />
                        <ext:RecordField Name="n_qtysp" Type="Float" />
                        <ext:RecordField Name="n_qtysp_adj" Type="Float" />
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
                    <%--<PrepareToolbar Handler="prepareCommandsAuto(record, toolbar, #{hfPlNo}.getValue());" />--%>
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch" Width="120" />
                  <ext:NumberColumn DataIndex="n_qtyrn" Header="Qty RN" Format="0.000,00/i" Width="75" />
                  <ext:Column DataIndex="c_sp" Header="SP Cabang" />
                  <ext:NumberColumn DataIndex="n_qtysp" Header="Qty SP" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_qtysp_adj" Header="Adjustment" Format="0.000,00/i"
                    Width="75" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); }" />
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="btnSimpan" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini."
            BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <%--<ext:Parameter Name="StoreID" Value="#{hfStoreIDAuto}.getValue()" Mode="Raw" />--%>
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnKeluar" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
