<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TransferGudangPemusnahan.aspx.cs"
  Inherits="transaksi_transfer_TransferGudangPemusnahan" MasterPageFile="~/Master.master" %>

<%@ Register Src="TransferGudangPemusnahanCtrl.ascx" TagName="TransferGudangPemusnahanCtrl"
  TagPrefix="uc" %>
<%--<%@ Register Src="TransferGudangPemusnahanCtrlPrint.ascx" TagName="TransferGudangRepackCtrlPrint"
  TagPrefix="uc" %>--%>
  <%@ Register Src="TransferGudangPemusnahanConfirmCtrl.ascx" TagName="TransferGudangPemusnahanConfirmCtrl"
   TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidSJDataFromStorePemusnahan = function(rec) {
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

                    Ext.net.DirectMethods.DeleteMethod(rec.get('c_sjno'), txt);
                  }
                });
            }
          });
    }
    
    var cekConfirmModePemusnahan = function(hf) {
      if (Ext.isEmpty(hf)) {
        return;
      }

      var v = hf.getValue();
      if (v == 'CF') {
        return false;
      }

      return;
    }

    var cekPrintModePemusnahan = function(hf) {
      if (Ext.isEmpty(hf)) {
        return;
      }

      var v = hf.getValue();
      if (v == 'CF') {
        return true;
      }

      return;
    }
    var prepareCommandsPagePemusnahan = function(toolbar, valX) {
      var vd = toolbar.items.get(1); // void button

      var isHideConfirm = (valX == 'CF' ? false : true);

      vd.setVisible(isHideConfirm);
    } 
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfGdg" runat="server" />
  <ext:Hidden ID="hfMode" runat="server" />
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
              <ext:Button ID="btnAddNew" Text="Tambah" Icon="Add" runat="server">
                <DirectEvents>
                  <Click OnEvent="btnAddNew_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
              <ext:ToolbarSeparator />
              <ext:Button ID="btnPrintSJ" runat="server" Text="Cetak" Icon="Printer">
                <DirectEvents>
                  <Click>
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
            <Listeners>
              <%--<Command Handler="if(command == 'Delete') { voidSJDataFromStorePemusnahan(record, #{DirectMethods}); }" />--%>
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_sjno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_sjno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridSJ" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0020" />
                  <ext:Parameter Name="parameters" Value="[['c_sjno', paramValueGetter(#{txSJFltr}) + '%', ''],
                        ['c_sjno', paramValueGetter(#{txSJFltr}) + '%', ''],
                        ['c_product_origin = @0', paramValueGetter(#{cbAsalProdukFltr}) , 'System.String'],
                        ['v_nodok', paramValueGetter(#{txNodokFltr}) + '%', ''],
                        ['c_gdg = @0', '1' , 'System.Char'],
                        ['c_gdg2 = @0', '5' , 'System.Char'],
                        ['c_type = @0', '02' , 'System.String']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_sjno">
                    <Fields>
                      <ext:RecordField Name="v_from" />
                      <ext:RecordField Name="c_sjno" />
                      <ext:RecordField Name="d_sjdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_to" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="l_print" Type="Boolean" />
                      <ext:RecordField Name="l_status" Type="Boolean" />
                      <ext:RecordField Name="l_confirm" Type="Boolean" />
                      <ext:RecordField Name="v_ket_product_origin" />
                      <ext:RecordField Name="v_nodok" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_sjno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                  </Commands>
                  <PrepareToolbar Handler="prepareCommandsPagePemusnahan(toolbar, #{hfMode}.getValue());" />
                </ext:CommandColumn>
                <ext:Column ColumnID="v_from" DataIndex="v_from" Header="From" />
                <ext:Column ColumnID="c_sjno" DataIndex="c_sjno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_sjdate" DataIndex="d_sjdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_to" DataIndex="v_to" Header="To" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" />
                <ext:CheckColumn ColumnID="l_print" DataIndex="l_print" Header="Print" Width="50" />
                <ext:CheckColumn ColumnID="l_status" DataIndex="l_status" Header="Status" Width="50" />
                <ext:CheckColumn ColumnID="l_confirm" DataIndex="l_confirm" Header="Confirm" Width="50" />
                <ext:Column ColumnID="v_ket_product_origin" DataIndex="v_ket_product_origin" Header="Asal Produk" Width="150" />
                <ext:Column ColumnID="v_nodok" DataIndex="v_nodok" Header="No. Dokumen" Width="100" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txSJFltr}, #{txDateFltr},#{txNodokFltr}, #{cbAsalProdukFltr},#{cbGudangFltrFrom},#{cbGudangFltrTo} );reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <%--<Component>
                          <ext:ComboBox ID="cbGudangFltrFrom" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="300" AllowBlank="true" TypeAhead="false" ForceSelection="false">
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
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudangFltrFrom}), '']]"
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
                        </Component>--%>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txSJFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDateFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbGudangFltrTo" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
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
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudangFltrTo}), '']]"
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
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                          <Component>
                            <ext:ComboBox ID="cbAsalProdukFltr" runat="server" ValueField="c_type"
                              DisplayField="v_ket" Width="200" AllowBlank="false">
                              <Store>
                                <ext:Store ID="Store2" runat="server">
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="model" Value="2001" />
                                    <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                              ['c_notrans = @0', '62', '']]" Mode="Raw" />
                                    <ext:Parameter Name="sort" Value="c_type" />
                                    <ext:Parameter Name="dir" Value="ASC" />
                                  </BaseParams>
                                  <Reader>
                                    <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                                      <Fields>
                                        <ext:RecordField Name="c_type" />
                                        <ext:RecordField Name="v_ket" />
                                      </Fields>
                                    </ext:JsonReader>
                                  </Reader>
                                </ext:Store>
                              </Store>
                              <Triggers>
                                <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                              </Triggers>
                              <Listeners>
                                <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                              </Listeners>
                            </ext:ComboBox>
                          </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNodokFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
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
    </Items>
  </ext:Viewport>
  <uc:TransferGudangPemusnahanCtrl ID="TransferGudangPemusnahanCtrl" runat="server" />
  <%--<uc:TransferGudangRepackCtrlPrint ID="TransferGudangRepackCtrlPrint" runat="server" />--%>
  <uc:TransferGudangPemusnahanConfirmCtrl ID="TransferGudangPemusnahanConfirmCtrl" runat="server" />
</asp:Content>
