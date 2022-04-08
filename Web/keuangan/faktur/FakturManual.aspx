<%@ Page Title="" Language="C#"  MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="FakturManual.aspx.cs" Inherits="faktur_manual" %>

<%@ Register Src="FakturManualCtrl.ascx" TagName="FakturManualCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

<script type="text/javascript">

    var prepareCommands = function(toolbar) {
//        var del = toolbar.items.get(0); // delete button
        var del = toolbar.items.get(1); // void button

        del.setVisible(true);
//        vd.setVisible(true);
    };
    
  var voidFMData = function(rec, dm) {
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
                      
                      Ext.net.DirectMethods.DeleteMethod(rec.get('c_fmno'), txt);
                    }
                  });
              }
            });
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
              <ext:Button ID="btnAddNew" runat="server" Text="Tambah" Icon="Add">
                <DirectEvents>
                  <Click OnEvent="btnAddNew_OnClick">
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
            <Listeners>
                <Command Handler="if(command == 'Delete') { voidFMData(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_fmno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_fmno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridFaktur" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0200" /> 
                  <ext:Parameter Name="parameters" Value="[['c_fmno', paramValueGetter(#{txfmnoFltr}) + '%', ''],
                    ['c_nosup', paramValueGetter(#{cbPrincipalFltr}) + '%', ''],
                    ['c_taxno', paramValueGetter(#{txtaxnoFltr}) + '%', ''],
                    ['d_taxdate = @0', paramRawValueGetter(#{txtaxdateFltr}) , 'System.DateTime']
                    ]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_fmno">
                    <Fields>
                      <ext:RecordField Name="c_fmno" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="c_taxno" />
                      <ext:RecordField Name="d_taxdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="n_dpp" Type="Float" />
                      <ext:RecordField Name="n_ppn" Type="Float" />
                      <ext:RecordField Name="n_total" />
                      <ext:RecordField Name="v_ref" />
                      <ext:RecordField Name="v_ket" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_fmno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                  </Commands>
                  <PrepareToolbar Handler="prepareCommands(toolbar);" />
                </ext:CommandColumn>
                <ext:Column ColumnID="c_fmno" DataIndex="c_fmno" Header="No. FM" Width="75" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Supplier" Width="160" />
                <ext:Column ColumnID="c_taxno" DataIndex="c_taxno" Header="No. Pajak" Width="150" />
                <ext:DateColumn ColumnID="d_taxdate" DataIndex="d_taxdate" Header="Tanggal Pajak" Format="dd-MM-yyyy" />
                <ext:NumberColumn ColumnID="n_dpp" DataIndex="n_dpp" Header="DPP" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_ppn" DataIndex="n_ppn" Header="PPN" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_total" DataIndex="n_total" Header="Total" Format="0.000,00/i" />
                <ext:Column ColumnID="v_ref" DataIndex="v_ref" Header="Referensi" Width="120" />
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" Width="250" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txfmnoFltr}, #{cbPrincipalFltr}, #{txtaxnoFltr}, #{txtaxdateFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                       <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txfmnoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbPrincipalFltr" runat="server" DisplayField="v_nama"
                              ValueField="c_nosup" Width="250" ItemSelector="tr.search-item" ListWidth="350"
                              MinChars="3" AllowBlank="true" ForceSelection="false">
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
                                    <ext:Parameter Name="limit" Value="-1" />
                                    <ext:Parameter Name="model" Value="14014" />
                                    <ext:Parameter Name="parameters" Value="[['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalFltr}), '']]"
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
                                <table cellpading="0" cellspacing="0" style="width: 350px">
                                <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                                <tpl for="."><tr class="search-item">
                                    <td>{c_nosup}</td><td>{v_nama}</td>
                                </tr></tpl>
                                </table>
                                </Html>
                              </Template>
                              <Triggers>
                                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                              </Triggers>
                              <Listeners>
                                <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                              </Listeners>
                            </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txtaxnoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txtaxdateFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:DateField>
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
  </ext:Viewport>
  <uc:FakturManualCtrl runat="server" ID="FakturManualCtrl1" />
</asp:Content>
