﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="FakturJual.aspx.cs" Inherits="keuangan_faktur_FakturJual" %>

<%@ Register Src="FakturJualCtrl.ascx" TagName="FakturJualCtrl" TagPrefix="uc" %>
<%@ Register Src="FakturJualPrintCtrl.ascx" TagName="FakturJualPrintCtrl" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidDataFromMainStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_fjno'), txt);
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
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
              <ext:Button ID="btnPrintFaktur" runat="server" Text="Cetak" Icon="Printer">
                <DirectEvents>
                  <Click OnEvent="btnPrintFaktur_OnClick">
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
              <ext:ToolbarSeparator />
              <ext:Button ID="btnGenerate" runat="server" Text="Generate" Icon="CogStart" ToolTip="Generate nomor pajak subdis" >
                <DirectEvents>
                  <Click OnEvent="btnGenerate_OnClick">
                  <Confirmation ConfirmRequest="true" Title="Generate" Message="Generate nomor pajak subdis?" />
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel ID="gridMain" runat="server">
            <LoadMask ShowMask="true" />
            <Listeners>
              <Command Handler="if(command == 'Delete') { voidDataFromMainStore(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_fjno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_fjno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridMain" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0100" />
                  <ext:Parameter Name="parameters" Value="[['c_fjno', paramRawValueGetter(#{txFakturIDFltr}) + '%', ''],
                    ['d_fjdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                    ['c_dono', paramValueGetter(#{txRefIDFltr}) + '%', ''],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean'],
                    ['(subdis == null ? false : subdis) = @0', paramValueGetter(#{cbSubdisFltr}) , 'System.Boolean']
                    ]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_fjno">
                    <Fields>
                      <ext:RecordField Name="c_fjno" />
                      <ext:RecordField Name="d_fjdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="c_cusno" />
                      <ext:RecordField Name="v_cunam" />
                      <ext:RecordField Name="n_net" Type="Float" />
                      <ext:RecordField Name="n_sisa" Type="Float" />
                      <ext:RecordField Name="c_dono" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="c_taxno" />
                      <ext:RecordField Name="subdis" Type="Boolean" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="d_fjdate" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_fjno" DataIndex="c_fjno" Header="Nomor" />
                <ext:DateColumn ColumnID="d_fjdate" DataIndex="d_fjdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="c_dono" DataIndex="c_dono" Header="No. Delivery" />
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Cabang" Width="250" />
                <ext:NumberColumn ColumnID="n_net" DataIndex="n_net" Header="N E T" Format="0.000,00/i" />
                <ext:NumberColumn ColumnID="n_sisa" DataIndex="n_sisa" Header="Sisa" Format="0.000,00/i" />
                <ext:Column ColumnID="c_taxno" DataIndex="c_taxno" Header="No. Pajak" />
                <ext:CheckColumn ColumnID="subdis" DataIndex="subdis" Header="Subdis" Width="70" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txFakturIDFltr}, #{txDateFltr}, #{txRefIDFltr}, #{cbCustomerFltr}, #{txVcIDFltr}, #{cbSubdisFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txFakturIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                          <ext:TextField ID="txRefIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                            AllowBlank="false" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store runat="server">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="={0}" />
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2011" />
                                  <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                    ['l_stscus = @0', true, 'System.Boolean'],['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]" Mode="Raw" />
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
                            <Template runat="server">
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
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:SelectBox ID="cbSubdisFltr" runat="server" AllowBlank="true">
                          <CustomConfig>
                            <ext:ConfigItem Name="allowBlank" Value="true" />
                          </CustomConfig>
                          <Items>
                            <ext:ListItem Value="true" Text="Ya" />
                            <ext:ListItem Value="false" Text="Tidak" />
                          </Items>
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                          </Listeners>
                          </ext:SelectBox>
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
  <uc:FakturJualCtrl ID="FakturJualCtrl1" runat="server" />
  <uc:FakturJualPrintCtrl ID="FakturJualPrintCtrl1" runat="server" />
</asp:Content>
