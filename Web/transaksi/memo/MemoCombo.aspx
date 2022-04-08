<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="MemoCombo.aspx.cs" Inherits="transaksi_memo_MemoCombo" %>

<%@ Register Src="MemoComboCtrl.ascx" TagName="MemoComboCtrl" TagPrefix="uc" %>
<%@ Register Src="MemoComboPrintCtrl.ascx" TagName="MemoComboPrintCtrl" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
    
  <script type="text/javascript">
    var voidCOMBODataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_gdg'), rec.get('c_combono'), txt);
                }
              });
          }
        });
    }
    var prepareCommandsParent = function(record, toolbar) {
      var accp = toolbar.items.get(0); // accept button

      var isConfirm = false;

      if (!Ext.isEmpty(record)) {
        isConfirm = record.get('l_confirm');
      }

      if (isConfirm) {
        accp.setVisible(false);
      }
      else {
        accp.setVisible(true);
      }
    }
    var submitComboData = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      ShowConfirm('Konfirmasi ?', 'Apakah anda yakin ingin memproses nomor ini ?',
        function(btn) {
          if (btn == 'yes') {
            //dm.SubmitMethod(rec.get('c_combono'));
            Ext.net.DirectMethods.SubmitMethod(rec.get('c_gdg'), rec.get('c_combono'));
          }
        });
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
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
              <ext:Button ID="btnPrintPL" runat="server" Text="Cetak" Icon="Printer">
                <DirectEvents>
                  <Click OnEvent="btnPrintCombo_OnClick">
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
              <Command Handler="if(command == 'Delete') { voidCOMBODataFromStore(record); } else if(command == 'Submit') { submitComboData(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_combono" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_combono" Mode="Raw" />
                  <ext:Parameter Name="GudangID" Value="record.data.c_gdg" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridCB" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0046" />
                  <ext:Parameter Name="parameters" Value="[['typeCode', '01', 'System.String'],
                    ['c_gdg = @0', paramValueGetter(#{cbGudangFltr}), 'System.Char'],
                    ['c_combono', paramRawValueGetter(#{txComboIDFltr}) + '%', ''],
                    ['d_combodate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_iteno = @0', paramValueGetter(#{cbItemDtl}) , 'System.String'],
                    ['c_batch', paramRawValueGetter(#{txComboIDFltr}) + '%', ''],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_combono">
                    <Fields>
                      <ext:RecordField Name="c_gdg" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="c_combono" />
                      <ext:RecordField Name="c_batch" />
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="c_memono" />
                      <ext:RecordField Name="n_acc" Type="Float" />
                      <ext:RecordField Name="n_bqty" Type="Float" />
                      <ext:RecordField Name="n_bsisa" Type="Float" />
                      <ext:RecordField Name="n_gqty" Type="Float" />
                      <ext:RecordField Name="n_gsisa" Type="Float" />
                      <ext:RecordField Name="d_combodate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="l_confirm" Type="Boolean" />
                      <ext:RecordField Name="v_itnam" />
                      <ext:RecordField Name="v_ket" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_combono" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false" ButtonAlign="Center">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" Hidden="false" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="120" />
                <ext:Column ColumnID="c_combono" DataIndex="c_combono" Header="Nomor" />
                <ext:DateColumn ColumnID="d_combodate" DataIndex="d_combodate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Item" Width="250" />
                <ext:Column ColumnID="c_batch" DataIndex="c_batch" Header="Batch" Width="125" />
                <ext:NumberColumn ColumnID="n_box" DataIndex="n_box" Header="Box" Format="0.000,00/i"
                  Width="50" />
                <ext:NumberColumn ColumnID="n_acc" DataIndex="n_acc" Header="Diterima" Format="0.000,00/i"
                  Width="75" />
                <ext:NumberColumn ColumnID="n_gqty" DataIndex="n_gqty" Header="Jumlah" Format="0.000,00/i"
                  Width="75" />
                <ext:CommandColumn ColumnID="l_confirm" DataIndex="l_confirm" Header="Konf." Width="50"
                  ButtonAlign="Center">
                  <Commands>
                    <ext:GridCommand CommandName="Submit" Icon="Accept" ToolTip-Title="Konfirmasi" ToolTip-Text="Confirm Combo" />
                  </Commands>
                  <PrepareToolbar Handler="prepareCommandsParent(record, toolbar);" />
                </ext:CommandColumn>
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{cbGudangFltr}, #{txComboIDFltr}, #{txDateFltr}, #{cbItemDtl}, #{txBatchIDFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbGudangFltr" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="250" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store runat="server">
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
                                  <ext:Parameter Name="model" Value="2031" />
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudangFltr}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_gdgdesc" />
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
                            <Template runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="1" style="width: 250px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                              <tpl for="."><tr class="search-item">
                              <td>{c_gdg}</td><td>{v_gdgdesc}</td>
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
                          <ext:TextField ID="txComboIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                          <ext:ComboBox ID="cbItemDtl" runat="server" DisplayField="v_itnam" ValueField="c_iteno"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store runat="server">
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
                                  <ext:Parameter Name="model" Value="2061" />
                                  <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                    ['l_combo = @0', true, 'System.Boolean'],
                                    ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItemDtl}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_itnam" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader IDProperty="c_iteno" Root="d.records" SuccessProperty="d.success"
                                    TotalProperty="d.totalRows">
                                    <Fields>
                                      <ext:RecordField Name="c_iteno" />
                                      <ext:RecordField Name="v_itnam" />
                                    </Fields>
                                  </ext:JsonReader>
                                </Reader>
                              </ext:Store>
                            </Store>
                            <Template runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="0" style="width: 400px">
                              <tr>
                              <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                              </tr>
                              <tpl for="."><tr class="search-item">
                              <td>{c_iteno}</td><td>{v_itnam}</td>
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
                          <ext:TextField ID="txBatchIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
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
    </Items>
  </ext:Viewport>
  <uc:MemoComboCtrl ID="MemoComboCtrl1" runat="server" />
  <uc:MemoComboPrintCtrl ID="MemoComboPrintCtrl1" runat="server" />
</asp:Content>
