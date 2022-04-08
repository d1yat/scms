<%--
Created By Indra
20171231FM
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StockOpnameMonitoring.ascx.cs" Inherits="proses_stock_opname_StockOpnameMonitoring" %>

<script type="text/javascript">

    var afterEditDataConfirmChange = function(e, store) {

        e.record.set('noform', e.originalValue);
        e.record.set('noadjust', e.originalValue);
    }
            
</script>


<ext:Window ID="winDetail3" runat="server" Height="500" Width="1200" Hidden="true"
  Maximizable="false" MinHeight="500" MinWidth="1200" Layout="Fit" Resizable="false">
 <Content>
  <ext:Hidden ID="hfTypeName" runat="server" />
 </Content>
 <Items>
  <ext:Panel ID="pnlMainControl3" runat="server" Layout="Fit">
    <Items>
      <ext:GridPanel ID="gridMainMonitor" runat="server" Title = "Monitoring Stock Opname">
        <LoadMask ShowMask="true" />
        <SelectionModel>
          <ext:RowSelectionModel SingleSelect="true" />
        </SelectionModel>
        <Store>
          <ext:Store ID="storeGridSOMonitor" runat="server" RemoteSort="false">
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
                <ext:Parameter Name="model" Value="0259" />
                <ext:Parameter Name="parameters" Value="[['@contains.c_nosup.Contains(@0)', paramValueGetter(#{txprincipal}) , 'System.String']]" Mode="Raw" />
              </BaseParams>
              <Reader>
              <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                IDProperty="c_reff">
                <Fields>
                  <ext:RecordField Name="noform" />
                  <ext:RecordField Name="c_nosup" />
                  <ext:RecordField Name="v_nama" />                      
                  <ext:RecordField Name="c_kddivpri" />
                  <ext:RecordField Name="v_nmdivpri" />
                  <ext:RecordField Name="hitawal" />
                  <ext:RecordField Name="rec1" />
                  <ext:RecordField Name="rec2" />
                  <ext:RecordField Name="adjust" />
                </Fields>
              </ext:JsonReader>
            </Reader>
            <SortInfo Field="v_nama" Direction="DESC" />
            </ext:Store>
        </Store>
        <ColumnModel>
          <Columns>
            <ext:Column DataIndex="no" Header="No." Width="55" />           
            <ext:Column DataIndex="noform" Header="No. Form" Width="110">
              <Editor>
                <ext:TextField DataIndex="noform" runat="server" Width="110">
                </ext:TextField>
              </Editor>
            </ext:Column>            
            <ext:Column DataIndex="kdprincipal" Header="Kd.Principal" Hidden ="false"/>
            <ext:Column DataIndex="principal" Header="Principal" Width="150" />
            <ext:Column DataIndex="kddivprincipal" Header="Kd.Div Principal" Hidden ="false" />
            <ext:Column DataIndex="divprincipal" Header="Div Principal" Width="150" />
            <ext:CheckColumn DataIndex = "hitungawal" Header="Hitung Awal" />
            <ext:CheckColumn DataIndex = "recount1" Header="Recount-1" />
            <ext:CheckColumn DataIndex = "recount2" Header="Recount-2" />
            <ext:CheckColumn DataIndex = "adjustment" Header="Adjusment" />
            <ext:Column DataIndex="noadjust" Header="No. Adjustment" Width="100" >
              <Editor>
                <ext:TextField DataIndex="noadjust" runat="server" Width="100">
                </ext:TextField>
              </Editor>
            </ext:Column>
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
                          <Click Handler="clearFilterGridHeader(#{gridMainMonitor}, #{txprincipal});reloadFilterGrid(#{gridMainMonitor});"
                            Buffer="300" Delay="300" />
                        </Listeners>
                      </ext:Button>
                    </Component>
                  </ext:HeaderColumn>                  
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      <ext:TextField ID="txprincipal" runat="server" EnableKeyEvents="true" AllowBlank="true">
                        <Listeners>
                          <KeyUp Handler="reloadFilterGrid(#{gridMainMonitor})" Buffer="300" Delay="300" />
                        </Listeners>                        
                      </ext:TextField>
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                  <ext:HeaderColumn>
                    <Component>
                      
                    </Component>
                  </ext:HeaderColumn>
                </Columns>
              </ext:HeaderRow>
            </HeaderRows>
          </ext:GridView>
        </View>
        <BottomBar>
          <ext:Toolbar ID="Toolbar1" runat = "server">
            <Items>
              <ext:Button ID="btnMonitoringSO" runat="server" Text="Print Form Monitoring SO" Icon="Printer" ToolTip="Print form monitoring SO">
              <DirectEvents>
                <Click OnEvent="btnMonitoringSO_OnClick">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                    ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin membuat form monitoring SO." />
                  <EventMask ShowMask="true" />
                </Click>
              </DirectEvents>
            </ext:Button>        
            </Items>
          </ext:Toolbar>
        </BottomBar>
         <Listeners>
           <AfterEdit Handler="afterEditDataConfirmChange(e, #{gridMainMonitor}.getStore());" />              
         </Listeners>
      </ext:GridPanel>
    </Items>
  </ext:Panel>
 </Items>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
