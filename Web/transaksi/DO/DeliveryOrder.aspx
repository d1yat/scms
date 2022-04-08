<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeliveryOrder.aspx.cs" 
Inherits="transaksi_DO_DeliveryOrder" MasterPageFile="~/Master.master" %>

<%@ Register Src="DOPLCtrl.ascx" TagName="DOPLCtrl" TagPrefix="uc1" %>
<%@ Register Src="DOSTTCtrl.ascx" TagName="DOSTTCtrl" TagPrefix="uc2" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfType" runat="server" />
  <ext:Hidden ID="hfTypeName" runat="server" />
  <ext:Hidden ID="hfMode" runat="server" />
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="Panel1" runat="server" Layout="Fit">
        <Content>
          <ext:Hidden ID="hfDONo" runat="server" />
          <ext:Hidden ID="hfStoreID" runat="server" />
        </Content>
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
              <ext:Button ID="btnAddNew" runat="server" Text="Add" Icon="Add">
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
            </Items>
            
          </ext:Toolbar>
        </TopBar>
        <Items>
           <ext:GridPanel ID="gridMain" runat="server">       
            <LoadMask ShowMask="true" />
              <Listeners>
                <Command Handler="if(command == 'Delete') { voidDODataFromStore(record, #{DirectMethods}); }" />
              </Listeners>
              <DirectEvents>
                <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                    <ext:Parameter Name="Parameter" Value="c_dono" />
                    <ext:Parameter Name="PrimaryID" Value="record.data.c_dono" Mode="Raw" />
                  </ExtraParams>
                </Command>
              </DirectEvents>
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                  <ext:Store ID="storeGridPL" runat="server" SkinID="OriginalExtStore" RemotePaging="true" RemoteSort="true">
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
                        <ext:Parameter Name="parameters" 
                              Value="[['c_dono', paramValueGetter(#{txNoFltr}) + '%', ''],
                                      ['d_dodate = @0', paramValueGetter(#{txDODate}) , 'System.DateTime'],
                                      ['c_typeRef = @0', paramValueGetter(#{hfType}) , 'System.String'],
                                      ['c_cusno = @0', paramValueGetter(#{txCustomer}) , 'System.String'],
                                      ['c_type = @0', paramValueGetter(#{cbViaHdr}) , 'System.String'],
                                      ['c_plno' , paramValueGetter(#{txNoPLFltr}) + '%',''],
                                      ['c_expno' ,paramValueGetter(#{txNoEksFltr}) + '%',''],
                                      ['c_gdg = @0', paramValueGetter(#{cbGudang}) , 'System.Char']]" 
                              Mode="Raw" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_dono"
                          Root="d.records" SuccessProperty="d.success" 
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_dono" />
                            <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
                            <ext:RecordField Name="v_gdgdesc" />
                            <ext:RecordField Name="v_cunam" />
                            <ext:RecordField Name="v_ket" />
                            <ext:RecordField Name="c_plno" />
                            <ext:RecordField Name="c_pin" />
                            <ext:RecordField Name="c_expno" />
                            <ext:RecordField Name="l_confirm" />
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
                      <ext:GridCommand CommandName="Delete" Icon="Delete" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                    </Commands>
                  </ext:CommandColumn>
                  <ext:Column ColumnID="c_dono" DataIndex="c_dono" Header="Nomor DO" Hideable="false" />
                <ext:DateColumn ColumnID="d_dodate" DataIndex="d_dodate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width = "120" />
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="150" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="VIA" Width="80" />
                <ext:Column ColumnID="c_plno" DataIndex="c_plno" Header="No Ref" />
                <ext:Column ColumnID="c_pin" DataIndex="c_pin" Header="Pin" />
                <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="No Ekspedisi" Width="100" />
                <ext:CheckColumn ColumnID="l_confirm" DataIndex="l_confirm" Header="Confirm" />
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
                                <Click Handler="clearFilterGridHeader(#{GridMain}, #{txEXPFltr},#{txDODate},#{cbGudang},#{txCustomer},#{cbViaHdr},#{txNoPLFltr},#{txNoEksFltr});reloadFilterGrid(#{gridMain});"
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
                            <ext:DateField ID="txDODate" runat="server" EnableKeyEvents="true" AllowBlank="true">
                              <Listeners>
                                <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                              </Listeners>
                            </ext:DateField>
                          </Component>
                        </ext:HeaderColumn>
                        <ext:HeaderColumn>
                           <Component>
                            <ext:ComboBox ID="cbGudang" runat="server" DisplayField="v_gdgdesc"
                              ValueField="c_gdg" Width="300" TypeAhead="false" AllowBlank="false" ForceSelection="false">
                              <Store>
                                <ext:Store runat="server" RemotePaging="false">
                                  <CustomConfig>
                                    <ext:ConfigItem Name="allowBlank" Value="false" />
                                  </CustomConfig>
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="model" Value="2031" />
                                    <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudang}), '']]" Mode="Raw" />
                                    <ext:Parameter Name="sort" Value="c_gdg" />
                                    <ext:Parameter Name="dir" Value="ASC" />
                                  </BaseParams>
                                  <Reader>
                                    <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                                      TotalProperty="d.totalRows">
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
                            <ext:ComboBox ID="txCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                                Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                                AllowBlank="true" ForceSelection="false">
                              <Store>
                                <ext:Store runat="server" >
                                  <CustomConfig>
                                    <ext:ConfigItem Name="allowBlank" Value="true" />
                                  </CustomConfig>
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                   </Proxy>
                                 <BaseParams>
                                      <ext:Parameter Name="start" Value="={0}" />
                                      <ext:Parameter Name="limit" Value="={10}" />
                                      <ext:Parameter Name="model" Value="2011" />
                                      <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                      ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{txCustomer}), '']]"
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
                            <ext:ComboBox ID="cbViaHdr" runat="server" DisplayField="v_ket"
                              ValueField="c_type" Width="250" AllowBlank="false" TypeAhead="false" ForceSelection="false">
                              <Store>
                                <ext:Store runat="server" RemotePaging="false">
                                  <CustomConfig>
                                    <ext:ConfigItem Name="allowBlank" Value="false" />
                                  </CustomConfig>
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="model" Value="2001" />
                                    <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                              ['c_notrans = @0', '02', ''],
                                                              ['@contains.c_type.Contains(@0) || @contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaHdr}), '']]" Mode="Raw" />
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
                        <ext:HeaderColumn />
                        <ext:HeaderColumn>
                          <Component>
                            <ext:TextField ID="txNoEksFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                              <Listeners>
                                <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                              </Listeners>
                            </ext:TextField>
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
  <uc1:DOPLCtrl ID="DOPLCtrl" runat="server" />
  <uc2:DOSTTCtrl ID="DOSTTCtrl" runat="server" />
</asp:Content>

