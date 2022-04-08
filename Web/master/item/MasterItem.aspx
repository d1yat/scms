<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MasterItem.aspx.cs" Inherits="master_item_MasterItem"
  MasterPageFile="~/Master.master" %>

<%@ Register Src="MasterItemCtrl.ascx" TagName="MasterItemCtrl" TagPrefix="uc" %>
<%@ Register Src="MasterItemPrintCtrl.ascx" TagName="MasterItemPrintCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar ID="Toolbar1" runat="server">
            <Items>
              <ext:Button ID="btnPrint" runat="server" Text="Cetak" Icon="Printer">
                <DirectEvents>
                  <Click OnEvent="btnPrint_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
              <ext:ToolbarSeparator />
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
                  <ext:Parameter Name="model" Value="0136" />
                  <ext:Parameter Name="parameters" Value="[['@contains.c_iteno.Contains(@0)', paramRawValueGetter(#{txItemIDFltr}), ''],
                              ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean'],
                              ['l_aktif = @0', true, 'System.Boolean'],
                              ['l_hide = @0', false, 'System.Boolean'],                              
                              ['c_nosup = @0', paramValueGetter(#{cbPrincipalFltr}) , 'System.String'],
                              ['@contains.v_itnam.Contains(@0)', paramRawValueGetter(#{txItemNameFltr}), ''],
                              ['c_type = @0', paramValueGetter(#{cbTipeJenisFltr}) , 'System.String'],
                              ['c_via = @0', paramValueGetter(#{cbTipeViaFltr}) , 'System.String'],
                              ['@contains.v_komposisi.Contains(@0)', paramRawValueGetter(#{txKomposisiFltr}), ''],
                              ['c_kddivams = @0', paramValueGetter(#{cbDivAms}) , 'System.String'],
                              ['c_kddivpri = @0', paramValueGetter(#{cbDivPrinsipal}) , 'System.String']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_iteno">
                    <Fields>
                      <ext:RecordField Name="c_nosup" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="c_alkes" />
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="n_salpri" Type="Float" />
                      <ext:RecordField Name="n_disc" Type="Float" />
                      <ext:RecordField Name="v_itnam" />
                      <ext:RecordField Name="l_status" Type="Boolean" />
                      <ext:RecordField Name="c_type" />
                      <ext:RecordField Name="Jenis" />
                      <ext:RecordField Name="Via" />
                      <ext:RecordField Name="v_undes" />
                      <ext:RecordField Name="c_itenopri" />
                      <ext:RecordField Name="n_box" />
                      <ext:RecordField Name="v_komposisi" />                       
                      <ext:RecordField Name="c_kddivams" />
                      <ext:RecordField Name="v_nmdivams" />
                      <ext:RecordField Name="c_kddivpri" />
                      <ext:RecordField Name="v_nmdivpri" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="v_itnam" Direction="ASC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />--%>
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode" Width="50" />
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama" Width="200" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama Supplier" Width="150" />
                <ext:NumberColumn ColumnID="n_salpri" DataIndex="n_salpri" Header="Harga" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_disc" DataIndex="n_disc" Header="Disc" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_box" DataIndex="n_box" Header="Box" Format="0.000,00/i" />
                <ext:Column ColumnID="Jenis" DataIndex="Jenis" Header="Jenis" />
                <ext:Column ColumnID="Via" DataIndex="Via" Header="Via" />
                <ext:Column ColumnID="v_komposisi" DataIndex="v_komposisi" Header="Komposisi" Width="300" />
                <ext:Column ColumnID="v_nmdivams" DataIndex="v_nmdivams" Header="Nama Div Ams" Width="150" />
                <ext:Column ColumnID="v_nmdivpri" DataIndex="v_nmdivpri" Header="Nama Div Prinsiple" Width="150" />                
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, 
                                                                #{txItemIDFltr}, 
                                                                #{cbPrincipalFltr}, 
                                                                #{txItemNameFltr}, 
                                                                #{cbTipeJenisFltr}, 
                                                                #{cbTipeViaFltr}, 
                                                                #{txKomposisiFltr},
                                                                #{cbDivAms},
                                                                #{cbDivPrinsipal}
                                                                );reloadFilterGrid(#{gridMain});"
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
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbTipeJenisFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store1" runat="server">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="={0}" />
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2001" />
                                  <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '11', 'System.String'],
                                    ['c_portal = @0', '3', 'System.Char']]" Mode="Raw" />
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbTipeViaFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store2" runat="server">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="={0}" />
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2001" />
                                  <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '02', 'System.String'],
                                    ['c_portal = @0', '3', 'System.Char']]" Mode="Raw" />
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txKomposisiFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                           <ext:ComboBox ID="cbDivAms" runat="server"  ValueField="c_kddivams"
                              DisplayField="v_nmdivams" Width="500" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
                              AllowBlank="true">
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
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="model" Value="2041" />
                                    <%--<ext:Parameter Name="parameters" Value="[['@contains.c_kddivams.Contains(@0) || @contains.v_nmdivams.Contains(@0)', paramTextGetter(#{cbDivAms}), '']]"
                                      Mode="Raw" />--%>
                                      
                                      <ext:Parameter Name="parameters" Value="[ ['l_aktif = @0', true, 'System.Boolean'],
                                                                                ['l_hide = @0', false, 'System.Boolean'], 
                                                                               ['@contains.v_nmdivams.Contains(@0) || @contains.c_kddivams.Contains(@0)', paramTextGetter(#{cbDivAms}), '']]"
                                                                       Mode="Raw" />
                                      
                                    <ext:Parameter Name="sort" Value="c_kddivams" />
                                    <ext:Parameter Name="dir" Value="ASC" />
                                  </BaseParams>
                                  <Reader>
                                    <ext:JsonReader IDProperty="c_kddivams" Root="d.records" SuccessProperty="d.success"
                                      TotalProperty="d.totalRows">
                                      <Fields>
                                        <ext:RecordField Name="c_kddivams" />
                                        <ext:RecordField Name="v_nmdivams" />
                                        <%--<ext:RecordField Name="v_divams_desc" />--%>
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
                                    <td>{c_kddivams}</td><td>{v_nmdivams}</td>
                                  </tr>
                                </tpl>
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
                            <ext:combobox ID="cbDivPrinsipal" runat="server"  ValueField="c_kddivpri"
                              DisplayField="v_nmdivpri" Width="500" ListWidth="500" WrapBySquareBrackets="true"
                              PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" Delimiter=";">
                              <CustomConfig>
                                <ext:ConfigItem Name="allowBlank" Value="true" />
                              </CustomConfig>
                              <Store>
                                <ext:Store ID="Store5" runat="server">
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="start" Value="={0}" />
                                    <ext:Parameter Name="limit" Value="={10}" />
                                    <ext:Parameter Name="model" Value="2051" />
                                    <%--<ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />--%>
                                    <ext:Parameter Name="parameters" Value="[['@contains.v_nmdivpri.Contains(@0) || @contains.c_kddivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]"
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
                              <Template ID="Template4" runat="server">
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
                                            
                              
                              <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                            
                            </ext:combobox>      
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
                </Items>
              </ext:PagingToolbar>
            </BottomBar>
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  <uc:MasterItemCtrl runat="server" ID="MasterItemCtrl1" />
  <uc:MasterItemPrintCtrl ID="MasterItemPrintCtrl1" runat="server" />
</asp:Content>