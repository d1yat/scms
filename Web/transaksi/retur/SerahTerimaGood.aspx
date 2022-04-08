<%@ Page Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="SerahTerimaGood.aspx.cs" Inherits="transaksi_retur_SerahTerimaGood"%>

<%@ Register Src="SerahTerimaGoodCtrl.ascx" TagName="SerahTerimaCtrl" TagPrefix="uc" %>
<%@ Register Src="SerahTerimaGoodPrint.ascx" TagName="SerahTerimaPrint" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" Runat="Server">
<ext:Viewport runat="server" Layout="Fit">
    <Content>
      <ext:Hidden ID="hfMode" runat="server" />
      <ext:Hidden ID="hfGudang" runat="server" />
      <ext:Hidden ID="hfType" runat="server" />
    </Content>
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
                <ext:Button ID="btnrefresh" runat="server" Text="Refresh" Icon="ArrowRefresh" >
                    <Listeners>
                        <Click Handler="refreshGrid(#{gridMain});" />
                    </Listeners>
                </ext:Button>
                <ext:Button ID="btnPrint" runat="server" Text="Cetak Report" Icon="Report">
                    <DirectEvents>
                        <Click OnEvent="btnPrint_OnClick">
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
                <DirectEvents>
                    <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                        <EventMask ShowMask="true" />
                        <ExtraParams>
                          <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                          <ext:Parameter Name="Parameter" Value="v_stno" />
                          <ext:Parameter Name="PrimaryID" Value="record.data.v_stno" Mode="Raw" />
                        </ExtraParams>
                    </Command>
                </DirectEvents>
                <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                </SelectionModel>
                <Store>
                    <ext:Store ID="gridMS" runat="server" SkinID="OriginalExtStore" RemoteSort="true">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <AutoLoadParams>
                          <ext:Parameter Name="start" Value="={0}" />
                        </AutoLoadParams>
                        <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="0361" />
                            <ext:Parameter Name="parameters" Value="[]" Mode="Raw" />
                        </BaseParams>
                        <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success" IDProperty="v_stno">
                                <Fields>
                                    <ext:RecordField Name="v_stno" />
                                    <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="v_nama" />
                                    <ext:RecordField Name="c_rcno" />
                                    <ext:RecordField Name="v_pbbrno" />
                                    <ext:RecordField Name="v_cunam" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="v_stno" Direction="DESC" />
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
                        <ext:Column ColumnID="stno" DataIndex="v_stno" Header="Nomor ST" />
                        <ext:DateColumn ColumnID="dtproses" DataIndex="d_entry" Header="Tanggal Buat" Format="dd-MM-yyyy" />
                        <ext:Column ColumnID="rcno" DataIndex="c_rcno" Header="Nomor RC" />
                        <ext:Column ColumnID="pbbrno" DataIndex="v_pbbrno" Header="Nomor PBB" />
                        <ext:Column ColumnID="cunam" DataIndex="v_cunam" Header="Cabang" />
                        <ext:Column ColumnID="nama" DataIndex="v_nama" Header="Pembuat" />
                    </Columns>
                </ColumnModel>
                <View>
                    <ext:GridView ID="gvST" runat="server" StandardHeaderRow="true">
                        <HeaderRows>
                            <ext:HeaderRow>
                                <Columns>
                                    <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                        <Component>
                                            <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                                <Listeners>
                                                  <Click Handler="clearFilterGridHeader(#{gridMain}, #{txRCFltr}, #{txDateFltr}, #{cbGudangFltr}, #{cbCustomerFltr}, #{txPBBR}, #{txNipFltr});reloadFilterGrid(#{gridMain});"
                                                    Buffer="300" Delay="300" />
                                                </Listeners>
                                            </ext:Button>
                                        </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn />
                                    <ext:HeaderColumn />
                                    <ext:HeaderColumn />
                                    <ext:HeaderColumn />
                                    <ext:HeaderColumn />
                                    <ext:HeaderColumn />
                                </Columns>
                            </ext:HeaderRow>
                        </HeaderRows>
                    </ext:GridView>
                </View>
            </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  <uc:SerahTerimaCtrl ID="SerahTerimaCtrl2" runat="server" />
  <uc:SerahTerimaPrint ID="SerahTerimaPrint1" runat="server" />
</asp:Content>

