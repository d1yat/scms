<%@ Control Language="C#" AutoEventWireup="true" 
CodeFile="AdjustStockBatch.ascx.cs" 
Inherits="transaksi_penyesuaian_AdjustStockBatch" %>

<script type="text/javascript">
  var prepareCommands = function(rec, toolbar, valX) {
    var del = toolbar.items.get(0); // delete button
    var vd = toolbar.items.get(1); // void button

    var isNew = false;

    if (!Ext.isEmpty(rec)) {
      isNew = rec.get('l_new');
    }

    if (Ext.isEmpty(valX) || isNew) {
      del.setVisible(false);
//      vd.setVisible(false);
    }
    else {
      del.setVisible(false);
//      vd.setVisible(true);
    }
  }
  var onChangeAdjBat = function(o, valu, tGQty, tBQty) {
    var store = o.getStore();

    if ((!Ext.isEmpty(tGQty)) && (!Ext.isEmpty(tBQty)) && (!Ext.isEmpty(store))) {
      var idx = store.findExact('c_batch', valu);
      if (r != -1) {
        var r = store.getAt(idx);

        var gsisa = r.get('n_gsisa');
        var bsisa = r.get('n_bsisa');

        try {
          tGQty.setMinValue(gsisa * -1);
          tBQty.setMinValue(bsisa * -1);
          tGQty.setMaxValue(gsisa);
          tBQty.setMaxValue(bsisa);

          if (Ext.isNumber(gsisa)) {
            tGQty.setValue(gsisa);
          }
          else {
            tGQty.setMaxValue(Number.MAX_VALUE);
            tGQty.setValue(0);
          }
          if (Ext.isNumber(bsisa)) {
            tBQty.setValue(bsisa);
          }
          else {
            tBQty.setMaxValue(Number.MAX_VALUE);
            tBQty.setValue(0);
          }
        }
        catch (e) {
          ShowError(e.toString());
        }
      }
      else {
        tGQty.reset();
        tBQty.reset();
      }
    }
  }

  var storeToDetailGrid = function(frm, grid, item, batchPlus, batchMin, QuantityG, QuantityB, ket) {
      if (!frm.getForm().isValid()) {
          ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
          return;
      }

      if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(batchPlus) ||
          Ext.isEmpty(batchMin)) {
          ShowWarning("Objek tidak terdefinisi.");
          return;
      }

      var store = grid.getStore();
      if (Ext.isEmpty(store)) {
          ShowWarning("Objek store tidak terdefinisi.");
          return;
      }

      var recItem = item.findRecord(item.valueField, item.getValue());
      if (Ext.isEmpty(recItem)) {
          ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", item.getText()));
          return;
      }

      var recBatMin = batchMin.findRecord(batchMin.valueField, batchMin.getValue());
      if (Ext.isEmpty(recBatMin)) {
          ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", batchMin.getText()));
          return;
      }

      var recBatPlus = batchPlus.findRecord(batchPlus.valueField, batchPlus.getValue());
      if (Ext.isEmpty(recBatPlus)) {
          ShowWarning(String.format("Record item '{0}' tidak dapat di baca dari store.", batchPlus.getText()));
          return;
      }
      
      var valX = [item.getValue(), batchMin.getValue()];
      var valX2 = [item.getValue(), batchPlus.getValue()];
      var fieldX = ['c_iteno', 'c_batch'];

      var isDup = findDuplicateEntryGrid(store, fieldX, valX);

      if (!isDup) {
          isDup = findDuplicateEntryGrid(store, fieldX, valX2);
      }

      if (!isDup) {
          var Mgqty = QuantityG.getValue() * -1;
          var Mbqty = QuantityB.getValue() * -1;
          var Pgqty = QuantityG.getValue();
          var Pbqty = QuantityB.getValue();
          var itemNo = item.getValue();
          var keterangan = ket.getValue();
          var PBacth = batchPlus.getValue();
          var MBacth = batchMin.getValue();

          store.insert(0, new Ext.data.Record({
              'c_iteno': itemNo,
              'v_itnam': item.getText(),
              'c_batch': MBacth,
              'n_gqty': Mgqty,
              'n_bqty': Mbqty,
              'v_ket': keterangan,
              'l_new': true
          }));

          store.insert(0, new Ext.data.Record({
              'c_iteno': itemNo,
              'v_itnam': item.getText(),
              'c_batch': PBacth,
              'n_gqty': Pgqty,
              'n_bqty': Pbqty,
              'v_ket': keterangan,
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
      var item = Ext.getCmp('<%= cbItemDtl.ClientID %>'),
      BaMin = Ext.getCmp('<%= cbBatDtlMin.ClientID %>'),
      BaPlus = Ext.getCmp('<%= cbBatDtlPlus.ClientID %>'),
      Good = Ext.getCmp('<%= txGQtyDtl.ClientID %>'),
      Bad = Ext.getCmp('<%= txBQtyDtl.ClientID %>'),
      ket = Ext.getCmp('<%= txKetDtl.ClientID %>');

      if (!Ext.isEmpty(item)) {
          item.reset();
      }
      if (!Ext.isEmpty(BaMin)) {
          BaMin.reset();
          BaMin.store.removeAll();
          BaMin.lastQuery = null;
      }
      if (!Ext.isEmpty(BaPlus)) {
          BaPlus.reset();
          BaPlus.store.removeAll();
          BaPlus.lastQuery = null;
      }
      if (!Ext.isEmpty(Good)) {
          Good.reset();
      }
      if (!Ext.isEmpty(Bad)) {
          Bad.reset();
      }
      if (!Ext.isEmpty(ket)) {
          ket.reset();
      }
      if (!Ext.isEmpty(g)) {
          g.getStore().removeAll();
      }
  }
</script>


<ext:Window ID="winDetail" runat="server" Height="480" Width="1000" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfADJBatchNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="130" MaxHeight="130" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="130" Padding="10">
          <Items>
            <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
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
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[['@contains.v_gdgdesc.Contains(@0) || @contains.c_gdg.Contains(@0)', paramTextGetter(#{cbGudangHdr}), '']]" Mode="Raw" />
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
              <Triggers>
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
              </Listeners>
            </ext:ComboBox>
            <ext:TextField ID="txKeteranganHdr" runat="server" FieldLabel="Keterangan" MaxLengthText="100"
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
                      ValueField="c_iteno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
                      MinChars="3" EmptyText="Pilihan..." AllowBlank="true" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="2061" />
                            <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                            ['l_hide = @0', false, 'System.Boolean'],
                            ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]" Mode="Raw" />
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
                        <table cellpading="0" cellspacing="0" style="width: 400px">
                        <tr>
                        <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                        </tr>
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbBatDtlMin}, #{cbBatDtlPlus});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBatDtlMin" runat="server" FieldLabel="Batch -" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="350" DisplayField="c_batch" ValueField="c_batch" 
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
                            <ext:Parameter Name="model" Value="0077" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{cbGudangHdr}.getValue(), 'System.Char'],
                                                          ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                          ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatDtlMin}), ''],
                                                          ['n_gsisa != 0 || n_bsisa != 0', 0, 'System.Float'],
                                                          ['c_batch != @0', #{cbBatDtlPlus}.getValue(), 'System.String']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_batch" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="n_gsisa" />
                                <ext:RecordField Name="n_bsisa" />
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
                            <td>{n_gsisa}</td>
                            <td>{n_bsisa}</td>
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
                        <Change Handler="onChangeAdjBat(this, newValue, #{txGQtyDtl}, #{txBQtyDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBatDtlPlus" runat="server" FieldLabel="Batch +" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="350" DisplayField="c_batch" ValueField="c_batch"
                      ForceSelection="false" AllowBlank="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store runat="server" >
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0077-1" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{cbGudangHdr}.getValue(), 'System.Char'],
                                                          ['item', #{cbItemDtl}.getValue(), 'System.String'],
                                                          ['@contains.c_batch.Contains(@0)', paramTextGetter(#{cbBatDtlPlus}), ''],
                                                          ['c_batch != @0', #{cbBatDtlMin}.getValue(), 'System.String']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_batch" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="c_batch" />
                                <ext:RecordField Name="n_gsisa" />
                                <ext:RecordField Name="n_bsisa" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template2" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr>
                          <td class="body-panel">Batch</td>
                          <td class="body-panel">Good Qty</td>
                          <td class="body-panel">Bad Qty</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_batch}</td>
                            <td>{n_gsisa}</td>
                            <td>{n_bsisa}</td>
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
                      </Listeners>
                    </ext:ComboBox>
                    <ext:NumberField ID="txGQtyDtl" runat="server" FieldLabel="Good" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:NumberField ID="txBQtyDtl" runat="server" FieldLabel="Bad" AllowNegative="false"
                      Width="75" AllowBlank="false" />
                    <ext:TextField ID="txKetDtl" runat="server" FieldLabel="Keterangan" AllowBlank="true" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{cbBatDtlPlus}, #{cbBatDtlMin}, #{txGQtyDtl}, #{txBQtyDtl}, #{txKetDtl});" />
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
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0075" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_adjno = @0', #{hfADJBatchNo}.getValue(), 'System.String'],
                                                             ['c_type = @0','02','System.String' ]]"
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
                        <ext:RecordField Name="n_bqty" Type="Float" />
                        <ext:RecordField Name="v_ket"  />
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
                      <%--<ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />--%>
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfADJBatchNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="c_batch" Header="Batch " />
                  <ext:NumberColumn DataIndex="n_gqty" Header="Good Qty" Format="0.000,00/i"
                    Width="75">
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_bqty" Header="Bad Qty" Format="0.000,00/i"
                    Width="75">
                   </ext:NumberColumn>
                  <ext:Column DataIndex="v_ket" Header="Keterangan" >
                  </ext:Column>
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
        <Click>
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfADJBatchNo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfADJBatchNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangID" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{cbGudangHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
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
</ext:Window>
