<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BlockItem.aspx.cs" Inherits="master_blokitem_MasterItem"
  MasterPageFile="~/Master.master" %>

<%@ Register Src="BlockItemCtrl.ascx" TagName="BlockItemCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript" language="javascript">
    var updateChangedItem = function(grid) {
      if (Ext.isEmpty(grid)) {
        return;
      }
      var stor = grid.getStore();
      if (Ext.isEmpty(stor)) {
        return;
      }
      if (stor.getModifiedRecords().length < 1) {
        return;
      }

      var valJS = saveStoreToServer(stor, true);

      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menyimpan data ini ?',
        function(btn) {
          if (btn == 'yes') {
            Ext.net.DirectMethods.SaveBlockItemMethod(valJS, stor.storeId);
          }
        });
    }

    var checkOrUncheckAll = function(isCek, grid) {
      if (Ext.isEmpty(grid)) {
        return;
      }

      var stor = grid.getStore();
      if (Ext.isEmpty(stor)) {
        return;
      }

      var prevBlock = false;
      stor.each(function(r) {
        prevBlock = r.get('prevBlock');

        if (prevBlock == isCek) {
          r.reject();
        }
        else {
          r.set('l_block', isCek);
        }

      }, this);
    }

    var recGridAddLoadHandler = function(s, recs) {
      Ext.each(recs, function(r) {
        r.set('prevBlock', r.get('l_block'));
      });
      s.commitChanges();
    }

    var recGridAfterEdit = function(e) {
      var prevBlock = e.record.get('prevBlock');
      if (e.field == 'l_block') {
        if (prevBlock == e.value) {
          e.record.reject();
        }
        else {
          e.record.set('l_block', e.value);
        }
      }
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
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
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_iteno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_iteno" Mode="Raw" />
                  <ext:Parameter Name="PrimaryNameID" Value="record.data.v_itnam" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridItem" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0163" />
                  <ext:Parameter Name="parameters" Value="[['@contains.c_iteno.Contains(@0)', paramRawValueGetter(#{txItemIDFltr}), ''],
                              ['c_nosup = @0', paramValueGetter(#{cbPrincipalFltr}) , 'System.String'],
                              ['@contains.v_itnam.Contains(@0)', paramRawValueGetter(#{txItemNameFltr}), '']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_iteno">
                    <Fields>
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="v_itnam" />
                      <ext:RecordField Name="c_nosup" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="l_aktif" Type="Boolean" />
                      <ext:RecordField Name="l_hide" Type="Boolean" />
                      <ext:RecordField Name="l_block" Type="Boolean" />
                      <ext:RecordField Name="prevBlock" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="v_itnam" Direction="ASC" />
                <Listeners>
                  <Load Fn="recGridAddLoadHandler" />
                </Listeners>
              </ext:Store>
            </Store>
            <Listeners>
              <AfterEdit Fn="recGridAfterEdit" />
            </Listeners>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />--%>
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode Item" Width="70" />
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama" Width="200" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama Supplier" Width="150" />
                <ext:CheckColumn ColumnID="l_hide" DataIndex="l_hide" Header="Sembunyi" Width="60" />
                <ext:CheckColumn ColumnID="l_block" DataIndex="l_block" Header="Blok" Width="40"
                  Editable="true" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txItemIDFltr}, #{cbPrincipalFltr}, #{txItemNameFltr}, #{cbTipeJenisFltr}, #{cbTipeViaFltr});reloadFilterGrid(#{gridMain});"
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
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbPrincipalFltr" runat="server" DisplayField="v_nama" ValueField="c_nosup"
                            Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
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
                                  <ext:Parameter Name="model" Value="2021" />
                                  <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                              ['l_hide = @0', false, 'System.Boolean'],
                                              ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalFltr}), '']]"
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:Panel runat="server" Frame="false" Border="false" BodyStyle="background:transparent"
                            Header="false">
                            <Content>
                              <center>
                                <ext:Checkbox ID="chkCheckAll" runat="server">
                                  <Listeners>
                                    <Check Handler="checkOrUncheckAll(checked, #{gridMain});" />
                                  </Listeners>
                                </ext:Checkbox>
                              </center>
                            </Content>
                          </ext:Panel>
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
                  <ext:ToolbarSeparator />
                  <ext:Button ID="btnSave" runat="server" Text="Simpan" Icon="Disk">
                    <Listeners>
                      <Click Handler="updateChangedItem(#{gridMain});" />
                    </Listeners>
                  </ext:Button>
                </Items>
              </ext:PagingToolbar>
            </BottomBar>
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  <uc:BlockItemCtrl runat="server" ID="BlockItemCtrl1" />
</asp:Content>
