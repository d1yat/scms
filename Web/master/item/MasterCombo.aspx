<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="MasterCombo.aspx.cs" Inherits="master_item_MasterCombo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var onComboRowSelected = function(rec, grid, hf) {
      if (Ext.isEmpty(rec)) {
        return;
      }
      var comboId = rec.get('c_combo');
      if (!Ext.isEmpty(hf)) {
        hf.setValue(comboId);
      }
      if (!Ext.isEmpty(grid)) {
        var stor = grid.getStore();
        if (!Ext.isEmpty(stor)) {
          stor.removeAll();

          showMaskLoad(grid, 'Mohon tunggu...', false);
          stor.reload();
        }
      }
    }
    var onComboRowDeselected = function(grid, hf, frm) {
      if (!Ext.isEmpty(hf)) {
        hf.setValue('');
      }
      if (!Ext.isEmpty(grid)) {
        var stor = grid.getStore();
        if (!Ext.isEmpty(stor)) {
          stor.removeAll();
        }
      }

      if (!Ext.isEmpty(frm)) {
        clearForm(frm);
      }
    }
    var onLoadingProgressDone = function(grid) {
      showMaskLoad(grid, '', true);
    }
    var storeToDetailGrid = function(mainGrid, frm, grid, item, quantity) {
      if (mainGrid.getSelectionModel().getCount() < 1) {
        ShowWarning("Pilih terlebih dahulu combo yang akan ditambah.");
        return;
      }
      else if (!frm.getForm().isValid()) {
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

      var itemNo = item.getValue();

      var valX = [itemNo];
      var fieldX = ['c_iteno'];

      // Find Duplicate entry
      var isDup = findDuplicateEntryGrid(store, fieldX, valX);
      var qty = (quantity.getValue() || 0);

      if (qty <= 0.00) {
        ShowWarning("Jumlah tidak boleh 0.");
        return;
      }

      if (!isDup) {
        var recItem = item.findRecord(item.valueField, itemNo);
        var suplName = (Ext.isEmpty(recItem) ? '' : recItem.get('v_nama'));

        store.insert(0, new Ext.data.Record({
          'c_iteno': itemNo,
          'v_itnam': item.getText(),
          'v_nama_supl': suplName,
          'n_qty': qty,
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
    var prepareCommands = function(rec, toolbar) {
      var del = toolbar.items.get(0); // delete button
      var vd = toolbar.items.get(1); // void button

      var isNew = false;

      if (!Ext.isEmpty(rec)) {
        isNew = rec.get('l_new');
      }

      if (isNew) {
        del.setVisible(true);
        vd.setVisible(false);
      }
      else {
        del.setVisible(false);
        vd.setVisible(true);
      }
    }
    var onGridDetailAfterEdit = function(e) {
      if (e.field == 'n_qty') {
        var isNew = e.record.get('l_new');
        var isVoid = e.record.get('l_void');
        if (isVoid) {
          e.value = e.originalValue;
        }
        else if (!isNew) {
          e.record.set('l_modified', true);
        }
      }
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfComboID" runat="server" />
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:BorderLayout ID="bllayout" runat="server">
        <North MinHeight="250" Collapsible="false" Split="true">
          <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit" Height="300">
            <TopBar>
              <ext:Toolbar ID="Toolbar1" runat="server">
                <Items>
                  <ext:Button ID="Button1" runat="server" Text="Segarkan" Icon="ArrowRefresh">
                    <Listeners>
                      <Click Handler="refreshGrid(#{gridMain});" />
                    </Listeners>
                  </ext:Button>
                </Items>
              </ext:Toolbar>
            </TopBar>
            <Items>
              <ext:GridPanel ID="gridMain" runat="server">
                <LoadMask ShowMask="true" />
                <SelectionModel>
                  <ext:RowSelectionModel SingleSelect="true">
                    <Listeners>
                      <RowSelect Handler="onComboRowSelected(record, #{gridDetail}, #{hfComboID}, #{frmpnlDetailEntry});" />
                      <RowDeselect Handler="onComboRowDeselected(#{gridDetail}, #{hfComboID}, #{frmpnlDetailEntry});" />
                    </Listeners>
                  </ext:RowSelectionModel>
                </SelectionModel>
                <Store>
                  <ext:Store ID="storeGridCombo" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
                    <Proxy>
                      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                        CallbackParam="soaScmsCallback" />
                    </Proxy>
                    <AutoLoadParams>
                      <ext:Parameter Name="start" Value="={0}" />
                      <ext:Parameter Name="limit" Value="={20}" />
                    </AutoLoadParams>
                    <BaseParams>
                      <ext:Parameter Name="start" Value="0" />
                      <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                      <ext:Parameter Name="model" Value="0153" />
                      <ext:Parameter Name="parameters" Value="[['@contains.c_iteno.Contains(@0)', paramRawValueGetter(#{txItemIDFltr}), ''],
                              ['c_nosup = @0', paramValueGetter(#{cbPrincipalFltr}) , 'System.String'],
                              ['@contains.v_itnam.Contains(@0)', paramRawValueGetter(#{txItemNameFltr}), ''],
                              ['c_type = @0', paramValueGetter(#{cbTipeJenisFltr}) , 'System.String'],
                              ['l_combo = @0', paramValueGetter(#{sbIsComboFltr}) , 'System.Boolean'],
                              ['c_via = @0', paramValueGetter(#{cbTipeViaFltr}) , 'System.String']]" Mode="Raw" />
                    </BaseParams>
                    <Reader>
                      <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                        IDProperty="c_combo">
                        <Fields>
                          <ext:RecordField Name="c_nosup" />
                          <ext:RecordField Name="v_nama_supl" />
                          <ext:RecordField Name="c_alkes" />
                          <ext:RecordField Name="c_combo" />
                          <ext:RecordField Name="n_salpri" Type="Float" />
                          <ext:RecordField Name="n_disc" Type="Float" />
                          <ext:RecordField Name="v_itnam" />
                          <ext:RecordField Name="l_status" Type="Boolean" />
                          <ext:RecordField Name="l_combo" Type="Boolean" />
                          <ext:RecordField Name="c_type" />
                          <ext:RecordField Name="Jenis" />
                          <ext:RecordField Name="Via" />
                          <ext:RecordField Name="v_undes" />
                          <ext:RecordField Name="c_itenopri" />
                        </Fields>
                      </ext:JsonReader>
                    </Reader>
                    <SortInfo Field="v_itnam" Direction="ASC" />
                    <Listeners>
                      <BeforeLoad Handler="onComboRowDeselected(#{gridDetail}, #{hfComboID}, #{frmpnlDetailEntry});" />
                      <Clear Handler="onComboRowDeselected(#{gridDetail}, #{hfComboID}, #{frmpnlDetailEntry});" />
                      <Exception Handler="onComboRowDeselected(#{gridDetail}, #{hfComboID}, #{frmpnlDetailEntry});" />
                    </Listeners>
                  </ext:Store>
                </Store>
                <ColumnModel>
                  <Columns>
                    <ext:RowNumbererColumn Width="25" Resizable="false" />
                    <ext:Column ColumnID="c_combo" DataIndex="c_combo" Header="Kode" Width="50" />
                    <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama" Width="200" />
                    <ext:NumberColumn ColumnID="n_salpri" DataIndex="n_salpri" Header="Harga" Format="0.000,00/i" />
                    <ext:NumberColumn ColumnID="n_disc" DataIndex="n_disc" Header="Disc" Format="0.000,00/i" />
                    <ext:Column ColumnID="Jenis" DataIndex="Jenis" Header="Jenis" />
                    <ext:Column ColumnID="Via" DataIndex="Via" Header="Via" />
                    <ext:CheckColumn ColumnID="Combo" DataIndex="l_combo" Header="Combo" Width="50" />
                  </Columns>
                </ColumnModel>
                <View>
                  <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                    <HeaderRows>
                      <ext:HeaderRow>
                        <Columns>
                          <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                            <Component>
                              <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                <Listeners>
                                  <Click Handler="clearFilterGridHeader(#{gridMain}, #{txItemIDFltr}, #{cbPrincipalFltr}, #{txItemNameFltr}, #{cbTipeJenisFltr}, #{cbTipeViaFltr}, #{sbIsComboFltr});reloadFilterGrid(#{gridMain});"
                                    Buffer="300" Delay="300" />
                                </Listeners>
                              </ext:Button>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:TextField ID="txItemIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                  <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                </Listeners>
                              </ext:TextField>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:TextField ID="txItemNameFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                  <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                </Listeners>
                              </ext:TextField>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn />
                          <ext:HeaderColumn />
                          <ext:HeaderColumn>
                            <Component>
                              <ext:ComboBox ID="cbTipeJenisFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                                Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
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
                                <Listeners>
                                  <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                </Listeners>
                              </ext:ComboBox>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:ComboBox ID="cbTipeViaFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                                Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                                AllowBlank="true" ForceSelection="false">
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
                                      <ext:Parameter Name="model" Value="2001" />
                                      <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '02', 'System.String'],
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
                                <Template ID="Template2" runat="server">
                                  <Html>
                                  <table cellpading="0" cellspacing="1" style="width: 200px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                              <tpl for="."><tr class="search-item">
                              <td>{c_type}</td><td>{v_ket}</td>
                              </tr></tpl>
                              </table>
                                  </Html>
                                </Template>
                                <Listeners>
                                  <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                </Listeners>
                              </ext:ComboBox>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:SelectBox ID="sbIsComboFltr" runat="server" ListWidth="50" SelectedIndex="1">
                                <Items>
                                  <ext:ListItem Text="&nbsp;" Value="" />
                                  <ext:ListItem Text="Ya" Value="true" />
                                  <ext:ListItem Text="Tdk" Value="false" />
                                </Items>
                                <Listeners>
                                  <Select Handler="reloadFilterGrid(#{gridMain})" Buffer="100" Delay="100" />
                                </Listeners>
                              </ext:SelectBox>
                            </Component>
                          </ext:HeaderColumn>
                        </Columns>
                      </ext:HeaderRow>
                    </HeaderRows>
                  </ext:GridView>
                </View>
                <BottomBar>
                  <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
                    <Items>
                      <ext:Label ID="Label1" runat="server" Text="Page size:" />
                      <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                      <ext:ComboBox ID="cbGmPagingBB" runat="server" Width="80">
                        <Items>
                          <ext:ListItem Text="5" />
                          <ext:ListItem Text="10" />
                          <ext:ListItem Text="20" />
                          <ext:ListItem Text="50" />
                          <ext:ListItem Text="100" />
                        </Items>
                        <SelectedItem Value="20" />
                        <Listeners>
                          <Select Handler="#{gmPagingBB}.pageSize = parseInt(this.getValue()); #{gmPagingBB}.doLoad();" />
                        </Listeners>
                      </ext:ComboBox>
                    </Items>
                  </ext:PagingToolbar>
                </BottomBar>
              </ext:GridPanel>
            </Items>
          </ext:Panel>
        </North>
        <Center MinHeight="150">
          <ext:Panel ID="pnlDetail" runat="server" Layout="Fit">
            <TopBar>
              <ext:Toolbar ID="Toolbar2" runat="server">
                <Items>
                  <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                    LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                    <Items>
                      <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="250" ItemSelector="tr.search-item"
                        PageSize="10" ListWidth="500" DisplayField="v_itnam" ValueField="c_iteno" AllowBlank="false"
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
                              <ext:Parameter Name="model" Value="0136" />
                              <ext:Parameter Name="parameters" Value="[['(l_hide == null ? false : l_hide) = @0', false , 'System.Boolean'],
                                                          ['l_aktif = @0', true, 'System.Boolean'],
                                                          ['l_combo = @0', false, 'System.Boolean'],
                                                          ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), ''],
                                                          ['(l_delete == null ? false : l_delete) = @0', false , 'System.Boolean']]"
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
                                  <ext:RecordField Name="c_alkes" />
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
                          <td class="body-panel">Pemasok</td><td class="body-panel">Alkes</td>
                        </tr>
                        <tpl for="."><tr class="search-item">
                          <td>{c_iteno}</td><td>{v_itnam}</td>
                          <td>{v_nama}</td><td>{c_alkes}</td>
                        </tr></tpl>
                        </table>
                          </Html>
                        </Template>
                        <Triggers>
                          <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                        </Triggers>
                        <Listeners>
                          <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                          <Change Handler="clearRelatedComboRecursive(true, #{cbSpcDtl}, #{cbBatDtl});" />
                        </Listeners>
                      </ext:ComboBox>
                      <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Quantity" AllowNegative="false"
                        AllowDecimals="true" DecimalPrecision="2" Width="75" AllowBlank="false" />
                      <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                        Icon="Add">
                        <Listeners>
                          <Click Handler="storeToDetailGrid(#{gridMain}, #{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{txQtyDtl});" />
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
                  <ext:Store ID="Store5" runat="server" RemotePaging="false" RemoteSort="false">
                    <Proxy>
                      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                        CallbackParam="soaScmsCallback" />
                    </Proxy>
                    <BaseParams>
                      <ext:Parameter Name="start" Value="0" />
                      <ext:Parameter Name="limit" Value="-1" />
                      <ext:Parameter Name="allQuery" Value="true" />
                      <ext:Parameter Name="model" Value="0154" />
                      <ext:Parameter Name="sort" Value="" />
                      <ext:Parameter Name="dir" Value="" />
                      <ext:Parameter Name="parameters" Value="[['comboItem', #{hfComboID}.getValue(), 'System.String']]"
                        Mode="Raw" />
                    </BaseParams>
                    <Reader>
                      <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                        IDProperty="c_iteno">
                        <Fields>
                          <ext:RecordField Name="c_iteno" />
                          <ext:RecordField Name="v_itnam" />
                          <ext:RecordField Name="n_qty" Type="Float" />
                          <ext:RecordField Name="v_acronim" />
                          <ext:RecordField Name="v_nama_supl" />
                          <ext:RecordField Name="l_new" Type="Boolean" />
                          <ext:RecordField Name="l_void" Type="Boolean" />
                          <ext:RecordField Name="l_modified" Type="Boolean" />
                          <ext:RecordField Name="v_ket" />
                        </Fields>
                      </ext:JsonReader>
                    </Reader>
                    <Listeners>
                      <Load Handler="onLoadingProgressDone(#{gridDetail});" />
                      <Exception Handler="onLoadingProgressDone(#{gridDetail});" />
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
                      <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                    </ext:CommandColumn>
                    <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                    <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                    <ext:NumberColumn DataIndex="n_qty" Header="Qty" Width="75" Format="0.000,00/i" Editable="true">
                      <Editor>
                        <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                          DecimalPrecision="2" MinValue="0.01" />
                      </Editor>
                    </ext:NumberColumn>
                    <ext:Column DataIndex="v_nama_supl" Header="Pemasok" />
                    <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                  </Columns>
                </ColumnModel>
                <Listeners>
                  <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                  <AfterEdit Handler="onGridDetailAfterEdit(e);" />
                </Listeners>
              </ext:GridPanel>
            </Items>
            <Buttons>
              <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
                <DirectEvents>
                  <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(null, #{gridDetail});">
                    <Confirmation BeforeConfirm="return verifyHeaderAndDetail(null, #{gridDetail});"
                      ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                      <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore(), true)"
                        Mode="Raw" />
                      <ext:Parameter Name="NumberID" Value="#{hfComboID}.getValue()" Mode="Raw" />
                    </ExtraParams>
                  </Click>
                </DirectEvents>
              </ext:Button>
              <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
                <Listeners>
                  <Click Handler="onComboRowSelected(#{gridMain}.getSelectionModel().getSelected(), #{gridDetail});" />
                </Listeners>
              </ext:Button>
            </Buttons>
          </ext:Panel>
        </Center>
      </ext:BorderLayout>
    </Items>
  </ext:Viewport>
</asp:Content>
