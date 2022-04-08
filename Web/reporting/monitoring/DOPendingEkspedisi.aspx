<%@ Page Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="DOPendingEkspedisi.aspx.cs"
  Inherits="transaksi_pengiriman_Ekspedisi" %>

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
                  <ext:Parameter Name="model" Value="0182" />
                  <ext:Parameter Name="parameters" Value="[['c_gdg = @0', paramValueGetter(#{cbGudang}) , 'System.Char'],
                    ['c_expno', paramValueGetter(#{txEXPFltr}) + '%', ''],
                    ['c_dono' , paramValueGetter(#{txDOSJFltr}) + '%', '']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                    <Fields>
                      <ext:RecordField Name="v_gdgdesc" />                    
                      <ext:RecordField Name="c_expno" />
                      <ext:RecordField Name="d_resi" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="c_dono" />
                      <ext:RecordField Name="d_doentry" Type="Date" DateFormat="M$" />                      
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
                    <%--<ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />--%>
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="120" />                
                <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="Nomor Ekspedisi" Hideable="false" Width="120" />
                <ext:DateColumn ColumnID="d_resi" DataIndex="d_resi" Header="Waktu Resi" Format="dd-MM-yyyy hh:mm:ss" Width="150"/>
                <ext:Column ColumnID="c_dono" DataIndex="c_dono" Header="Nomor DO/SJ" Width="120" />
                <ext:DateColumn ColumnID="d_doentry" DataIndex="d_doentry" Header="Waktu Input DO/SJ" Format="dd-MM-yyyy hh:mm:ss" Width="150"/>
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
                              <Click Handler="clearFilterGridHeader(#{GridMain}, #{cbGudang}, #{txEXPFltr}, #{txDOSJFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
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
                              <ext:Store ID="Store4" runat="server" RemotePaging="false">
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
                          <ext:TextField ID="txEXPFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txDOSJFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
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
</asp:Content>
