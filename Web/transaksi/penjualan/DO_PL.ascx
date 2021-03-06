<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DO_PL.ascx.cs" Inherits="transaksi_penjualan_DO_PL" %>
<%@ Register Src="DO_PL_Print.ascx" TagName="DO_PL_Print" TagPrefix="uc2" %>

<script type="text/javascript">

  var recOnGrid = function(grid, lbl, lblTotal) {
    var s = grid.getStore().getCount();
    var storeGrid = grid.getStore();
    var ii = 0;
    var iNtotal = 0;
    for (ii; ii < storeGrid.data.length; ii++) {
      var iSp = storeGrid.data.items[ii].data.n_sisa;
      iNtotal += iSp;
    };

    lbl.setText(s);
    lblTotal.setValue(iNtotal);
  }
    
  var prepareCommandsDoPL = function(toolbar, valX) {
    var del = toolbar.items.get(0); // delete button
    var vd = toolbar.items.get(1); // void button

    

    if (Ext.isEmpty(valX)) {
      del.setVisible(true);
      vd.setVisible(false);
    }
    else {
      del.setVisible(false);
      vd.setVisible(true);
    }
  };

  var voidPLDataFromStoreDoPL = function(rec, dm) {
    if (Ext.isEmpty(rec)) {
      return;
    }
    ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
            function(btn) {
              if (btn == 'yes') {
                ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
                  function(btnP, txt) {
                    if (btnP == 'ok') {
                      if (txt.trim().length < 1) {
                        txt = 'Kesalahan pemakai.';
                      }
                      dm.DeleteMethod(rec.get('c_dono'), txt);
                    }
                  });
              }
            });
  }

  var voidInsertedDataFromStoreDoPL = function(rec) {
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

  var prepareCommandsParentDoPL = function(record, toolbar) {
    var accp = toolbar.items.get(0); // accept button

    var isSubmitDO = false;

    if (!Ext.isEmpty(record)) {
      isSubmitDO = record.get('l_sent');
    }

    if (isSubmitDO) {
      accp.setVisible(false);
    }
    else {
      accp.setVisible(true);
    }
  }
  
  var submitDODataToFJDoPL = function(rec, dm) {
    if (Ext.isEmpty(rec)) {
      return;
    }

    ShowConfirm('Kirim ?', 'Apakah anda yakin ingin memproses nomor ini ?',
        function(btn) {
          if (btn == 'yes') {
            dm.SubmitMethod(rec.get('c_dono'));
          }
        });
  }
</script>

<ext:Viewport runat="server" Layout="Fit">
  <Items>
    <ext:Panel runat="server" Layout="Fit">
      <Content>
        <ext:Hidden ID="hfDONo" runat="server" />
        <ext:Hidden ID="hfGudang" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
      </Content>
      <TopBar>
        <ext:Toolbar runat="server">
          <Items>
            <ext:Button ID="btnAddNew" runat="server" Text="Tambah" Icon="Add">
              <DirectEvents>
                <Click OnEvent="btnAddNew_OnClick">
                  <EventMask ShowMask="true" />
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:ToolbarSeparator />
            <ext:Button ID="btnPrintDO" runat="server" Text="Cetak" Icon="Printer">
              <DirectEvents>
                <Click OnEvent="btnPrintDO_OnClick">
                  <EventMask ShowMask="true" />
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:ToolbarSeparator />
            <ext:Button ID="Button3" runat="server" Text="Segarkan" Icon="ArrowRefresh">
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
          <Listeners>
            <Command Handler="if(command == 'Delete') { voidPLDataFromStoreDoPL(record, #{DirectMethods}); } else if(command == 'Submit') { submitDODataToFJDoPL(record, #{DirectMethods}); }" />
          </Listeners>
          <DirectEvents>
            <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                <ext:Parameter Name="Parameter" Value="c_dono" />
                <ext:Parameter Name="PrimaryID" Value="record.data.c_dono" Mode="Raw" />
                <ext:Parameter Name="GudangID" Value="#{hfGudang}.getValue()" Mode="Raw" />
              </ExtraParams>
            </Command>
          </DirectEvents>
          <SelectionModel>
            <ext:RowSelectionModel SingleSelect="true" />
          </SelectionModel>
          <Store>
            <ext:Store ID="storeGridPL" runat="server" SkinID="OriginalExtStore" RemotePaging="true"
              RemoteSort="true">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <AutoLoadParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={20}" />
              </AutoLoadParams>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={20}" />
                <ext:Parameter Name="model" Value="0007" />
                <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                <ext:Parameter Name="parameters" Value="[['c_dono', paramValueGetter(#{txNoFltr}) + '%', ''],
                                  ['d_dodate = @0', paramRawValueGetter(#{txDODate}) , 'System.DateTime'],
                                  ['c_cusno = @0', paramValueGetter(#{cbCustomer}) , 'System.String'],
                                  ['c_type = @0', paramValueGetter(#{cbViaHdr}) , 'System.String'],
                                  ['c_plno' , paramValueGetter(#{txNoPLFltr}) + '%',''],
                                  ['c_expno' , paramValueGetter(#{txNoExpFltr}) + '%',''],
                                  ['c_gdg = @0', paramValueGetter(#{hfGudang}) , 'System.Char'],
                                  ['@contains.c_entry.Contains(@0)', paramValueGetter(#{txNipFltr}),'System.String'],
                                  ['c_pin' , paramValueGetter(#{txPin}) + '%',''],
                                  ['ETDPL = @0', paramRawValueGetter(#{txETDDate}) , 'System.DateTime'],
                                  ['l_sent = @0', paramValueGetter(#{sbCekSend}) , 'System.Boolean']]"                                  
                                    Mode="Raw" />
                            </BaseParams>
                            <Reader>
                                <ext:JsonReader IDProperty="c_dono" Root="d.records" SuccessProperty="d.success"
                                    TotalProperty="d.totalRows">
                                    <Fields>
                                        <ext:RecordField Name="c_dono" />
                                        <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                                        <ext:RecordField Name="v_gdgdesc" />
                                        <ext:RecordField Name="v_cunam" />
                                        <ext:RecordField Name="v_ket" />
                                        <ext:RecordField Name="c_plno" />
                                        <ext:RecordField Name="c_expno" />
                                        <ext:RecordField Name="c_entry" />
                                        <ext:RecordField Name="c_pin" />
                                        <ext:RecordField Name="l_print" Type="Boolean" />
                                        <ext:RecordField Name="l_sent" Type="Boolean" />
                                        <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                                        <ext:RecordField Name="ETDPL" Type="Date" DateFormat="M$" />
                                    </Fields>
                                </ext:JsonReader>
                            </Reader>
                            <SortInfo Field="c_dono" Direction="DESC" />
                        </ext:Store>
                    </Store>
                    <ColumnModel>
                        <Columns>
                            <ext:CommandColumn Width="50" Resizable="false">
                                <Commands>
                                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                                </Commands>
                                <%--<PrepareToolbar Handler="prepareCommandsDoPL(record, toolbar, #{hfDONo}.getValue());" />--%>
                            </ext:CommandColumn>
                            <ext:Column ColumnID="c_dono" DataIndex="c_dono" Header="Nomor DO" Hideable="false" />
                            <ext:DateColumn ColumnID="d_dodate" DataIndex="d_dodate" Header="Tanggal" Format="dd-MM-yyyy" />
                            <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" />
                            <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="150" />
                            <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="VIA" Width="80" />
                            <ext:Column ColumnID="c_plno" DataIndex="c_plno" Header="No PL" />
                            <%--<ext:Column ColumnID="c_pin" DataIndex="c_pin" Header="Pin" />--%>
                            <%--<ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="Ekspedisi" Width="75" />--%>
                            <%--<ext:CheckColumn ColumnID="l_send" DataIndex="l_send" Header="Confirm" />--%>
                            <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="No. Expedisi" />
                            <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                            <ext:DateColumn ColumnID="ETDPL" DataIndex="ETDPL" Header="ETD" Format="dd-MM-yyyy" />                         
                            <ext:Column ColumnID="c_entry" DataIndex="c_entry" Header="N I P" />
                            <ext:Column ColumnID="c_pin" DataIndex="c_pin" Header="P I N" />
                            <ext:CheckColumn ColumnID="l_print" DataIndex="l_print" Header="Cetak" Width="50" />                
                            <ext:CommandColumn ColumnID="l_sent" DataIndex="l_sent" Header="Kirim" Width="50"
                                ButtonAlign="Center">
                                <Commands>
                                    <ext:GridCommand CommandName="Submit" Icon="Accept" ToolTip-Title="" ToolTip-Text="Kirim DO" />
                                </Commands>
                                <PrepareToolbar Handler="prepareCommandsParentDoPL(record, toolbar);" />
                            </ext:CommandColumn>
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
                                                        <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                                                        <%--<Click Handler="clearFilterGridHeader(#{GridMain}, #{txEXPFltr},#{txDODate},#{cbCustomer},#{cbViaHdr},#{txNoPLFltr}, #{txNoExpFltr}, #{txNipFltr}, #{txPin},#{sbCekSend});reloadFilterGrid(#{gridMain});"--%>
                                                        <Click Handler="clearFilterGridHeader(#{GridMain}, #{txEXPFltr},#{txDODate},#{cbCustomer},#{cbViaHdr},#{txNoPLFltr}, #{txNoExpFltr}, #{txNipFltr}, #{txPin},#{sbCekSend},#{txETDDate});reloadFilterGrid(#{gridMain});"
                                                            Buffer="300" Delay="300" />
                                                    </Listeners>
                                                </ext:Button>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:TextField ID="txNoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                        <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                </ext:TextField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:DateField ID="txDODate" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                                                    AllowBlank="true">
                                                    <Listeners>
                                                        <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                                                    </Listeners>
                                                </ext:DateField>
                                            </Component>
                                        </ext:HeaderColumn>
                                        <ext:HeaderColumn />
                                        <ext:HeaderColumn>
                                            <Component>
                                                <ext:ComboBox ID="cbCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                                                    Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
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
                                                                <ext:Parameter Name="model" Value="2011" />
                                                                <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                  ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), '']]"
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
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:ComboBox>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:ComboBox ID="cbViaHdr" runat="server" DisplayField="v_ket" ValueField="c_type"
                          Width="250" AllowBlank="true" TypeAhead="false" ForceSelection="false">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="true" />
                          </CustomConfig>
                          <Store>
                            <ext:Store runat="server" RemotePaging="false">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="allQuery" Value="true" />
                                <ext:Parameter Name="model" Value="2001" />
                                <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                          ['c_notrans = @0', '02', ''],
                                ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaHdr}), '']]" Mode="Raw" />
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
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:ComboBox>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:TextField ID="txNoPLFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                          <Listeners>
                            <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                          </Listeners>
                        </ext:TextField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:TextField ID="txNoExpFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                          <Listeners>
                            <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                          </Listeners>
                        </ext:TextField>
                      </Component>
                    </ext:HeaderColumn>
                    <%--Indra 20190312FM Tambah kolom ETD di Grid--%>
                    <ext:HeaderColumn>
                        <Component>
                            <ext:DateField ID="txETDDate" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                                AllowBlank="true">
                                <Listeners>
                                    <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                                </Listeners>
                            </ext:DateField>
                        </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                        <Component>
                            <ext:TextField ID="txNipFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                    <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                </Listeners>
                            </ext:TextField>
                        </Component>
                    </ext:HeaderColumn>
                     <ext:HeaderColumn>
                      <Component>
                        <ext:TextField ID="txPin" runat="server" EnableKeyEvents="true" AllowBlank="true">
                          <Listeners>
                            <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                          </Listeners>
                        </ext:TextField>
                      </Component>
                    </ext:HeaderColumn>
                                        <ext:HeaderColumn />                    
                     <ext:HeaderColumn>
                        <Component>
                          <ext:SelectBox ID="sbCekSend" runat="server" ListWidth="100">
                            <Items>
                              <ext:ListItem Text="&nbsp;" Value="" />
                              <ext:ListItem Text="Sudah Terkirim" Value="true" />
                              <ext:ListItem Text="Belum Terkirim" Value="false" />
                            </Items>
                            <Listeners>
                              <Select handler="reloadFilterGrid(#{GridMain})" buffer="100" delay="100" />
                            </Listeners>
                          </ext:SelectBox>
                        </Component>
                      </ext:HeaderColumn>
                    <ext:HeaderColumn />                    
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
  </Items>
</ext:Viewport>
<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Items>
    <ext:BorderLayout runat="server" ID="bdrLay1">
      <North MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="175" Padding="10">
          <Items>
            <ext:Panel runat="server" Header="false" Border="false">
              <Items>
                <ext:ComboBox FieldLabel="Customer" runat="server" ID="cbCustomerHdr" DisplayField="v_cunam"
                  ValueField="c_cusno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                  MinChars="3" AllowBlank="false" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Store>
                    <ext:Store runat="server" ID="storeID">
                      <Proxy>
                        <ext:ScriptTagProxy CallbackParam="soaScmsCallback" Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson"
                          Timeout="10000000" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="10" />
                        <ext:Parameter Name="model" Value="2011" />
                        <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                       ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
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
                  <Template ID="Template3" runat="server">
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
                    <Select Handler="clearRelatedComboRecursive(this, #{cbPLHdr})" />
                  </Listeners>
                </ext:ComboBox>
                <ext:ComboBox FieldLabel="No PL" runat="server" ID="cbPLHdr" DisplayField="c_plno"
                  ValueField="c_plno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="350"
                  MinChars="3" AllowBlank="false" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Store>
                    <ext:Store runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy CallbackParam="soaScmsCallback" Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson"
                          Timeout="10000000" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="10" />
                        <ext:Parameter Name="model" Value="6001" />
                        <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                       ['@contains.c_plno.Contains(@0)', paramTextGetter(#{cbPLHdr}), '']]"
                          Mode="Raw" />
                        <ext:Parameter Name="sort" Value="c_plno" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_plno" Root="d.records" SuccessProperty="d.success"
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_plno" />
                            <ext:RecordField Name="v_ket" />
                            <ext:RecordField Name="c_baspbno" />
                            <ext:RecordField Name="ket" Type="Int" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Template ID="tmpPL" runat="server">
                    <Html>
                    <table cellpading="0" cellspacing="1" style="width: 350px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Ket</td>
                        <td class="body-panel">Pharos Group</td>
                        <tpl for="."><tr class="search-item">
                            <td>{c_plno}</td> <td>{v_ket}</td><td>{ket}</td>
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
                  <DirectEvents>
                    <Change OnEvent="OnEvenAddGrid">
                      <ExtraParams>
                        <%--<ext:Parameter Name="Command" Value="Command" Mode="Raw" />--%>
                        <ext:Parameter Name="Parameter" Value="c_plno" />
                        <ext:Parameter Name="PrimaryID" Value="#{cbPLHdr}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="GudangID" Value="#{hfGudang}.getValue()" Mode="Raw" />
                      </ExtraParams>
                    </Change>
                  </DirectEvents>
                </ext:ComboBox>
                <ext:ComboBox ID="cbViaPLDtlHdr" runat="server" FieldLabel="Via" DisplayField="v_ket" ValueField="c_type"
                  Width="250" AllowBlank="false" ForceSelection="false">
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
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2001" />
                        <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                ['c_notrans = @0', '02', ''],
                                                ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaPLDtlHdr}), '']]"
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
                <ext:TextField ID="txKet" runat="server" FieldLabel="Keterangan" MaxLength="100"
                  Width="400" />
                <ext:Label ID="lblTot" runat="server" FieldLabel="Total Item" MaxLength="100"
                  Width="400" />
                <ext:TextField ID="lblQtyTotal" runat="server" FieldLabel="Total Item" MaxLength="100"
                  Width="400" Hidden="true" />
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="175">
        <ext:Panel ID="pnlDetil" runat="server" Title="Detail" Height="200" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <LoadMask ShowMask="true" />
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
                    <%--<ext:Parameter Name="model" Value="" />--%>
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <%--<ext:Parameter Name="parameters" Value="[[]]"
                      Mode="Raw" />--%>
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="l_new" />
                        <ext:RecordField Name="n_sisa" Type="Float" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <Listeners>
                    <Load Handler=" recOnGrid(#{gridDetail}, #{lblTot}, #{lblQtyTotal});" />
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
                    <PrepareToolbar Handler="prepareCommandsDoPL(toolbar, #{hfDONo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="250" />
                  <ext:NumberColumn DataIndex="n_sisa" Header="Quantity" Format="0.000,00/i" Width="75" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreDoPL(record); }" />
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
            <ext:Parameter Name="NumberID" Value="#{hfDONo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfDONo}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
      <DirectEvents>
        <Click OnEvent="ReloadBtn_Click">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button4" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>  
  </Buttons>
</ext:Window>
<uc2:DO_PL_Print ID="DOPLPrint" runat="server" />
<ext:Window ID="wndDown" runat="server" Hidden="true" />
