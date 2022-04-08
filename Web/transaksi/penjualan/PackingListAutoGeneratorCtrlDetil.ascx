<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PackingListAutoGeneratorCtrlDetil.ascx.cs" Inherits="transaksi_penjualan_PackingListAutoGeneratorCtrlDetil" %>

<%@ Register Src="PackingListCtrl.ascx" TagName="PackingListCtrl" TagPrefix="uc" %>
<script type="text/javascript"> 
        var setGroupStyle = function (view) {
            var groups = view.getGroups();

            for (var i = 0; i < groups.length; i++) {
                var spans =  Ext.query("span", groups[i]);
                
                if (spans && spans.length > 0){
                    var color = "#" + spans[0].id.split("-")[1];

                    Ext.get(groups[i]).setStyle("background-color", color);
                }
            }
          };

          var showData = function(view, f, dm) {
            var sd = null;
          }

    </script>   

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfGudangDet" runat="server" />
    <ext:Hidden ID="hfGudangDescDet" runat="server" />
    <ext:Hidden ID="hfPlNoAutoGenDet" runat="server" />
    <ext:Hidden ID="hfNip" runat="server" />
    <ext:Hidden ID="hfStoreAutoGenIDDet" runat="server" />
    <ext:Store ID="strGirdDet" runat="server" Visible ="false" />
    
  </Content>
  <Items>
  <ext:BorderLayout ID="bllayout" runat="server">
     <Center MinHeight="250">
      <ext:Panel ID="frmHeaders" runat="server" Title="Header" Height="150"
        Layout="Fit" ButtonAlign="Center" MonitorValid="true">
        <Items>
          <ext:GridPanel ID="gridDetail" runat="server" AutoExpandColumn="c_iteno">
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <View>
                <ext:GroupingView  
                    ID="GroupingView1"
                    HideGroupedColumn="true"
                    runat="server" 
                    ForceFit="true"
                    StartCollapsed="true"
                    GroupTextTpl='<span id="ColorCode-{[values.rs[0].data.ColorCode]}"></span>{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
                    EnableRowBody="true">
                    <Listeners>
                        <Refresh Fn="setGroupStyle" />
                    </Listeners>
                </ext:GroupingView>
                
            </View>
            <Store>
              <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false" GroupField="v_ketDet">
                <Proxy>
                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                    CallbackParam="soaScmsCallback" />
                </Proxy>
                <BaseParams>
                  <ext:Parameter Name="start" Value="0" />
                  <ext:Parameter Name="limit" Value="-1" />
                  <ext:Parameter Name="allQuery" Value="true" />
                  <%--<ext:Parameter Name="model" Value="0203" />--%>
                  <ext:Parameter Name="sort" Value="" />
                  <ext:Parameter Name="dir" Value="" />
                  <%--<ext:Parameter Name="parameters" Value="[['nip', #{hfNip}.getValue(), 'System.String'],
                                                           ['gdg', #{hfGudangDet}.getValue(), 'System.Char']]"
                    Mode="Raw" />--%>
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                    <Fields>
                    
                      <ext:RecordField Name="v_ketDet" />
                      <ext:RecordField Name="c_plno" />
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="v_itemdesc" />
                      <ext:RecordField Name="c_sp" />
                      <ext:RecordField Name="c_spc" />
                      <ext:RecordField Name="v_undes" />
                      <ext:RecordField Name="c_batch" />
                      <ext:RecordField Name="v_ket_Cat" />
                      <ext:RecordField Name="v_ket_Lat" />
                      <ext:RecordField Name="n_booked" Type="Float" />
                      <ext:RecordField Name="n_QtyRequest" Type="Float" />
                      <ext:RecordField Name="n_lastqtyRequest" Type="Float" />
                      <ext:RecordField Name="n_sisa" Type="Float" />
                      <ext:RecordField Name="l_new" Type="Boolean" />
                      <ext:RecordField Name="l_void" Type="Boolean" />
                      <ext:RecordField Name="l_modified" Type="Boolean" />
                      <ext:RecordField Name="v_ket" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:Column DataIndex="v_ketDet" Header="No PL" Width="150" Hidden="true" />
                <ext:Column DataIndex="c_plno" Header="No PL" Width="150" Hidden="true" />
                <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                <ext:Column DataIndex="v_undes" Header="Kemasan" />
                <ext:Column DataIndex="c_spc" Header="SP Cabang" />
                <%--<ext:Column DataIndex="v_typedesc" Header="Type" Width="50" />--%>
                <ext:Column DataIndex="c_batch" Header="Batch" />
                <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                <ext:NumberColumn DataIndex="n_QtyRequest" Header="Terpenuhi" Format="0.000,00/i"
                  Width="75" >
                  <Editor>
                    <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                  </Editor>
                </ext:NumberColumn>
                <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                <ext:CommandColumn Hidden="true">
                  <GroupCommands>
                    <ext:GridCommand CommandName="ItemCommand" Text="Click" Icon="Pencil" >  
                    </ext:GridCommand>
                  </GroupCommands>
                </ext:CommandColumn>
              </Columns>
            </ColumnModel>
            <%--<Listeners>
              <GroupCommand Handler="if(command == 'ItemCommand') { showData(this, groupId, #{DirectMethods} ); }" />
            </Listeners>--%>
            <DirectEvents>
              <GroupCommand OnEvent="showDetil">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="sId" Value="groupId" Mode="Raw"></ext:Parameter>
                </ExtraParams>
              </GroupCommand>
            </DirectEvents>
          </ext:GridPanel>
        </Items>
        <Buttons>
          <%--<ext:Button runat="server" Text="Print" Icon="Printer">
            <DirectEvents>
              <Click OnEvent="PrintBtn_Click">
                <Confirmation 
                  ConfirmRequest="true" Title="Print ?" Message="Anda yakin ingin Print data ini." />
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="gridValuesAll" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>
          <ext:Button ID="server1" runat="server" Text="Auto Print" Icon="Printer">
            <DirectEvents>
              <Click OnEvent="PrintBtnAuto_Click">
                <Confirmation 
                  ConfirmRequest="true" Title="Print ?" Message="Anda yakin ingin Print data ini." />
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="gridValuesAll" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>--%>
          <ext:Button ID="Button1" runat="server" Icon="Cancel" Text="Keluar">
              <Listeners>
                <Click Handler="#{winDetail}.hide();" />
              </Listeners>
          </ext:Button>
        </Buttons>
     </ext:Panel>
   </Center>
  </ext:BorderLayout>
  </Items>
</ext:Window>
<uc:PackingListCtrl ID="PackingListCtrl1" runat="server" />
<ext:Window ID="wndDown" runat="server" Hidden="true" />