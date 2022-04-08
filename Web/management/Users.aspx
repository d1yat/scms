<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Users.aspx.cs" Inherits="management_Users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var validateEntry = function(f) {
      if (Ext.isEmpty(f)) {
        ShowWarning('Form objek tidak terdefinisi.');
        return false;
      }

      if (!f.isValid()) {
        ShowWarning('Data tidak akurat, mohon di perbaiki.');
        return false;
      }
    }

    var resetPassword = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      var msg = String.format('Apakah anda yakin ingin mereset kata kunci pengguna \'{0}\' ?', rec.get('v_username'));

      ShowConfirm('Reset ?', msg,
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Kata kunci baru', 'Masukkan kata kunci yang baru.',
              function(btnP, txt) {
                if (btnP == 'ok') {
                  if (txt.trim().length < 5) {
                    ShowWarning('Kata kunci tidak boleh kosong dan minimum 5 karakter.');
                    return;
                  }

                  Ext.net.DirectMethods.ResetPasswordMethod(rec.get('c_nip'), txt.trim());
                }
              });
          }
        });
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit">
        <Items>
          <ext:Panel runat="server" Layout="Fit">
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
                  <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
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
                  <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Listeners>
                  <Command Handler="if(command == 'Reset') { resetPassword(record); return false; }" />
                </Listeners>
                <DirectEvents>
                  <Command OnEvent="gridMainCommand" Before="if(command == 'Reset') { return false; }">
                    <Confirmation ConfirmRequest="true" Title="Hapus ?" Message="Apa anda yakin ingin menghapus data ini ?"
                      BeforeConfirm="if(command != 'Delete') { return false; }" />
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                      <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                      <ext:Parameter Name="Parameter" Value="c_nip" />
                      <ext:Parameter Name="PrimaryID" Value="record.data.c_nip" Mode="Raw" />
                    </ExtraParams>
                  </Command>
                </DirectEvents>
                <Store>
                  <ext:Store runat="server" SkinID="OriginalExtStore">
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
                      <ext:Parameter Name="model" Value="100001" />
                      <ext:Parameter Name="parameters" Value="[['@contains.c_nip.Contains(@0)', paramValueGetter(#{txNipFltr}), ''],
                          ['@contains.v_username.Contains(@0)', paramValueGetter(#{txNamaFltr}), ''],
                          ['c_gdg = @0', paramValueGetter(#{cbGCFtlr}), '']]" Mode="Raw" />
                    </BaseParams>
                    <Reader>
                      <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                        IDProperty="c_nip">
                        <Fields>
                          <ext:RecordField Name="c_nip" />
                          <ext:RecordField Name="v_gdgdesc" />
                          <ext:RecordField Name="l_aktif" Type="Boolean" />
                          <ext:RecordField Name="v_username" />
                          <ext:RecordField Name="c_gdg" />
                          <ext:RecordField Name="c_nosup" />
                          <ext:RecordField Name="v_supdesc" />
                          <ext:RecordField Name="c_kddivpri" />
                          <ext:RecordField Name="v_divpridesc" />
                        </Fields>
                      </ext:JsonReader>
                    </Reader>
                    <SortInfo Field="c_nip" Direction="ASC" />
                  </ext:Store>
                </Store>
                <ColumnModel>
                  <Columns>
                    <ext:CommandColumn Width="75" Resizable="false" ButtonAlign="Center">
                      <Commands>
                        <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                        <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                        <ext:GridCommand CommandName="Reset" Icon="KeyStart" ToolTip-Title="" ToolTip-Text="Reset kata kunci" />
                      </Commands>
                    </ext:CommandColumn>
                    <ext:Column DataIndex="c_nip" Header="Nip" Width="100" />
                    <ext:Column DataIndex="v_username" Header="Nama" Width="200" />
                    <ext:Column DataIndex="v_gdgdesc" Header="Gudang / Cabang" Width="250" />
                    <ext:Column DataIndex="v_supdesc" Header="Pemasok" Width="150" />
                    <ext:Column DataIndex="v_divpridesc" Header="Div. Pemasok" Width="150" />
                    <ext:CheckColumn DataIndex="l_aktif" Header="Aktif" Width="50" />
                  </Columns>
                </ColumnModel>
                <View>
                  <ext:GridView runat="server" StandardHeaderRow="true">
                    <HeaderRows>
                      <ext:HeaderRow>
                        <Columns>
                          <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                            <Component>
                              <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                <Listeners>
                                  <Click Handler="clearFilterGridHeader(#{gridMain}, #{txNipFltr}, #{txNamaFltr}, #{cbGCFtlr});reloadFilterGrid(#{gridMain});"
                                    Buffer="300" Delay="300" />
                                </Listeners>
                              </ext:Button>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:TextField ID="txNipFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                  <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                </Listeners>
                              </ext:TextField>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:TextField ID="txNamaFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                  <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                </Listeners>
                              </ext:TextField>
                            </Component>
                          </ext:HeaderColumn>
                          <ext:HeaderColumn>
                            <Component>
                              <ext:ComboBox ID="cbGCFtlr" runat="server" DisplayField="v_desc" ValueField="v_kode"
                                Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                                AllowBlank="true">
                                <CustomConfig>
                                  <ext:ConfigItem Name="allowBlank" Value="true" />
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
                                      <ext:Parameter Name="model" Value="110001" />
                                      <ext:Parameter Name="parameters" Value="[['v_desc', #{cbGCFtlr}.getText().trim() + '%', '']]"
                                        Mode="Raw" />
                                      <ext:Parameter Name="sort" Value="" />
                                      <ext:Parameter Name="dir" Value="" />
                                    </BaseParams>
                                    <Reader>
                                      <ext:JsonReader IDProperty="v_kode" Root="d.records" SuccessProperty="d.success"
                                        TotalProperty="d.totalRows">
                                        <Fields>
                                          <ext:RecordField Name="v_kode" />
                                          <ext:RecordField Name="v_desc" />
                                          <ext:RecordField Name="l_aktif" Type="Boolean" />
                                        </Fields>
                                      </ext:JsonReader>
                                    </Reader>
                                  </ext:Store>
                                </Store>
                                <Template runat="server">
                                  <Html>
                                  <table cellpading="0" cellspacing="0" style="width: 400px">
                                    <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td>
                                    <td class="body-panel">Aktif</td></tr>
                                    <tpl for="."><tr class="search-item">
                                      <td>{v_kode}</td><td>{v_desc}</td>
                                      <td align="center"><input type="checkbox" value="{l_aktif}" disabled="disable" /></td>
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
                          <ext:HeaderColumn />
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
                      <ext:Label runat="server" Text="Page size:" />
                      <ext:ToolbarSpacer runat="server" Width="10" />
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
          <ext:Window ID="winDetail" runat="server" Height="255" Width="450" Hidden="true"
            Resizable="false" MinHeight="255" MinWidth="450">
            <Items>
              <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Padding="5" Layout="Form">
                <Items>
                  <ext:Hidden ID="hfNip" runat="server" />
                  <ext:TextField ID="txNip" runat="server" FieldLabel="N I P" AllowBlank="false" MaxLength="15" />
                  <ext:TextField ID="txNama" runat="server" FieldLabel="Nama" AllowBlank="false" MaxLength="100" />
                  <ext:TextField ID="txPassword" runat="server" FieldLabel="Password" AllowBlank="false"
                    InputType="Password" MaxLength="20" />
                  <ext:ComboBox ID="cbGC" runat="server" DisplayField="v_desc" ValueField="v_kode"
                    Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                    AllowBlank="true" FieldLabel="Gudang / Cabang">
                    <Store>
                      <ext:Store runat="server" AutoLoad="false">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={10}" />
                          <ext:Parameter Name="model" Value="110001" />
                          <ext:Parameter Name="parameters" Value="[['v_desc', #{cbGC}.getText().trim() + '%', '']]"
                            Mode="Raw" />
                          <ext:Parameter Name="sort" Value="" />
                          <ext:Parameter Name="dir" Value="" />
                        </BaseParams>
                        <Reader>
                          <ext:JsonReader IDProperty="v_kode" Root="d.records" SuccessProperty="d.success"
                            TotalProperty="d.totalRows">
                            <Fields>
                              <ext:RecordField Name="v_kode" />
                              <ext:RecordField Name="v_desc" />
                              <ext:RecordField Name="l_aktif" Type="Boolean" />
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                      </ext:Store>
                    </Store>
                    <Template runat="server">
                      <Html>
                      <table cellpading="0" cellspacing="0" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td>
                        <td class="body-panel">Aktif</td></tr>
                        <tpl for="."><tr class="search-item">
                          <td>{v_kode}</td><td>{v_desc}</td>
                          <td align="center"><input type="checkbox" value="{l_aktif}" disabled="disable" /></td>
                        </tr></tpl>
                        </table>
                      </Html>
                    </Template>
                  </ext:ComboBox>
                  <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
                    DisplayField="v_nama" Width="250" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
                    AllowBlank="true" ForceSelection="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="true" />
                    </CustomConfig>
                    <Store>
                      <ext:Store ID="Store3" runat="server">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={10}" />
                          <ext:Parameter Name="model" Value="2021" />
                          <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                              ['l_aktif == @0', true, 'System.Boolean'],
                              ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]"
                            Mode="Raw" />
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
                    <Template ID="Template2" runat="server">
                      <Html>
                      <table cellpading="0" cellspacing="0" style="width: 500px">
                      <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                      <tpl for=".">
                        <tr class="search-item">
                          <td>{c_nosup}</td><td>{v_nama}</td>
                        </tr>
                      </tpl>
                      </table>
                      </Html>
                    </Template>
                  </ext:ComboBox>
                  <ext:ComboBox ID="cbDivPrinsipal" runat="server" FieldLabel="Divisi Pemasok" ValueField="c_kddivpri"
                    DisplayField="v_nmdivpri" Width="310" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
                    AllowBlank="true" ForceSelection="false" Delimiter=";">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="true" />
                    </CustomConfig>
                    <Store>
                      <ext:Store ID="Store4" runat="server">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={10}" />
                          <ext:Parameter Name="model" Value="2051" />
                          <ext:Parameter Name="parameters" Value="[['c_nosup = @0', paramValueGetter(#{cbSuplier}), ''],
                                        ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]"
                            Mode="Raw" />
                          <ext:Parameter Name="sort" Value="v_nmdivpri" />
                          <ext:Parameter Name="dir" Value="ASC" />
                        </BaseParams>
                        <Reader>
                          <ext:JsonReader IDProperty="c_kddivpri" Root="d.records" SuccessProperty="d.success"
                            TotalProperty="d.totalRows">
                            <Fields>
                              <ext:RecordField Name="c_kddivpri" />
                              <ext:RecordField Name="v_nmdivpri" />
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                      </ext:Store>
                    </Store>
                    <Template ID="Template3" runat="server">
                      <Html>
                      <table cellpading="0" cellspacing="0" style="width: 500px">
                      <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                      <tpl for=".">
                        <tr class="search-item">
                          <td>{c_kddivpri}</td><td>{v_nmdivpri}</td>
                        </tr>
                      </tpl>
                      </table>
                      </Html>
                    </Template>
                  </ext:ComboBox>
                  <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" />
                </Items>
              </ext:FormPanel>
            </Items>
            <Buttons>
              <ext:Button runat="server" Icon="Disk" Text="Simpan">
                <DirectEvents>
                  <Click OnEvent="btnSave_OnClick" Before="return validateEntry(#{frmpnlDetailEntry});">
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                      <ext:Parameter Name="NIP" Value="#{hfNip}.getValue()" Mode="Raw" />
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
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
</asp:Content>
