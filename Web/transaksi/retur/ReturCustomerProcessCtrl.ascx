<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReturCustomerProcessCtrl.ascx.cs"
  Inherits="transaksi_retur_ReturCustomerProcessCtrl" %>

<script type="text/javascript">

    var setDefaultAcc = function(chkDefault, store, chkDeclined) {
        var chk = chkDefault.getValue();

        if (chk) {
            store.each(function(r) {

                r.set('n_qtyAcc', r.get('N_QTY'));
                r.set('c_batchterima', r.get('C_BATCH'));

            });
            chkDeclined.setValue(false);
        }
        else {
            store.each(function(r) {
                r.set('n_qtyAcc', 0);
                r.set('c_batchterima', 0);

            });
        }


    }
  

  var prepareCommandsProcess = function(rec, toolbar, valX) {
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

  var voidInsertedDataFromStoreProcess = function(rec) {
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

  var afterEditDataRCProcess = function(e, cb) {
      if (e.field == 'n_qtyAcc') {
          var nBook = e.record.get('N_QTY');
          if (e.value > nBook) {
//              e.value = nBook;
//              e.record.set('n_qtyAcc', nBook);
          }
          else if (e.value < 0) {
//              e.value = nBook;
//              e.record.set('n_qtyAcc', nBook);
          }
      }
      else if (e.field == 'n_destroy') {
          var nBook = e.record.get('N_QTY');
          if (e.value > nBook) {
//              e.value = nBook;
//              e.record.set('n_destroy', nBook);
          }
          else if (e.value < 0) {
//              e.value = nBook;
//              e.record.set('n_destroy', nBook);
          }
          
          if (cb.substr(0, 3) == 'PBB' || cb.substr(0, 3) == 'TBB') {
//              e.value = 0;
//              e.record.set('n_destroy', 0);
          }
      }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="510" Width="800" Hidden="true"
  Maximizable="true" MinHeight="510" MinWidth="1000" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudangProcess" runat="server" />
    <ext:Hidden ID="hfGudangProcessDesc" runat="server" />
    <ext:Hidden ID="hfRCNoProcess" runat="server" />
    <ext:Hidden ID="hfStoreIDProcess" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="160" MaxHeight="160" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="160" Padding="10">
          <Items>
            <%--<ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
              ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
              MinChars="3">
              <Store>
                <ext:Store ID="Store1" runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                    <ext:Parameter Name="model" Value="2031" />
                    <ext:Parameter Name="parameters" Value="[['c_gdg != @0', '3', 'System.Char']]" Mode="Raw" />
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
            </ext:ComboBox>--%>
            <ext:Label ID="lbGudang" runat="server" FieldLabel="Gudang" />
            <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
              ValueField="c_cab" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
              MinChars="3" AllowBlank="false" ForceSelection="false" Disabled="true">
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
                    <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                    ['l_cabang = @0', true, 'System.Boolean'],
                                    ['l_stscus = @0', true, 'System.Boolean'],
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
                        <ext:RecordField Name="v_cunam" />
                        <ext:RecordField Name="c_cab" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <Template runat="server">
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
                <%--<Change Handler="clearRelatedComboRecursive(true, #{cbDO});" />--%>
              </Listeners>
            </ext:ComboBox>
            <ext:TextField FieldLabel="No PBB / R" runat="server" ID="txPBBR" Width="400px" MaxLength="100"
              AllowBlank="false">
              <DirectEvents>
                <Change OnEvent="OnEvenAddGrid">
                  <ExtraParams>
                    <ext:Parameter Name="Parameter" Value="c_no" />
                    <ext:Parameter Name="PrimaryID" Value="#{txPBBR}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Change>
              </DirectEvents>
            </ext:TextField>
            <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLengthText="100"
              Width="400" />
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
        
       <TopBar>
              <ext:Toolbar ID="Toolbar1" runat="server">
                  <Items>
                       <ext:Checkbox runat="server" ID="chkAllAcc" FieldLabel="Set All Accepted ">
                             <Listeners>
                                 <Check Handler="setDefaultAcc(this, #{gridDetail}.getStore(),#{chkAllDec});" />
                             </Listeners>
                        </ext:Checkbox>
                
                 </Items>
            </ext:Toolbar>
      </TopBar>
        
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
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />   
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <%--<ext:RecordField Name="C_ITENO" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="c_type_item" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="c_dono" />
                        <ext:RecordField Name="c_rnno" />
                        <ext:RecordField Name="c_itenoType" />
                        <ext:RecordField Name="n_qty" />
                        <ext:RecordField Name="n_qtyAcc" />--%>                        
                        <ext:RecordField Name="C_ITENO" />
                        <ext:RecordField Name="C_ITNAM" />
                        <ext:RecordField Name="C_BATCH" />
                        <ext:RecordField Name="c_batchterima" /><%--Suwandi--%>
                        <ext:RecordField Name="N_QTY" />
                        <ext:RecordField Name="C_NOREF" />
                        <ext:RecordField Name="C_CUNAM" />
                        <ext:RecordField Name="C_CUSTYPE" />
                        <ext:RecordField Name="C_REASON" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="0">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommandsProcess(record, toolbar, #{hfRCNoProcess}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="C_ITENO" Header="Kode" Width="50" />
                  <ext:Column DataIndex="C_ITNAM" Header="Nama Barang" Width="250" />
                  <ext:Column DataIndex="C_BATCH" Header="Batch " />
                  <ext:Column DataIndex="c_batchterima" Header="Batch Terima" >
                   <Editor>
                      <ext:TextField DataIndex="c_batchterima" runat="server" Header="Batch Terima" AllowBlank="false" />
                    </Editor>
                  </ext:Column> 
                  <ext:Column DataIndex="C_NOREF" Header="No. DO" />
                  <ext:Column DataIndex="N_QTY" Header="Qty" />
                  <ext:NumberColumn DataIndex="n_qtyAcc" Header="Qty Terima" Format="0.000,00/i" Width="75">
                    <Editor>
                      <ext:NumberField DataIndex="n_qtyAcc" runat="server" Header="Qty Terima" Width="75" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:NumberColumn DataIndex="n_destroy" Header="Qty Pemusnahan" Format="0.000,00/i" Hidden="true"
                    Width="75">
                    <Editor>
                      <ext:NumberField DataIndex="n_destroy" runat="server" Header="Qty Pemusnahan" Width="75" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:Column DataIndex="C_CUNAM" Header="Outlet" Width="100" />
                  <ext:Column DataIndex="C_CUSTYPE" Header="Tipe Outlet" Width="50" />                  
                  <ext:Column DataIndex="C_REASON" Header="Reason" Width="150" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreProcess(record); }" />
                <AfterEdit Handler="afterEditDataRCProcess(e, #{txPBBR}.getValue());" />
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
          <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." 
            BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfRCNoProcess}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreIDProcess}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangDesc" Value="#{hfGudangProcessDesc}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GudangID" Value="#{hfGudangProcess}.getValue()" Mode="Raw" />
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
