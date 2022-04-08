<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WaktuPelayanan.aspx.cs" Inherits="transaksi_wp_WaktuPelayanan"
 MasterPageFile="~/Master.master" %>
 
 <%@ Register Src="WaktuPelayananCtrl.ascx" TagName="WaktuPelayananCtrl" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">

    var showResultText = function(btn, text) {
      Ext.Msg.notify("Button Click", "You clicked the " + btn + 'button and entered the text "' + text + '".');
    };

    var onGridRowUserClick = function( grid) {

      var store = grid.getStore();
      if (!Ext.isEmpty(store)) {
        store.clearFilter();

        store.removeAll();
        store.reload();
      }
    }

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
  <ext:Hidden ID="hfDate" runat="server" />
  <ext:Hidden ID="hfDateY" runat="server" />
  <ext:Hidden ID="hfDateM" runat="server" />
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:FormPanel ID="frmPanel1" runat="server" Layout="ColumnLayout">
        <Items>
          <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit" ColumnWidth="0.75">
              <Items>
                <ext:FormPanel ID="FormPanel1" runat="server" Layout="ColumnLayout">
                  <Items>
                    <ext:Panel ID="Panel4" runat="server" Layout="Fit" ColumnWidth="0.33">
                      <Items>
                        <ext:GridPanel ID="GridPanel2" runat="server" StripeRows="true">
                          <LoadMask ShowMask="true" />
                          <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel3" runat="server" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="store5" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                                <ext:Parameter Name="limit" Value="parseInt(#{ComboBox6}.getValue())" Mode="Raw" />
                                <ext:Parameter Name="model" Value="0303" />
                                <ext:Parameter Name="parameters" Value="[['c_gdg', paramValueGetter(#{hfGdg}) , 'System.Char'],
                                      ['c_type', paramValueGetter(#{hfType}) , 'System.String'],
                                      ['c_modeDay', '03' , 'System.String'],
                                      ['d_pldate', paramRawValueGetter(#{hfDateM}) , 'System.DateTime']]" Mode="Raw" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                                  IDProperty="c_notrans">
                                  <Fields>
                                    <ext:RecordField Name="c_notrans" />
                                    <ext:RecordField Name="c_gdg" />
                                    <ext:RecordField Name="c_type" />
                                    <ext:RecordField Name="v_gdgdesc" />
                                    <ext:RecordField Name="l_new" Type="Boolean" />
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
                                <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                              </ext:CommandColumn>
                              <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="80" />
                              <ext:Column ColumnID="c_notrans" DataIndex="c_notrans" Header="Nomor" Hideable="false" Width="100" />
                              <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Tanggal" Format="dd-MM-yyyy HH:mm:ss" Width="140" />
                            </Columns>
                          </ColumnModel>
                          <View>
                            <ext:GridView ID="GridView3" runat="server" StandardHeaderRow="true">
                              <HeaderRows>
                                <ext:HeaderRow>
                                  <Columns>
                                    <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                      <Component>
                                        <ext:Button ID="Button3" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                          <Listeners>
                                            <Click Handler="clearFilterGridHeader(#{GridPanel2}, #{cbDono3});reloadFilterGrid(#{GridPanel2});"
                                              Buffer="300" Delay="300" />
                                          </Listeners>
                                        </ext:Button>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn >
                                      <Component>
                                        <ext:ComboBox ID="cbGudang3" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                                          Width="300" AllowBlank="true" ForceSelection="false" TypeAhead="false">
                                          <CustomConfig>
                                            <ext:ConfigItem Name="allowBlank" Value="true" />
                                          </CustomConfig>
                                          <Store>
                                            <ext:Store ID="Store6" runat="server" RemotePaging="false">
                                              <Proxy>
                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                  CallbackParam="soaScmsCallback" />
                                              </Proxy>
                                              <BaseParams>
                                                <ext:Parameter Name="allQuery" Value="true" />
                                                <ext:Parameter Name="model" Value="2031" />
                                                <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudang3}), '']]"
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
                                            <Change Handler="reloadFilterGrid(#{GridPanel2})" Buffer="300" Delay="300" />
                                          </Listeners>
                                        </ext:ComboBox>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                      <Component>
                                        <ext:CompositeField ID="CompositeField3" runat="server">
                                          <Items>
                                            <ext:ComboBox ID="cbDono3" runat="server" ItemSelector="tr.search-item"
                                            DisplayField="c_dono" ValueField="c_dono" MinChars="3" PageSize="10" ListWidth="300"
                                            AllowBlank="false" ForceSelection="false" Width="100">
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
                                                  <ext:Parameter Name="limit" Value="={10}" />
                                                  <ext:Parameter Name="model" Value="0305" />
                                                  <ext:Parameter Name="parameters" Value="[['c_gdg', paramValueGetter(#{hfGdg}) , 'System.Char'],
                                                        ['c_type', #{hfType}.getValue() , 'System.String'],
                                                        ['@contains.c_dono.Contains(@0)', paramTextGetter(#{cbDono3}), ''],
                                                        ['c_modeDay', '01' , 'System.String']]"
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
                                            <Template ID="Template2" runat="server">
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
                                    <ext:HeaderColumn >
                                      <Component>
                                        <ext:Button ID="Button4" runat="server" ToolTip="Add" Icon="Accept">
                                          <DirectEvents>
                                            <Click OnEvent="SubmitSelection">
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanel2}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                    <ext:Parameter Name="BtnTipe" Value="1" Mode="Raw" />
                                                    <ext:Parameter Name="katcol" Value="1" Mode="Raw" />
                                                </ExtraParams>
                                                <EventMask ShowMask="true" />
                                            </Click>
                                        </DirectEvents>
                                        </ext:Button>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn />
                                  </Columns>
                                </ext:HeaderRow>
                              </HeaderRows>
                            </ext:GridView>
                          </View>
                          <BottomBar>
                            <ext:PagingToolbar runat="server" ID="PagingToolbar2" PageSize="20">
                              <Items>
                                <ext:Label ID="Label3" runat="server" Text="Page size:" />
                                <ext:ToolbarSpacer ID="ToolbarSpacer3" runat="server" Width="10" />
                                <ext:ComboBox ID="ComboBox6" runat="server" Width="80">
                                  <Items>
                                    <ext:ListItem Text="5" />
                                    <ext:ListItem Text="10" />
                                    <ext:ListItem Text="20" />
                                    <ext:ListItem Text="50" />
                                    <ext:ListItem Text="100" />
                                  </Items>
                                  <SelectedItem Value="20" />
                                  <Listeners>
                                    <Select Handler="#{PagingToolbar2}.pageSize = parseInt(this.getValue()); #{PagingToolbar2}.doLoad();" />
                                  </Listeners>
                                </ext:ComboBox>
                              </Items>
                            </ext:PagingToolbar>
                          </BottomBar>
                        </ext:GridPanel>
                      </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel2" runat="server" Layout="Fit" ColumnWidth="0.33">
                      <Items>
                        <ext:GridPanel ID="GridPanel1" runat="server" StripeRows="true">
                          <LoadMask ShowMask="true" />
                          <Listeners>
                            <Command Handler="if(command == 'Delete') { voidWPDataFromStore(record); }" />
                          </Listeners>
                          <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel2" runat="server" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="store2" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                                <ext:Parameter Name="limit" Value="parseInt(#{ComboBox3}.getValue())" Mode="Raw" />
                                <ext:Parameter Name="model" Value="0303" />
                                <ext:Parameter Name="parameters" Value="[['c_gdg', paramValueGetter(#{hfGdg}) , 'System.Char'],
                                      ['c_type', paramValueGetter(#{hfType}) , 'System.String'],
                                      ['c_modeDay', '02' , 'System.String'],
                                      ['d_pldate = @0', paramRawValueGetter(#{hfDateY}) , 'System.DateTime']]" Mode="Raw" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                                  IDProperty="c_notrans">
                                  <Fields>
                                    <ext:RecordField Name="c_notrans" />
                                    <ext:RecordField Name="c_gdg" />
                                    <ext:RecordField Name="c_type" />
                                    <ext:RecordField Name="v_gdgdesc" />
                                    <ext:RecordField Name="l_new" Type="Boolean" />
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
                                <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                              </ext:CommandColumn>
                              <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="80" />
                              <ext:Column ColumnID="c_notrans" DataIndex="c_notrans" Header="Nomor" Hideable="false" Width="100" />
                              <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Tanggal" Format="dd-MM-yyyy HH:mm:ss" Width="140" />
                            </Columns>
                          </ColumnModel>
                          <View>
                            <ext:GridView ID="GridView2" runat="server" StandardHeaderRow="true">
                              <HeaderRows>
                                <ext:HeaderRow>
                                  <Columns>
                                    <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                      <Component>
                                        <ext:Button ID="Button1" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                          <Listeners>
                                            <Click Handler="clearFilterGridHeader(#{GridPanel1}, #{cbDono2});reloadFilterGrid(#{GridPanel1});"
                                              Buffer="300" Delay="300" />
                                          </Listeners>
                                        </ext:Button>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn >
                                      <Component>
                                        <ext:ComboBox ID="cbGudang2" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                                          Width="300" AllowBlank="true" ForceSelection="false" TypeAhead="false">
                                          <CustomConfig>
                                            <ext:ConfigItem Name="allowBlank" Value="true" />
                                          </CustomConfig>
                                          <Store>
                                            <ext:Store ID="Store3" runat="server" RemotePaging="false">
                                              <Proxy>
                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                  CallbackParam="soaScmsCallback" />
                                              </Proxy>
                                              <BaseParams>
                                                <ext:Parameter Name="allQuery" Value="true" />
                                                <ext:Parameter Name="model" Value="2031" />
                                                <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudang2}), '']]"
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
                                            <Change Handler="reloadFilterGrid(#{GridPanel1})" Buffer="300" Delay="300" />
                                          </Listeners>
                                        </ext:ComboBox>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                      <Component>
                                        <ext:CompositeField ID="CompositeField2" runat="server">
                                          <Items>
                                            <ext:ComboBox ID="cbDono2" runat="server" ItemSelector="tr.search-item"
                                            DisplayField="c_dono" ValueField="c_dono" MinChars="3" PageSize="10" ListWidth="300"
                                            AllowBlank="false" ForceSelection="false" Width="100">
                                            <CustomConfig>
                                              <ext:ConfigItem Name="allowBlank" Value="false" />
                                            </CustomConfig>
                                            <Store>
                                              <ext:Store ID="Store4" runat="server" AutoLoad="false">
                                                <Proxy>
                                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                    CallbackParam="soaScmsCallback" />
                                                </Proxy>
                                                <BaseParams>
                                                  <ext:Parameter Name="start" Value="={0}" />
                                                  <ext:Parameter Name="limit" Value="10" />
                                                  <ext:Parameter Name="model" Value="0305" />
                                                  <ext:Parameter Name="parameters" Value="[['c_gdg', paramValueGetter(#{hfGdg}) , 'System.Char'],
                                                        ['c_type', #{hfType}.getValue() , 'System.String'],
                                                        ['@contains.c_dono.Contains(@0)', paramTextGetter(#{cbDono2}), '']]"
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
                                            <Template ID="Template1" runat="server">
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
                                    <ext:HeaderColumn >
                                      <Component>
                                        <ext:Button ID="Button2" runat="server" ToolTip="Add" Icon="Accept">
                                          <DirectEvents>
                                            <Click OnEvent="SubmitSelection">
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanel1}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                    <ext:Parameter Name="BtnTipe" Value="1" Mode="Raw" />
                                                    <ext:Parameter Name="katcol" Value="2" Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                        <Listeners>
                                          <%--<AfterRender Handler="onGridRowUserClick(#{GridPanel1});" />--%>
                                          <%--<Click Handler="onGridRowUserClick(#{GridPanel1});" />--%>
                                        </Listeners>
                                        </ext:Button>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn />
                                  </Columns>
                                </ext:HeaderRow>
                              </HeaderRows>
                            </ext:GridView>
                          </View>
                          <BottomBar>
                            <ext:PagingToolbar runat="server" ID="PagingToolbar1" PageSize="20">
                              <Items>
                                <ext:Label ID="Label2" runat="server" Text="Page size:" />
                                <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
                                <ext:ComboBox ID="ComboBox3" runat="server" Width="80">
                                  <Items>
                                    <ext:ListItem Text="5" />
                                    <ext:ListItem Text="10" />
                                    <ext:ListItem Text="20" />
                                    <ext:ListItem Text="50" />
                                    <ext:ListItem Text="100" />
                                  </Items>
                                  <SelectedItem Value="20" />
                                  <Listeners>
                                    <Select Handler="#{PagingToolbar1}.pageSize = parseInt(this.getValue()); #{PagingToolbar1}.doLoad();" />
                                  </Listeners>
                                </ext:ComboBox>
                              </Items>
                            </ext:PagingToolbar>
                          </BottomBar>
                        </ext:GridPanel>
                      </Items>
                    </ext:Panel>
                    <ext:Panel ID="Panel3" runat="server" Layout="Fit" ColumnWidth="0.33">
                      <Items>
                        <ext:GridPanel ID="gridMain" runat="server" StripeRows="true">
                          <LoadMask ShowMask="true" />
                          <Listeners>
                            <Command Handler="if(command == 'Delete') { voidWPDataFromStore(record); }" />
                          </Listeners>
                          <SelectionModel>
                            <ext:RowSelectionModel ID="RowSelectionModel1" runat="server" />
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
                                      ['c_type', paramValueGetter(#{hfType}) , 'System.String'],
                                      ['c_modeDay', '01' , 'System.String'],
                                      ['d_pldate = @0', paramRawValueGetter(#{hfDate}) , 'System.DateTime']]" Mode="Raw" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                                  IDProperty="c_notrans">
                                  <Fields>
                                    <ext:RecordField Name="c_notrans" />
                                    <ext:RecordField Name="c_gdg" />
                                    <ext:RecordField Name="c_type" />
                                    <ext:RecordField Name="v_gdgdesc" />
                                    <ext:RecordField Name="l_new" Type="Boolean" />
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
                                  <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                                </Commands>
                                <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                              </ext:CommandColumn>
                              <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="80" />
                              <ext:Column ColumnID="c_notrans" DataIndex="c_notrans" Header="Nomor" Hideable="false" Width="100" />
                              <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Tanggal" Format="dd-MM-yyyy HH:mm:ss" Width="140" />
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
                                            <ext:Store runat="server" RemotePaging="false" >
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
                                            <ext:ComboBox ID="cbDODtl" runat="server" ItemSelector="tr.search-item"
                                            DisplayField="c_dono" ValueField="c_dono" MinChars="3" PageSize="10" ListWidth="300"
                                            AllowBlank="false" ForceSelection="false" Width="100">
                                            <CustomConfig>
                                              <ext:ConfigItem Name="allowBlank" Value="false" />
                                            </CustomConfig>
                                            <Store>
                                              <ext:Store ID="Store1" runat="server" AutoLoad="false">
                                                <Proxy>
                                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                    CallbackParam="soaScmsCallback" />
                                                </Proxy>
                                                <BaseParams>
                                                  <ext:Parameter Name="start" Value="={0}" />
                                                  <ext:Parameter Name="limit" Value="10" />
                                                  <ext:Parameter Name="model" Value="0305" />
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
                                    <ext:HeaderColumn >
                                      <Component>
                                        <ext:Button ID="btnAdd" runat="server" ToolTip="Add" Icon="Accept">
                                          <DirectEvents>
                                            <Click OnEvent="SubmitSelection">
                                                <ExtraParams>
                                                    <ext:Parameter Name="Values" Value="Ext.encode(#{gridMain}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                                    <ext:Parameter Name="BtnTipe" Value="1" Mode="Raw" />
                                                    <ext:Parameter Name="katcol" Value="3" Mode="Raw" />
                                                </ExtraParams>
                                            </Click>
                                        </DirectEvents>
                                        </ext:Button>
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
                </ext:FormPanel>            
              </Items>
          </ext:Panel>
          <ext:Panel ID="Panel1" runat="server" Layout="Fit" ColumnWidth="0.25">
            <Items>
              <ext:Panel ID="Panel5" runat="server" Layout="Fit" ColumnWidth="0.33">
                <Items>
                  <ext:GridPanel ID="GridPanel3" runat="server" StripeRows="true">
                    <LoadMask ShowMask="true" />
                    <Listeners>
                      <Command Handler="if(command == 'Delete') { voidWPDataFromStore(record); }" />
                    </Listeners>
                    <SelectionModel>
                      <ext:RowSelectionModel ID="RowSelectionModel4" runat="server" />
                    </SelectionModel>
                    <Store>
                      <ext:Store ID="store8" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                          <ext:Parameter Name="limit" Value="parseInt(#{ComboBox4}.getValue())" Mode="Raw" />
                          <ext:Parameter Name="model" Value="0304" />
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
                              <ext:RecordField Name="l_new" Type="Boolean" />
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
                          <%--<Commands>
                            <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                          </Commands>--%>
                          <PrepareToolbar Handler="prepareCommands(record, toolbar);" />
                        </ext:CommandColumn>
                        <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="80" />
                        <ext:Column ColumnID="c_notrans" DataIndex="c_notrans" Header="Nomor" Hideable="false" Width="100" />
                        <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Tanggal" Format="dd-MM-yyyy HH:mm:ss" Width="140" />
                      </Columns>
                    </ColumnModel>
                    <View>
                      <ext:GridView ID="GridView4" runat="server" StandardHeaderRow="true">
                        <HeaderRows>
                          <ext:HeaderRow>
                            <Columns>
                              <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                <Component>
                                  <ext:Button ID="Button5" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                    <Listeners>
                                      <Click Handler="clearFilterGridHeader(#{GridPanel3}, #{cbDODtl});reloadFilterGrid(#{gridMain});"
                                        Buffer="300" Delay="300" />
                                    </Listeners>
                                  </ext:Button>
                                </Component>
                              </ext:HeaderColumn>
                              <ext:HeaderColumn >
                                <Component>
                                  <ext:ComboBox ID="ComboBox1" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                                    Width="300" AllowBlank="true" ForceSelection="false" TypeAhead="false">
                                    <CustomConfig>
                                      <ext:ConfigItem Name="allowBlank" Value="true" />
                                    </CustomConfig>
                                    <Store>
                                      <ext:Store ID="Store9" runat="server" RemotePaging="false" >
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
                                  <ext:CompositeField ID="CompositeField4" runat="server">
                                    <Items>
                                      <ext:ComboBox ID="ComboBox2" runat="server" ItemSelector="tr.search-item"
                                      DisplayField="c_dono" ValueField="c_dono" MinChars="3" PageSize="10" ListWidth="300"
                                      AllowBlank="false" ForceSelection="false" Width="100">
                                      <CustomConfig>
                                        <ext:ConfigItem Name="allowBlank" Value="false" />
                                      </CustomConfig>
                                      <Store>
                                        <ext:Store ID="Store10" runat="server" AutoLoad="false">
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
                                      <Template ID="Template3" runat="server">
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
                              <ext:HeaderColumn >
                                <Component>
                                  <ext:Button ID="Button6" runat="server" ToolTip="Add" Icon="Decline">
                                    <DirectEvents>
                                      <Click OnEvent="SubmitSelection">
                                          <ExtraParams>
                                              <ext:Parameter Name="Values" Value="Ext.encode(#{GridPanel3}.getRowsValues({selectedOnly:true}))" Mode="Raw" />
                                              <ext:Parameter Name="BtnTipe" Value="2" Mode="Raw" />
                                          </ExtraParams>
                                      </Click>
                                  </DirectEvents>
                                  </ext:Button>
                                </Component>
                              </ext:HeaderColumn>
                              <ext:HeaderColumn />
                            </Columns>
                          </ext:HeaderRow>
                        </HeaderRows>
                      </ext:GridView>
                    </View>
                    <BottomBar>
                      <ext:PagingToolbar runat="server" ID="PagingToolbar3" PageSize="20">
                        <Items>
                          <ext:Label ID="Label4" runat="server" Text="Page size:" />
                          <ext:ToolbarSpacer ID="ToolbarSpacer4" runat="server" Width="10" />
                          <ext:ComboBox ID="ComboBox4" runat="server" Width="80">
                            <Items>
                              <ext:ListItem Text="5" />
                              <ext:ListItem Text="10" />
                              <ext:ListItem Text="20" />
                              <ext:ListItem Text="50" />
                              <ext:ListItem Text="100" />
                            </Items>
                            <SelectedItem Value="20" />
                            <Listeners>
                              <Select Handler="#{PagingToolbar3}.pageSize = parseInt(this.getValue()); #{PagingToolbar3}.doLoad();" />
                            </Listeners>
                          </ext:ComboBox>
                        </Items>
                      </ext:PagingToolbar>
                    </BottomBar>
                  </ext:GridPanel>
                </Items>
              </ext:Panel>
            </Items>
          </ext:Panel>
        </Items>
      </ext:FormPanel>
    </Items>
  </ext:Viewport>
  <uc:WaktuPelayananCtrl ID="WaktuPelayananCtrl" runat="server" />
</asp:Content>