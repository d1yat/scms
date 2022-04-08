<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MonitoringPO.aspx.cs" Inherits="transaksi_wp_MonitoringPO"
    MasterPageFile="~/Master.master" %>

<%@ Register Src="~/transaksi/wp/MonitoringPOCtrl.ascx" TagName="MonitoringPOCtrl"
    TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

    <script type="text/javascript">
        var voidRCDataFromStore = function(rec) {
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

                      Ext.net.DirectMethods.DeleteMethod(rec.get('c_rcno'), rec.get('c_gdg'), txt);
                  }
              });
            }
        });
        }

        var prepareCommandsParentRC = function(record, toolbar) {
            var accp = toolbar.items.get(0); // accept button

            var isSubmitRC = false;

            if (!Ext.isEmpty(record)) {
                isSubmitRC = record.get('l_sent');
            }

            if (isSubmitRC) {
                accp.setVisible(false);
            }
            else {
                accp.setVisible(true);
            }
        }

        var submitRCData = function(rec) {
            if (Ext.isEmpty(rec)) {
                return;
            }

            ShowConfirm('Kirim ?', 'Apakah anda yakin ingin memproses nomor ini ?',
        function(btn) {
            if (btn == 'yes') {
                Ext.net.DirectMethods.SubmitMethod(rec.get('c_rcno'));
            }
        });
        }

        var selectedSavedRCData = function(rcNumber) {
            Ext.net.DirectMethods.SelectedSavedMethod(rcNumber);
        }

        var voidEXPDataFromStore = function(rec) {
            if (Ext.isEmpty(rec)) {
                return;
            }

            ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?', function(btn) {
                if (btn == 'yes') {
                    ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.', function(btnP, txt) {
                        if (btnP == 'ok') {
                            if (txt.trim().length < 1) {
                                txt = 'Kesalahan pemakai.';
                            }

                            Ext.net.DirectMethods.DeleteMethod(rec.get('c_expno'), txt);
                        }
                    });
                }
            });
        }

        var getRowClass = function(record) {
            var cStatus = record.get('c_status');
//            if (cStatus == 'Waiting') {
//                return "red";
//            }

            if (cStatus == 'Receiving') {
                return "orange";
            }

            if (cStatus == 'RN Parsial') {
                return "magenta";
            }

            if (cStatus == 'RN Full') {
                return "green";
            }
        }
    </script>

    <style type="text/css">
        .red {
	        background: #FF0000;
        }
        
        .orange {
	        background: #FF9900;
        }
        .magenta {
	        background: #C8FFC8;
        } 
        
        .green {
	        background: #00FF00;
        }
    </style>    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <ext:Hidden ID="hfGdg" runat="server" />
    <ext:Hidden ID="hfMode" runat="server" />
    <ext:Hidden ID="hfType" runat="server" />
    <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
        <Items>
            <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
                <TopBar>
                    <ext:Toolbar runat="server">
                        <Items>
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
                                    <ext:Parameter Name="Parameter" Value="c_nodoc" />
                                    <ext:Parameter Name="PrimaryID" Value="record.data.c_nodoc" Mode="Raw" />
                                    <ext:Parameter Name="Status" Value="record.data.c_status" Mode="Raw" />
                                </ExtraParams>
                            </Command>
                        </DirectEvents>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
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
                                    <ext:Parameter Name="model" Value="0350" />
                                    <ext:Parameter Name="parameters" Value="[['c_nodoc', paramValueGetter(#{txTransFltr}) + '%', ''],
                                                            ['c_urut', paramValueGetter(#{txUrutFltr}) + '%', ''],
                                                            ['c_nosup = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String']]"
                                        Mode="Raw" />
                                </BaseParams>
                                <Reader>
                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                                        IDProperty="c_nodoc">
                                        <Fields>
                                            <ext:RecordField Name="c_nodoc" />
                                            <ext:RecordField Name="c_urut" />
                                            <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                                            <ext:RecordField Name="d_scan" Type="Date" DateFormat="M$" />
                                            <ext:RecordField Name="d_rn" Type="Date" DateFormat="M$" />
                                            <ext:RecordField Name="v_timeantri" />
                                            <ext:RecordField Name="v_timescan" />
                                            <ext:RecordField Name="v_timern" />
                                            <ext:RecordField Name="c_status" />
                                            <ext:RecordField Name="v_nama" />                                            
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="c_nodoc" Direction="DESC" />
                            </ext:Store>
                        </Store>
                        <ColumnModel>
                            <Columns>
                                <ext:CommandColumn Width="25" Resizable="false">
                                    <Commands>
                                        <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                                    </Commands>
                                </ext:CommandColumn>
                                <ext:Column ColumnID="c_nodoc" DataIndex="c_nodoc" Header="Nomor" />
                                <ext:Column ColumnID="c_urut" DataIndex="c_urut" Header="No.Antrian" />
                                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Pemasok" />                                                                
                                <ext:DateColumn ColumnID="d_entry" DataIndex="d_entry" Header="Waktu Antrian" Format="dd-MM-yyyy hh:mm:ss" Width="150" />
                                <ext:Column ColumnID="v_timeantri" DataIndex="v_timeantri" Header="Timer Antrian" />     
                                <ext:DateColumn ColumnID="d_scan" DataIndex="d_scan" Header="Waktu Receive" Format="dd-MM-yyyy hh:mm:ss" Width="150" />
                                <ext:Column ColumnID="v_timescan" DataIndex="v_timescan" Header="Timer Receive - RN" Width="150"/>     
                                <ext:DateColumn ColumnID="d_rn" DataIndex="d_rn" Header="Waktu RN" Format="dd-MM-yyyy hh:mm:ss" Width="150" />
                                <ext:Column ColumnID="v_timern" DataIndex="v_timern" Header="Timer Total" />                           
                                <ext:Column ColumnID="c_status" DataIndex="c_status" Header="Status" Width="150" />
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
                                                            <Click Handler="clearFilterGridHeader(#{GridMain}, #{txTransFltr}, #{txDateFltr}, #{txUrutFltr}, #{cbSuplierFltr}, #{txPlatFltr}, #{cbReceiveFltr});reloadFilterGrid(#{gridMain});"
                                                                Buffer="300" Delay="300" />
                                                        </Listeners>
                                                    </ext:Button>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:TextField ID="txTransFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                        <Listeners>
                                                            <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:TextField ID="txUrutFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                        <Listeners>
                                                            <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                                                        </Listeners>
                                                    </ext:TextField>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                    <ext:ComboBox ID="cbSuplierFltr" runat="server" DisplayField="v_nama" ValueField="c_nosup"
                                                        Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
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
                                                                    <ext:Parameter Name="model" Value="2021" />
                                                                    <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                                ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbSuplierFltr}), '']]"
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
                                                            <table cellpading="0" cellspacing="1" style="width: 400px">
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
                                            <ext:HeaderColumn />
                                            <ext:HeaderColumn />
                                            <ext:HeaderColumn />
                                            <ext:HeaderColumn />                                            
                                        </Columns>
                                    </ext:HeaderRow>
                                </HeaderRows>
                                <GetRowClass Fn="getRowClass" /> 
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
    <uc:MonitoringPOCtrl ID="MonitoringPOCtrl" runat="server" />
</asp:Content>
