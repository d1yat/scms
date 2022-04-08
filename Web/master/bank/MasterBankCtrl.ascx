<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterBankCtrl.ascx.cs" Inherits="master_bank_MasterBankCtrl" %>

<script type="text/javascript">

  var afterEdit = function(e) {
    if (!e.record.get('l_new')) {
      e.record.set('l_modified', true);
    }
  };

  var storeToDetailGrid = function(frm, grid, rek, pemilik, gl, tipe) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }


    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(rek) ||
          Ext.isEmpty(pemilik) ||
          Ext.isEmpty(gl) ||
          Ext.isEmpty(tipe)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [rek.getValue()];
    var fieldX = ['c_rekno'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var rekVal = rek.getValue().trim();
      var pemVal = pemilik.getValue();
      var glVal = gl.getValue();
      var tipVal = tipe.getValue().trim();
      var tipTex = tipe.getText().trim();


      store.insert(0, new Ext.data.Record({
        'c_rekno': rekVal,
        'v_pemilk': pemVal,
        'c_glno': glVal,
        'v_ket': tipTex,
        'c_type': tipVal,
        'l_new': true
      }));

      rek.reset();
      pemilik.reset();
      gl.reset();
      tipe.reset();
    }
    else {
      ShowError('Data telah ada.');

      return false;
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
                      rec.set('v_ketBatal', txt);
                    }
                  });
              }
            });
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
<ext:Window ID="winDetail" runat="server" Height="525" Width="800" Hidden="true"
  Maximizable="true" MinHeight="525" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfBankId" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="155" MaxHeight="155" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="155" Padding="10">
          <Items>
            <ext:TextField ID="txNamaBank" runat="server" MaxLength="20" FieldLabel="Bank" Width="150" />
            <ext:TextField ID="txNamaCabang" runat="server" MaxLength="50" FieldLabel="Cabang" Width="250" />
            <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" />
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    
                    <ext:TextField ID="txNoRek" runat="server" MaxLength="20" FieldLabel="No Rekening" Width="170" />
                    <ext:TextField ID="txPemilik" runat="server" MaxLength="50" FieldLabel="Pemilik" Width="170" />
                    <ext:TextField ID="txNoGL" runat="server" MaxLength="50" FieldLabel="No GL" Width="170" />
                    <ext:ComboBox ID="cbTipe" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                      ValueField="c_type" ItemSelector="tr.search-item" ListWidth="200" MinChars="3">
                      <Store>
                        <ext:Store ID="Store2" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '47', 'System.String'],
                                    ['c_portal = @0', '0', 'System.Char']]" Mode="Raw" />
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
                    </ext:ComboBox>
                    <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                      Icon="Add">
                      <Listeners>
                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{txNoRek}, #{txPemilik}, #{txNoGL}, #{cbTipe});" />
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
                    <ext:Parameter Name="model" Value="0169" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_bank = @0', #{hfBankId}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_rekno" />
                        <ext:RecordField Name="IDX" />
                        <ext:RecordField Name="c_bank" />
                        <ext:RecordField Name="c_glno" />
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="v_pemilk" />
                        <ext:RecordField Name="v_bank" />
                        <ext:RecordField Name="v_ket" />
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
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Delete" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfBankId}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column ColumnID="IDX" DataIndex="IDX" Header="No" Width="50" Hidden="true" />
                  <ext:Column ColumnID="c_rekno" DataIndex="c_rekno" Header="No Rekening" Width="150">
                    <Editor>
                      <ext:TextField ColumnID="v_pemilk" runat="server" Width="150" />
                    </Editor>
                  </ext:Column>
                  <ext:Column ColumnID="v_pemilk" DataIndex="v_pemilk" Header="Pemilik" Width="150" >
                    <Editor>
                      <ext:TextField ColumnID="v_pemilk" runat="server" Width="150" />
                    </Editor>
                  </ext:Column>
                  <ext:Column ColumnID="c_glno" DataIndex="c_glno" Header="No GL" Width="150" >
                    <Editor>
                      <ext:TextField ColumnID="c_glno" runat="server" Width="150" />
                    </Editor>
                  </ext:Column>
                  <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Tipe" Width="130" >
                    <%--<Editor>
                      <ext:ComboBox ColumnID="v_ket" ID="cbTipeEdit" runat="server" DisplayField="v_ket"
                        ValueField="c_type" ItemSelector="tr.search-item" ListWidth="200" MinChars="3">
                        <Store>
                          <ext:Store ID="Store3" runat="server">
                            <Proxy>
                              <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                CallbackParam="soaScmsCallback" />
                            </Proxy>
                            <BaseParams>
                              <ext:Parameter Name="start" Value="={0}" />
                              <ext:Parameter Name="limit" Value="-1" />
                              <ext:Parameter Name="allQuery" Value="true" />
                              <ext:Parameter Name="model" Value="2001" />
                              <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '47', 'System.String'],
                                      ['c_portal = @0', '0', 'System.Char']]" Mode="Raw" />
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
                      </ext:ComboBox>
                    </Editor>--%>
                  </ext:Column>
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                <AfterEdit Fn="afterEdit" />
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfBankId}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
