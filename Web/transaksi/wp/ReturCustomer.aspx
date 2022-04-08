<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReturCustomer.aspx.cs" Inherits="transaksi_wp_ReturCustomer"
 MasterPageFile="~/Master.master" %>
 
 <asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">

    var voidWPDataFromStore = function(rec) {
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

                    Ext.net.DirectMethods.DeleteMethod(rec.get('c_notrans'), rec.get('c_gdg'), txt);
                  }
                });
            }
          });
    }

    var prepareCommands = function(rec, toolbar) {
      var del = toolbar.items.get(0); // delete button
      var vd = toolbar.items.get(1); // void button
      var rt = toolbar.items.get(2); // void button
    }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfGdg" runat="server" />
  <ext:Hidden ID="hfType" runat="server" />
  <ext:Hidden ID="hfGdgDesc" runat="server" />
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <Items>
          <ext:GridPanel ID="gridMain" runat="server">
            <LoadMask ShowMask="true" />
            <Listeners>
              <Command Handler="if(command == 'Delete') { voidWPDataFromStore(record); }" />
            </Listeners>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridWP" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0303" />
                  <ext:Parameter Name="parameters" Value="[['c_gdg', paramValueGetter(#{hfGdg}) , 'System.Char'],
                        ['c_type', paramValueGetter(#{hfType}) , 'System.String']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_notrans">
                    <Fields>
                      <ext:RecordField Name="c_notrans" />
                      <ext:RecordField Name="c_gdg" />
                      <ext:RecordField Name="c_type" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_notrans" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <%--<ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />--%>
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                  </Commands>
                  <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                </ext:CommandColumn>
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="150" />
                <ext:Column ColumnID="c_notrans" DataIndex="c_notrans" Header="Nomor" Hideable="false" Width="210" />
                <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Tanggal" Format="dd-MM-yyyy HH:mm:ss" Width="200" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{cbDODtl});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn >
                        <Component>
                          <ext:ComboBox ID="cbGudang" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="300" AllowBlank="true" ForceSelection="false" TypeAhead="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store1" runat="server" RemotePaging="false">
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
                          <ext:CompositeField ID="CompositeField1" runat="server">
                            <Items>
                              <%--<ext:TextField ID="txNoTrans" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                <Listeners>
                                  <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                </Listeners>
                              </ext:TextField>--%>
                              <ext:ComboBox ID="cbDODtl" runat="server" ItemSelector="tr.search-item"
                              DisplayField="c_dono" ValueField="c_dono" MinChars="3" PageSize="10" ListWidth="300"
                              AllowBlank="false" ForceSelection="false">
                              <CustomConfig>
                                <ext:ConfigItem Name="allowBlank" Value="false" />
                              </CustomConfig>
                              <Store>
                                <ext:Store ID="Store7" runat="server" AutoLoad="false">
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="start" Value="={0}" />
                                    <ext:Parameter Name="limit" Value="10" />
                                    <ext:Parameter Name="model" Value="0304" />
                                    <ext:Parameter Name="parameters" Value="[['c_gdg', paramValueGetter(#{hfGdg}) , 'System.Char'],
                                          ['c_type', #{hfType}.getValue() , 'System.String'],
                                          ['@contains.c_dono.Contains(@0)', paramTextGetter(#{cbDODtl}), '']]"
                                      Mode="Raw" />
                                    <ext:Parameter Name="sort" Value="c_dono" />
                                    <ext:Parameter Name="dir" Value="ASC" />
                                  </BaseParams>
                                  <Reader>
                                    <ext:JsonReader IDProperty="c_dono" Root="d.records" SuccessProperty="d.success"
                                      TotalProperty="d.totalRows">
                                      <Fields>
                                        <ext:RecordField Name="c_dono" />
                                        <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                                      </Fields>
                                    </ext:JsonReader>
                                  </Reader>
                                </ext:Store>
                              </Store>
                              <Template ID="Template4" runat="server">
                                <Html>
                                <table cellpading="0" cellspacing="1" style="width: 300px">
                                <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td></tr>
                                <tpl for="."><tr class="search-item">
                                <td>{c_dono}</td><td>{d_dodate:this.formatDate}</td>
                                </tr></tpl>
                                </table>
                                </Html>
                                <Functions>
                                  <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                                </Functions>
                              </Template>
                              <Triggers>
                                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                              </Triggers>
                              <Listeners>
                                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                              </Listeners>
                            </ext:ComboBox>
                            </Items>
                          </ext:CompositeField>
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
            </BottomBar>
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
</asp:Content>


