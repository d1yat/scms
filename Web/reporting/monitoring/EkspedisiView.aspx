<%@ Page Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="EkspedisiView.aspx.cs"
  Inherits="transaksi_pengiriman_Ekspedisi" %>

<%@ Register Src="EkspedisiPrintCtrl.ascx" TagName="EkspedisiPrintCtrl" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

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
      var fieldX = ['c_dono'];

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

    var prepareCommands = function(toolbar, valX) {
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

    var voidInsertedDataFromStore = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      var isVoid = rec.get('l_void');

      if (isVoid) {
        ShowWarning('Item ini telah di batalkan.');
      }
      else {
        ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?', function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.', function(btnP, txt) {
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

    var voidEXPDataFromStore = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?', function(btn) {
        if (btn == 'yes') {
          ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.', function(btnP, txt) {
            if (btnP == 'ok') {
              if (txt.trim().length < 1) {
                txt = 'Kesalahan pemakai.';
              }

              Ext.net.DirectMethods.DeleteMethod(rec.get('c_expno'), txt);
            }
          });
        }
      });
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

    var prepareCommandsDetil = function(rec, toolbar, valX) {
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

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfGdg" runat="server" />
  <ext:Hidden ID="hfExpNo" runat="server" />
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar ID="Toolbar1" runat="server">
            <Items>
              <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
                <Listeners>
                  <Click Handler="refreshGrid(#{gridMain});" />
                </Listeners>
              </ext:Button>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel runat="server" ID="gridMain">
            <LoadMask ShowMask="true" />
            <DirectEvents>
              <Command OnEvent="GridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_expno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_expno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="strGridMain" runat="server" RemotePaging="true" RemoteSort="true"
                SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0005" />
                  <ext:Parameter Name="parameters" Value="[['c_expno', paramValueGetter(#{txEXPFltr}) + '%', ''],
                    ['d_expdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                    ['c_gdg = @0', paramValueGetter(#{cbGudang}) , 'System.Char'],
                    ['c_via = @0', paramValueGetter(#{cbViaFltr}) , 'System.String'],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_expno">
                    <Fields>
                      <ext:RecordField Name="c_expno" />
                      <ext:RecordField Name="d_expdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="v_cunam" />
                      <ext:RecordField Name="v_ketTran" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_expno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_expdate" DataIndex="d_expdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="120" />
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="150" />
                <ext:Column ColumnID="v_ketTran" DataIndex="v_ketTran" Header="Via" />
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
                              <Click Handler="clearFilterGridHeader(#{GridMain}, #{txEXPFltr}, #{txDateFltr},#{cbGudang}, #{cbCustomerFltr},#{cbViaFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txEXPFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDateFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbGudang" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="300" AllowBlank="true" ForceSelection="false" TypeAhead="false">
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
                                  <ext:Parameter Name="model" Value="2031" />
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudang}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="c_gdg" />
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="false" />
                            </CustomConfig>
                            <Store>
                              <ext:Store runat="server" AutoLoad="false">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="={0}" />
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2011" />
                                  <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                  ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_cunam" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
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
                              <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbViaFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                            Width="250" TypeAhead="false" AllowBlank="true" ForceSelection="false">
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
                                                                        ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaFltr}), '']]"
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
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
    Maximizable="true" MinHeight="520" MinWidth="725" Layout="Fit">
    <Items>
      <ext:BorderLayout ID="bllayout" runat="server">
        <North MinHeight="175" MaxHeight="175" Collapsible="false">
          <ext:FormPanel ID="frmHeaders" Title="Header" runat="server" Layout="Column" MinHeight="170"
            MaxHeight="170">
            <Items>
              <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5"
                Layout="Form" LabelAlign="Right" Padding="10">
                <Items>
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
                  <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                    ValueField="c_cusno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                    MinChars="3" AllowBlank="false" ForceSelection="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="false" />
                    </CustomConfig>
                    <Store>
                      <ext:Store ID="Store6" runat="server" SkinID="OriginalExtStore">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="10" />
                          <ext:Parameter Name="model" Value="5005" />
                          <ext:Parameter Name="parameters" Value="[['c_via', #{cbViaHdr}.getValue() , 'System.String'],
                                    ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
                            Mode="Raw" />
                          <ext:Parameter Name="sort" Value="v_cunam" />
                          <ext:Parameter Name="dir" Value="ASC" />
                        </BaseParams>
                        <Reader>
                          <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
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
                      <Change Handler="clearRelatedComboRecursive(true, #{cbDODtl});#{gridDetail}.getStore().removeAll();" />
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
                    <Listeners>
                      <Select Handler="cekPilihExp(this, #{cbEksHdr});" />
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
          <ext:Panel ID="pnlDetailEntry" runat="server" itle="Detail" Height="150" Layout="Fit">
            <Items>
              <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit" AutoScroll="true">
                <SelectionModel>
                  <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Store>
                  <ext:Store ID="Store5" runat="server" RemotePaging="false" RemoteSort="false" AutoLoad="false">
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
                        </Fields>
                      </ext:JsonReader>
                    </Reader>
                  </ext:Store>
                </Store>
                <ColumnModel>
                  <Columns>
                    <ext:Column DataIndex="c_dono" Header="Kode" Width="150" />
                    <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                  </Columns>
                </ColumnModel>
              </ext:GridPanel>
            </Items>
          </ext:Panel>
        </Center>
      </ext:BorderLayout>
    </Items>
    <Buttons>
      <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
        <Listeners>
          <Click Handler="#{winDetail}.hide();" />
        </Listeners>
      </ext:Button>
    </Buttons>
  </ext:Window>
  <uc1:EkspedisiPrintCtrl ID="eksctrl" runat="server" />
</asp:Content>
