<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DiscountCtrl.ascx.cs"
  Inherits="keuangan_discount_DiscountCtrl" %>

<script language="javascript" type="text/javascript">

  var storeToDetailGrid = function(frm, grid, discOn, discOff, datAwal, datAkhir) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(discOn) ||
          Ext.isEmpty(discOff) ||
          Ext.isEmpty(datAwal) ||
          Ext.isEmpty(datAkhir)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var dateAwal = datAwal.getValue();
    var dateAkhir = datAkhir.getValue();

    var valX = [dateAwal, dateAkhir, true];
    var fieldX = ['d_start', 'd_finish', 'l_aktif'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      store.insert(0, new Ext.data.Record({
        'n_discon': discOn.getValue(),
        'n_discoff': discOff.getValue(),
        'd_start': dateAwal,
        'd_finish': dateAkhir,
        'l_aktif': true,
        'l_new': true,
        'l_void': false,
        'l_modified': false
      }));

      discOn.reset();
      discOff.reset();
      datAwal.reset();
      datAkhir.reset();
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  var prepareCommands = function(rec, toolbar) {
    var nona = toolbar.items.get(0); // NonAktif button
    var del = toolbar.items.get(1); // delete button
    var vd = toolbar.items.get(2); // void button

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

  var cmdListenerGrid = function(grid, cmd, rec) {
    switch (cmd) {
      case 'Delete':
        deleteRecordOnGrid(grid, rec);
        break;
      case 'Void':
        voidInsertedDataFromStore(rec);
        break;
      case 'NonAktif':
        //voidInsertedDataFromStore(this, record);
        if (rec.get('l_new')) {
          deleteRecordOnGrid(grid, rec);
        }
        else {
          rec.set('l_aktif', (!rec.get('l_aktif')));
          rec.set('l_modified', true);
        }
        break;
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfDiscNo" runat="server" />
    <ext:Hidden ID="hfItemNo" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="100" MaxHeight="150" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="150" AutoScroll="true"
          Layout="Fit" Padding="5">
          <Items>
            <ext:Hidden ID="hfIdCode" runat="server" />
            <ext:Label ID="lbItem" runat="server" FieldLabel="Item" />
            <ext:NumberField ID="txDiscOnHdr" runat="server" AllowBlank="false" FieldLabel="Discont On"
              AllowNegative="false" AllowDecimals="true" DecimalPrecision="2" MinValue="0" MaxValue="100" />
            <ext:NumberField ID="txDiscOffHdr" runat="server" AllowBlank="false" FieldLabel="Discont Off"
              AllowNegative="false" AllowDecimals="true" DecimalPrecision="2" MinValue="0" MaxValue="100" />
            <ext:CompositeField runat="server" FieldLabel="Periode">
              <Items>
                <ext:DateField ID="txTglAwalHdr" runat="server" AllowBlank="false" Format="dd-MM-yyyy"
                  Vtype="daterange">
                  <CustomConfig>
                    <ext:ConfigItem Name="endDateField" Value="#{txTglAkhirHdr}" Mode="Value" />
                  </CustomConfig>
                </ext:DateField>
                <ext:Label runat="server" Text=" - " />
                <ext:DateField ID="txTglAkhirHdr" runat="server" AllowBlank="false" Format="dd-MM-yyyy"
                  Vtype="daterange">
                  <CustomConfig>
                    <ext:ConfigItem Name="startDateField" Value="#{txTglAwalHdr}" Mode="Value" />
                  </CustomConfig>
                </ext:DateField>
              </Items>
            </ext:CompositeField>
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
                    <ext:NumberField ID="txDiscOnDtl" runat="server" AllowBlank="false" FieldLabel="Discont On"
                      AllowNegative="false" AllowDecimals="true" DecimalPrecision="2" MinValue="0" MaxValue="100"
                      Width="50" />
                    <ext:NumberField ID="txDiscOffDtl" runat="server" AllowBlank="false" FieldLabel="Discont Off"
                      AllowNegative="false" AllowDecimals="true" DecimalPrecision="2" MinValue="0" MaxValue="100"
                      Width="50" />
                    <ext:DateField ID="txTglAwalDtl" runat="server" FieldLabel="Mulai" AllowBlank="false"
                      Format="dd-MM-yyyy" Vtype="daterange">
                      <CustomConfig>
                        <ext:ConfigItem Name="endDateField" Value="#{txTglAkhirDtl}" Mode="Value" />
                      </CustomConfig>
                    </ext:DateField>
                    <ext:DateField ID="txTglAkhirDtl" runat="server" FieldLabel="Selesai" AllowBlank="false"
                      Format="dd-MM-yyyy" Vtype="daterange">
                      <CustomConfig>
                        <ext:ConfigItem Name="startDateField" Value="#{txTglAwalDtl}" Mode="Value" />
                      </CustomConfig>
                    </ext:DateField>
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{txDiscOnDtl}, #{txDiscOffDtl}, #{txTglAwalDtl}, #{txTglAkhirDtl});" />
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
                    <ext:Parameter Name="model" Value="0113" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['noDiscount', #{hfDiscNo}.getValue(), 'System.String'],
                          ['noItem', #{hfItemNo}.getValue(), 'System.String'],
                          ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="IDX" Type="Int" />
                        <ext:RecordField Name="n_discon" Type="Float" />
                        <ext:RecordField Name="n_discoff" Type="Float" />
                        <ext:RecordField Name="d_start" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="d_finish" Type="Date" DateFormat="M$" />
                        <ext:RecordField Name="l_aktif" Type="Boolean" />
                        <ext:RecordField Name="v_keterangan" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="v_ket" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="50">
                    <Commands>
                      <ext:GridCommand CommandName="NonAktif" Icon="KeyDelete" ToolTip-Title="Command"
                        ToolTip-Text="Non aktif" />
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                  </ext:CommandColumn>
                  <ext:NumberColumn DataIndex="n_discon" Header="On" Format="0.000,00/i" Width="50" />
                  <ext:NumberColumn DataIndex="n_discoff" Header="Off" Format="0.000,00/i" Width="50" />
                  <ext:DateColumn DataIndex="d_start" Header="Mulai" Format="dd-MM-yyyy" />
                  <ext:DateColumn DataIndex="d_finish" Header="Selesai" Format="dd-MM-yyyy" />
                  <ext:CheckColumn DataIndex="l_aktif" Header="Aktif" Width="50" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <%--<BottomBar>
                <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
                  <Items>
                    <ext:Label runat="server" Text="Page size:" />
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
              </BottomBar>--%>
              <Listeners>
                <Command Handler="cmdListenerGrid(this, command, record);" />
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfIdCode}.getValue()" Mode="Raw" />
            <ext:Parameter Name="DiscountID" Value="#{hfDiscNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="ItemID" Value="#{hfItemNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="storeValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="DiscOn" Value="#{txDiscOnHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="DiscOff" Value="#{txDiscOffHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TanggalAwal" Value="#{txTglAwalHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TanggalAkhir" Value="#{txTglAkhirHdr}.getValue()" Mode="Raw" />
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
