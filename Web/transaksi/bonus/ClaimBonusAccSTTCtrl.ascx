<%@ Control Language="C#" AutoEventWireup="true" 
CodeFile="ClaimBonusAccSTTCtrl.ascx.cs" Inherits="transaksi_bonus_ClaimBonusAccSTTCtrl" %>

<script language="javascript">

  var selectedItem = function(rec, target1, target2, cbItemDtl) {
    if (Ext.isEmpty(target1)) {
      ShowWarning("Objek target tidak terdefinisi.");
      return;
    }
    if (Ext.isEmpty(target2)) {
      ShowWarning("Objek target tidak terdefinisi.");
      return;
    }

    var recItem = cbItemDtl.findRecord(cbItemDtl.valueField, cbItemDtl.getValue());
    if (Ext.isEmpty(recItem)) {
      ShowWarning(String.format("Record Item '{0}' tidak dapat di baca dari store.", cbBatDtl.getText()));
      return;
    }

    var qtyacc = recItem.get('n_sisa');

    try {
      //target1.setMinValue(0);
      //target2.setMinValue(0);

      if (Ext.isNumber(qtyacc) || Ext.isNumber(qtytolak)) {
        //target1.setMaxValue(qtyacc);
        target1.setValue(qtyacc);
        //target2.setMaxValue(0);
        target2.setValue(0);
      }
      else {
        //target1.setMaxValue(Number.MAX_VALUE);
        target1.setValue(qtyacc);
        //target2.setMaxValue(Number.MAX_VALUE);
        target2.setValue(qtytolak);
      }
    }
    catch (e) {
      ShowError(e.toString());
    }
  }

  var storeToDetailGrid = function(frm, grid, item, qtyacc, qtytolak) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(item) ||
          Ext.isEmpty(qtyacc)) {
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
      var nQtyAcc = qtyacc.getValue();
      var nQtyTolak = qtytolak.getValue();
      var reqQty = 0;
      var itemNo = item.getValue().trim();

      var recItem = item.findRecord(item.valueField, item.getValue());

      store.insert(0, new Ext.data.Record({
        'c_iteno': itemNo,
        'v_itnam': item.getText(),
        'n_qtyacc': nQtyAcc,
        'n_qtytolak': nQtyTolak,
        'l_new': true
      }));

      item.reset();
      qtyacc.reset();
      qtytolak.reset();

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

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfClaimAccNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="125" MaxHeight="125" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="175" Padding="10">
          <Items>
            <ext:TextField runat="server" ID="txNoClaimAcc" MaxLength="30" FieldLabel="No Claim Prinsipal" Width="150"/>
            <ext:DateField Width="180" runat="server" ID="txDayClaimAcc" FieldLabel="Tanggal Prinsipal" Format="dd-MM-yyyy" AllowBlank="false" />
            <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
              MinChars="3" AllowBlank="true" ForceSelection="false">
              <Store>
                <ext:Store runat="server">
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
                <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});clearRelatedComboRecursive(true, #{cbNoClaim});" />
              </Listeners>
            </ext:ComboBox>
            <ext:ComboBox ID="cbNoClaim" runat="server" FieldLabel="No Claim" DisplayField="c_claimno"
            ValueField="c_claimno" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200"
            MinChars="3" AllowBlank="true" ForceSelection="false">
            <Store>
              <ext:Store runat="server">
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
                  <ext:Parameter Name="model" Value="0071" />
                  <ext:Parameter Name="parameters" Value="[['c_nosup = @0', paramValueGetter(#{cbPrincipalHdr}) , 'System.String'],
                    ['c_type = @0', '02' , 'System.String']]" Mode="Raw" />
                  <ext:Parameter Name="sort" Value="c_claimno" />
                  <ext:Parameter Name="dir" Value="ASC" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader IDProperty="c_claimno" Root="d.records" SuccessProperty="d.success"
                    TotalProperty="d.totalRows">
                    <Fields>
                      <ext:RecordField Name="c_claimno" />
                      <ext:RecordField Name="c_nosup" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
              </ext:Store>
            </Store>
            <Template ID="Template1" runat="server">
              <Html>
              <table cellpading="0" cellspacing="0" style="width: 200px">
                <tr><td class="body-panel">No Claim</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_claimno}</td>
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
                        <ext:Store runat="server">
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
                            <ext:Parameter Name="model" Value="0072" />
                            <ext:Parameter Name="parameters" Value="[['claimno', #{cbNoClaim}.getValue(), 'System.String']]"
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
                                <ext:RecordField Name="n_disc" />
                                <ext:RecordField Name="n_sisa" />
                                <ext:RecordField Name="n_qtyacc" Type="Float" />
                                <ext:RecordField Name="n_qtytolak" Type="Float" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
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
                        <Select Handler="selectedItem(record, #{txQtyDtlAcc}, #{txQtyDtlTolak},  #{cbItemDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    
                    <ext:NumberField ID="txQtyDtlAcc" runat="server" FieldLabel="Qty ACC" AllowNegative="true"
                      Width="75" AllowBlank="false" />
                    <ext:NumberField ID="txQtyDtlTolak" runat="server" FieldLabel="Qty Tolak" AllowNegative="true"
                      Width="75" AllowBlank="false" />
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbItemDtl}, #{txQtyDtlAcc},#{txQtyDtlTolak} );" />
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
                    <ext:Parameter Name="model" Value="0073" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_claimaccno = @0', #{hfClaimAccNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="n_qtyacc" Type="Float" />
                        <ext:RecordField Name="n_qtytolak" Type="Float" /> 
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
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfClaimAccNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama Barang" Width="250" />
                  <ext:NumberColumn DataIndex="n_qtyacc" Header="Qty ACC" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_qtytolak" Header="Qty Tolak" Format="0.000,00/i" Width="75" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>
            </ext:GridPanel>
          </Items>
          <Buttons>
            <ext:Button runat="server" Icon="Disk" Text="Simpan">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click">
                  <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfClaimAccNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="NoClaimAcc" Value="#{txNoClaimAcc}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="DayClaimAcc" Value="#{txDayClaimAcc}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="PrincipalId" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="PrincipalDesc" Value="#{cbPrincipalHdr}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="NoClaim" Value="#{cbNoClaim}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="hfStoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
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
