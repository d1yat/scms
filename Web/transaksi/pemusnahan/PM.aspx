﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PM.aspx.cs" Inherits="transaksi_pemusnahan_PM"
  MasterPageFile="~/Master.master" %>

<%@ Register Src="PMCtrl.ascx" TagName="PMCtrl" TagPrefix="uc" %>
<%@ Register Src="PMCtrlPrint.ascx" TagName="PMCtrlPrint" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidSTTDataFromStore = function(rec) {
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

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_pmno'), txt);
                }
              });
          }
        });
    }
  </script>

  <link href="../../styles/main.css" type="text/css" rel="Stylesheet" />

  <script type="text/javascript" src="../../scripts/simpleutils.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
  <ext:Hidden ID="hfMode" runat="server" />
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
              <ext:Button ID="btnPrintST" runat="server" Text="Cetak" Icon="Printer">
                <DirectEvents>
                  <Click OnEvent="btnPrintPL_OnClick">
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
              <Command Handler="if(command == 'Delete') { voidSTTDataFromStore(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_pmno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_pmno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridSTT" runat="server" RemotePaging="true" RemoteSort="true"
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
                  <ext:Parameter Name="model" Value="0320" />
                  <ext:Parameter Name="parameters" Value="[['c_pmno', paramValueGetter(#{txPMFltr}) + '%', ''],                    
                    ['d_pmdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_gdg = @0', paramValueGetter(#{cbGudangFltr}), 'System.Char'],
                    ['@contains.c_memo.Contains(@0) || @contains.c_memo.Contains(@0)', paramValueGetter(#{txMemoFltr}), ''],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]" Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_pmno">
                    <Fields>
                      <ext:RecordField Name="c_pmno" />
                      <ext:RecordField Name="d_pmdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="v_ket" />
                      <ext:RecordField Name="c_memo" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_pmno" Direction="DESC" />
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
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="120"/>                
                <ext:Column ColumnID="c_pmno" DataIndex="c_pmno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_pmdate" DataIndex="d_pmdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="c_memo" DataIndex="c_memo" Header="No Memo" Width="150" />                
                <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Keterangan" Width="200"/>
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txPMFltr}, #{txDateFltr}, #{cbGudangFltr}, #{txMemoFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />                      
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txPMFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                          <ext:TextField ID="txMemoFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
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
  <uc:PMCtrl ID="PMCtrl1" runat="server" />
  <uc:PMCtrlPrint ID="PMCtrlPrint1" runat="server" />
</asp:Content>
