<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EkspedisiGudangViewCtrl.ascx.cs"
  Inherits="transaksi_pengiriman_EkspedisiGudangCtrl" %>

<script type="text/javascript">
  var storeToDetailGrid = function(frm, grid, dono) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(dono)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var valX = [dono.getValue()];
    var fieldX = ['c_sjno'];

    var c_dono = dono.getValue();

    if (c_dono.length != 10) {
      ShowWarning("No tidak terdefinisi.");
      return false;
    }

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      store.insert(0, new Ext.data.Record({
        'c_dono': c_dono,
        'l_new': true
      }));

      dono.reset();

    } else {
      ShowError("Data Telah Ada");
    }
  }

  var validasiJamResi = function(obj) {
    if (Ext.isEmpty(obj)) {
      return;
    }

    var valu = obj.getValue();
    var tgl = (Ext.isDate(valu) ? valu : Date.parseDate(valu, 'g:i:s'));

    obj.setValue(myFormatTime(tgl));
  }

  var cekPilihExp = function(cb, cbExp) {
    if (Ext.isEmpty(cb)) {
      if (!Ext.isEmpty(cbExp)) {
        cbExp.disable();
        cbExp.clearValue();
      }
      return;
    }

    if (cb.getValue() == '01') { // Tipe Expedisi
      if (!Ext.isEmpty(cbExp)) {
        cbExp.enable();
        cbExp.clearValue();
      }
    }
    else {
      if (!Ext.isEmpty(cbExp)) {
        cbExp.disable();
        cbExp.clearValue();
      }
    }
  }

  var prepareCommandsExpGdg = function(rec, toolbar, valX) {
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
  Maximizable="true" MinHeight="520" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudang" runat="server" />
    <ext:Hidden ID="hfGudangDesc" runat="server" />
    <ext:Hidden ID="hfExpNo" runat="server" />
    <ext:Hidden ID="hfType" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="190" MaxHeight="190" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" Title="Header" runat="server" Layout="Column" MinHeight="190"
          MaxHeight="190">
          <Items>
            <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5"
              Layout="Form" LabelAlign="Right" Padding="10">
              <Items>
                <ext:Label ID="lbGudangFrom" runat="server" FieldLabel="Asal" />
                <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Tujuan" DisplayField="v_gdgdesc"
                  ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
                  MinChars="3" AllowBlank="false" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store6" runat="server" RemotePaging="false">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <%--<ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="={10}" />--%>
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2031" />
                        <ext:Parameter Name="parameters" Value="[['c_gdg != @0', #{hfGdg}.getValue(), 'System.Char']]"
                          Mode="Raw" />
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
                    <table cellpading="0" cellspacing="0" style="width: 200px">
                          <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                          <tpl for=".">
                            <tr class="search-item">
                              <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                            </tr>
                          </tpl>
                          </table>
                    </Html>
                  </Template>
                  <Triggers>
                    <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                  </Triggers>
                  <Listeners>
                    <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                    <Change Handler="clearRelatedComboRecursive(true, #{cbDODtl});" />
                  </Listeners>
                </ext:ComboBox>
                <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
                  ValueField="c_type" Width="150" AllowBlank="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store1" runat="server" RemotePaging="false" SkinID="OriginalExtStore">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2001" />
                        <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                            ['c_notrans = @0', '02', ''],
                                            ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaHdr}), '']]"
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
                <ext:ComboBox ID="cbByHdr" runat="server" FieldLabel="Oleh" DisplayField="v_ket"
                  ValueField="c_type" MinChars="3" AllowBlank="false" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store2" runat="server">
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
                        <ext:Parameter Name="limit" Value="={10}" />
                        <ext:Parameter Name="model" Value="2001" />
                        <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                        ['c_notrans = @0', '08', ''],
                                        ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbByHdr}), '']]"
                          Mode="Raw" />
                        <ext:Parameter Name="sort" Value="v_ket" />
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
                    <Select Handler="cekPilihExp(this, #{cbEksHdr});" />
                    <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                  </Listeners>
                </ext:ComboBox>
                <ext:ComboBox ID="cbEksHdr" runat="server" FieldLabel="Ekspedisi" DisplayField="v_ket"
                  ValueField="c_exp" Width="250" MinChars="3" AllowBlank="false" ItemSelector="tr.search-item"
                  ForceSelection="false" ListWidth="300" PageSize="10">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="false" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store3" runat="server">
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
                        <ext:Parameter Name="limit" Value="={10}" />
                        <ext:Parameter Name="model" Value="5002" />
                        <ext:Parameter Name="parameters" Value="[['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbEksHdr}), '']]"
                          Mode="Raw" />
                        <ext:Parameter Name="sort" Value="v_ket" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_exp" />
                            <ext:RecordField Name="v_ket" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Template ID="Template2" runat="server">
                    <Html>
                    <table cellpading="0" cellspacing="1" style="width: 400px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                    <tpl for="."><tr class="search-item">
                        <td>{c_exp}</td><td>{v_ket}</td>
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
                <ext:TextField ID="txKetHdr" runat="server" FieldLabel="Keterangan" Width="250" />
              </Items>
            </ext:Panel>
            <ext:Panel ID="Panel2" runat="server" Border="false" Layout="Form" ColumnWidth=".5"
              LabelAlign="Right" Padding="10">
              <Items>
                <ext:TextField ID="txNoResiHdr" runat="server" FieldLabel="No Resi" AllowBlank="false" />
                <ext:DateField ID="txDayResiHdr" runat="server" FieldLabel="Tanngal Resi" AllowBlank="false"
                  Format="dd-MM-yyyy" />
                <ext:TextField ID="txTimeResiHdr" runat="server" FieldLabel="Jam Resi" MaxLength="8"
                  AllowBlank="false" Width="75">
                  <Listeners>
                    <Change Fn="validasiJamResi" />
                  </Listeners>
                  <Plugins>
                    <ux:InputTextMask Mask="99:99:99" />
                  </Plugins>
                </ext:TextField>
                <ext:NumberField ID="txKoli" runat="server" FieldLabel="Koli" AllowBlank="false"
                  AllowNegative="false" MinValue="0" />
                <ext:NumberField ID="txBerat" runat="server" FieldLabel="Berat" AllowBlank="false"
                  AllowNegative="false" MinValue="0" />
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlDetailEntry" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
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
                    <ext:Parameter Name="model" Value="0006" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_expno = @0', #{hfExpNo}.getValue(), 'System.String']]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_dono" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <%--<ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommandsExpGdg(record, toolbar, #{hfExpNo}.getValue());" />
                  </ext:CommandColumn>--%>
                  <ext:Column DataIndex="c_dono" Header="Kode" Width="150" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <%--<Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>--%>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <%--<ext:Button ID="btnSimpan" runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="NumberID" Value="#{hfExpNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>--%>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
