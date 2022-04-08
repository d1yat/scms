<%@ Control Language="C#" AutoEventWireup="true" 
CodeFile="ClaimBonusSTTCtrl.ascx.cs" Inherits="transaksi_bonus_ClaimBonusSTTCtrl" %>

<script language="javascript">

  var storeToDetailGrid = function(frm, grid, item, qty) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(qty)) {
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

    var isDup = false;
    var nDup = 0;

    // Find Duplicate entry
    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (nDup == valX.length) {
      isDup = true;
    }

    if (!isDup) {
      var quantity = qty.getValue();
      var reqQty = 0;
      var itemNo = item.getValue().trim();

      var recItem = item.findRecord(item.valueField, item.getValue());
      var disc = recItem.get('n_disc');
      var price = recItem.get('n_salpri');

      store.insert(0, new Ext.data.Record({
        'n_disc': disc,
        'n_salpri': price,
        'n_net': (price * quantity) - ((price * quantity) * (disc / 100)),
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'n_qty': quantity,
        'l_new': true
      }));

      item.reset();
      qty.reset();

    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }
  var OnAfter = function(e) {
    if (e.value !== e.originalValue) {
      if (e.value == 0) {
        ShowError('Data tidak boleh 0.');
        return false;
      } else {
        e.record.set('l_modified', true);
        var Salpri = e.record.get('n_salpri');
        var disc = e.record.get('n_disc');

        var nHasil = (Salpri * e.value) - (Salpri * e.value * (disc / 100));

        e.record.set('n_net', nHasil);
      }
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
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfClaimNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="125" MaxHeight="125" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="185" Padding="10">
          <Items>
            <ext:CompositeField runat="server" FieldLabel="Periode">
                <Items>
                  <ext:SelectBox ID="txYear" runat="server" Width="75" AllowBlank="false" FieldLabel="Tahun" />
                  <ext:SelectBox ID="cbMonthHdr" runat="server" AllowBlank="false" FieldLabel="Bulan"/>
                </Items>
              </ext:CompositeField>
            <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="true" ForceSelection="false">
              <Store>
                <ext:Store ID="Store3" runat="server">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig> 
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
                                ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]" Mode="Raw" />
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
                <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});clearRelatedComboRecursive(true, #{cbItemDtl});" />
              </Listeners>
            </ext:ComboBox>
            <ext:NumberField runat="server" ID="txTop" MaxLength="3" FieldLabel="Top" Width="100"/>
 
            <ext:CompositeField runat="server" FieldLabel="Kurs">
                <Items>
                  <ext:ComboBox ID="cbKurs" runat="server" FieldLabel="Kurs" DisplayField="c_desc"
                    ValueField="c_kurs" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200"
                    MinChars="3" AllowBlank="true" ForceSelection="false">
                    <Store>
                      <ext:Store ID="Store2" runat="server">
                        <CustomConfig>
                          <ext:ConfigItem Name="allowBlank" Value="true" />
                        </CustomConfig>
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={10}" />
                          <ext:Parameter Name="model" Value="2072" />
                          <ext:Parameter Name="parameters" Value="[['@contains.c_desc.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]" Mode="Raw" />
                          <ext:Parameter Name="sort" Value="c_desc" />
                          <ext:Parameter Name="dir" Value="ASC" />
                        </BaseParams>
                        <Reader>
                          <ext:JsonReader IDProperty="c_desc" Root="d.records" SuccessProperty="d.success"
                            TotalProperty="d.totalRows">
                            <Fields>
                              <ext:RecordField Name="c_desc" />
                              <ext:RecordField Name="c_kurs" />
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                      </ext:Store>
                    </Store>
                    <Template ID="Template1" runat="server">
                      <Html>
                      <table cellpading="0" cellspacing="0" style="width: 200px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Kurs</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_desc}</td><td>{c_kurs}</td>
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
                  <ext:Label ID="Label1" runat="server" Text="-"></ext:Label>
                  <ext:TextField ID="txKurs" runat="server" MaxLength="5">
                  </ext:TextField>
                </Items>
            </ext:CompositeField>
           
           <ext:TextField runat="server" ID="txKeterangan" FieldLabel="Keterangan" Width="300" />
           
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
                  
                    <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" Width="200" ItemSelector="tr.search-item"
                      PageSize="10" ListWidth="400" DisplayField="v_itnam" ValueField="c_iteno" EmptyText="Pilihan..."
                      AllowBlank="true" ForceSelection="false">
                      <Store>
                        <ext:Store ID="Store4" runat="server">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="true" />
                          </CustomConfig>
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="0065" />
                            <ext:Parameter Name="parameters" Value="[['supl', #{cbPrincipalHdr}.getValue(), 'System.String'],
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
                                <ext:RecordField Name="n_salpri" Type="Float" />
                                <ext:RecordField Name="n_discon" />
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
                      </Listeners>
                    </ext:ComboBox>
                    
                    <ext:NumberField ID="txQtyDtl" runat="server" FieldLabel="Qty" AllowNegative="true"
                      Width="75" AllowBlank="false" />
                      
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
                <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false" >
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0063" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_claimno = @0', #{hfClaimNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="n_salpri" Type="Float" />
                        <ext:RecordField Name="n_disc" Type="Float" />
                        <ext:RecordField Name="n_net" Type="Float" />
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
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfClaimNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Qty" Format="0.000,00/i" Width="75" >
                    <Editor>
                      <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="true" DecimalPrecision="2" MinValue="0" />
                    </Editor>  
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_salpri" Header="Price" Format="0.000,00/i"
                    Width="75" />
                  <ext:NumberColumn DataIndex="n_disc" Header="Disc" Format="0.000,00/i"
                    Width="75" />
                  <ext:NumberColumn DataIndex="n_net" Header="Net" Format="0.000,00/i"
                    Width="75" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                  <ext:CheckColumn DataIndex="l_modified" Header="Modif" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners> 
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                <AfterEdit Fn="OnAfter" />
              </Listeners>
            </ext:GridPanel>
           </Items>
           <Buttons>
            <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click">
                  <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfClaimNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="SuplierDesc" Value="#{cbPrincipalHdr}.getText()" Mode="Raw" />
                    <ext:Parameter Name="SuplierId" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Top" Value="#{txTop}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="KursTipe" Value="#{cbKurs}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="KursVal" Value="#{txKurs}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Year" Value="#{txYear}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Month" Value="#{cbMonthHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:Button ID="Button2" runat="server" Icon="Reload" Text="Bersihkan">
              <DirectEvents>
                <Click OnEvent="ReloadBtn_Click">
                  <EventMask ShowMask="true" />
                </Click>
              </DirectEvents>
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