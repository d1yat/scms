<%--
Created By Indra
20171231FM
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StockOpnamePerProduk.ascx.cs" Inherits="proses_stock_opname_StockOpnamePerProduk" %>

<script type="text/javascript">

    var PilihItemSO = function(e, storeout, storein) {

        storeout.remove(e);
        storein.insert(0, e);
    }

    var BatalItemSO = function(e, storeout, storein) {

        storeout.remove(e);
        storein.insert(0, e);
    }
    

</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="825" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="825" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfRnNo" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="150">
        <ext:Panel ID="Panel2" runat="server" Layout="Fit">
          <Items>
            <ext:BorderLayout ID="BorderLayout1" runat="server">
              <West Collapsible="true">
                <ext:Panel ID="Panel4" runat="server" Layout="Fit" Title="Daftar Items">
                  <TopBar>
                      <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>
                          <ext:ComboBox ID="cbSuplier" runat="server" DisplayField="v_nama" ValueField="c_nosup"
                            Width="170" ItemSelector="tr.search-item" PageSize="10" ListWidth="380" MinChars="3"
                            EmptyText="Principal" AllowBlank="false" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="false" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store3" runat="server">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="={0}" />
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2021" />
                                  <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                    ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]"
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
                            <Template ID="Template3" runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="1" style="width: 400px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                              <tpl for="."><tr class="search-item">
                                  <td>{c_nosup}</td><td>{v_nama}</td>
                              </tr></tpl>
                              </table>
                              </Html>
                            </Template>
                            <Triggers>
                              <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                              <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                            </Triggers>
                            <Listeners>                
                              <Select Handler="this.triggers[0].show();" />
                              <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                              <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                              <Change Handler="clearRelatedComboRecursive(true, #{cbDivSuplier});" />
                            </Listeners>
                          </ext:ComboBox>
                          <ext:Label ID="LbSpasi2" runat="server" Text="&nbsp;" />              
                          <ext:ComboBox ID="cbDivSuplier" runat="server" DisplayField="v_nmdivpri" ValueField="c_kddivpri"
                            Width="170" ItemSelector="tr.search-item" PageSize="10" ListWidth="380" MinChars="3"
                            EmptyText = "Div.Princ" AllowBlank="false" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="false" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store4" runat="server">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="={0}" />
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2051" />
                                  <ext:Parameter Name="parameters" Value="[['@in.c_nosup', paramValueMultiGetter(#{cbSuplier}), 'System.String[]'],
                                    ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivSuplier}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_nmdivpri" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader IDProperty="c_kddivpri" Root="d.records" SuccessProperty="d.success"
                                    TotalProperty="d.totalRows">
                                    <Fields>
                                      <ext:RecordField Name="c_kddivpri" />
                                      <ext:RecordField Name="v_nmdivpri" />
                                    </Fields>
                                  </ext:JsonReader>
                                </Reader>
                              </ext:Store>
                            </Store>
                            <Template ID="Template4" runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="1" style="width: 400px">
                              <tr><td class="body-panel">Kode</td><td class="body-panel">Div. Pemasok</td></tr>
                              <tpl for="."><tr class="search-item">
                                  <td>{c_kddivpri}</td><td>{v_nmdivpri}</td>
                              </tr></tpl>
                              </table>
                              </Html>
                            </Template>
                            <Triggers>
                              <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                              <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                            </Triggers>
                            <Listeners>
                              <Select Handler="this.triggers[0].show();" />
                              <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                              <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                              <Change Handler="clearRelatedComboRecursive(true, #{cbItems});" />
                            </Listeners>
                          </ext:ComboBox>
                          <ext:Label ID="LbSpasi3" runat="server" Text="&nbsp;" />               
                          <ext:SelectBox ID="cbKategori" runat="server" Width="100" ListWidth="80" SelectedIndex="-1" EmptyText="Kategori">
                            <Items>
                                <ext:ListItem Text="Regular" Value="01" />
                                <ext:ListItem Text="Psikotropika" Value="02" />
                                <ext:ListItem Text="Dingin" Value="03" />
                            </Items>
                          </ext:SelectBox>
                          <ext:Label ID="LbSpasi4" runat="server" Text="&nbsp;" />     
                          <ext:SelectBox ID="cbStatus" runat="server" Width="80" ListWidth="80" SelectedIndex="-1" EmptyText="Status">
                            <Items>
                                <ext:ListItem Text="Good" Value="Stock Good" />
                                <ext:ListItem Text="Bad" Value="Stock Bad" />
                            </Items>
                          </ext:SelectBox>
                          <ext:Button ID="btnGetBtn" runat="server" Text="1.Get Data" Icon="BulletRight" Disabled="false" ToolTip="Muat ulang data">
                            <DirectEvents>
                              <Click OnEvent="GetBtn_Click">
                                <EventMask ShowMask="true" />
                                <ExtraParams>
                                  <ext:Parameter Name="cbSuplier" Value="#{cbSuplier}.getValue()" Mode="Raw" />
                                  <ext:Parameter Name="cbDivSuplier" Value="#{cbDivSuplier}.getValue()" Mode="Raw" />
                                  <ext:Parameter Name="cbKategori" Value="#{cbKategori}.getValue()" Mode="Raw" />
                                  <ext:Parameter Name="cbStatus" Value="#{cbStatus}.getValue()" Mode="Raw" />
                                </ExtraParams>
                              </Click>
                            </DirectEvents>
                          </ext:Button>
                        </Items>
                      </ext:Toolbar>
                    </TopBar>
                  <Items>
                    <ext:GridPanel ID="gridDetail" runat="server" ClicksToEdit="1">
                      <SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                      </SelectionModel>
                      <Store>
                        <ext:Store ID="storeGridSP" runat="server" RemoteSort="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="20" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="0257" />
                          </BaseParams>
                          <Reader>
                          <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                            IDProperty="principal">
                            <Fields>
                              <ext:RecordField Name="kdprincipal" />
                              <ext:RecordField Name="principal" />                      
                              <ext:RecordField Name="kddivprincipal" />
                              <ext:RecordField Name="divprincipal" />
                              <ext:RecordField Name="location" />
                              <ext:RecordField Name="kdbarang" />
                              <ext:RecordField Name="nmbarang" />                      
                              <ext:RecordField Name="stbarang" />                   
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="principal" Direction="ASC" />
                        </ext:Store>
                      </Store>
                    <ColumnModel DefaultSortable="true">
                      <Columns>
                      <ext:Column DataIndex="nomor" Header="No." Width="55" />
                      <ext:Column DataIndex="kdprincipal" Header="Kd. Principal" Width="150" Hidden="true"/>
                      <ext:Column DataIndex="principal" Header="Principal" Width="150" />
                      <ext:Column DataIndex="kddivprincipal" Header="Kd. Div Principal" Width="150" Hidden="true"/>
                      <ext:Column DataIndex="divprincipal" Header="Div Principal" Width="150" />
                      <ext:Column DataIndex="location" Header="Location" Width="75" Hidden="true"/>
                      <ext:Column DataIndex="kdbarang" Header="Kd. Brg" Width="75" />
                      <ext:Column DataIndex="nmbarang" Header="Nm. Brg" Width="175" />
                      <ext:Column DataIndex="stbarang" Header="Status" Width="75" Hidden="true" />                                                       
                      </Columns>                
                    </ColumnModel> 
                    <Listeners>
                      <DblClick Handler="PilihItemSO(#{gridDetail}.getSelectionModel().getSelected(), #{gridDetail}.getStore(), #{gridDetailNewItem}.getStore());" />    
                    </Listeners>
                    </ext:GridPanel>
                  </Items>
                </ext:Panel>
              </West>
              <Center>
                <ext:Panel ID="Panel3"  runat="server" Layout="Fit" Title="Daftar Item Akan di Freeze">
                  <Items>
                    <ext:GridPanel ID="gridDetailNewItem" runat="server" ClicksToEdit="1">
                      <SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                      </SelectionModel>
                      <Store>
                        <ext:Store ID="store1" runat="server" RemoteSort="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="20" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="0257" />
                          </BaseParams>
                          <Reader>
                          <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                            IDProperty="principal">
                            <Fields>
                              <ext:RecordField Name="kdprincipal" />
                              <ext:RecordField Name="principal" />                      
                              <ext:RecordField Name="kddivprincipal" />
                              <ext:RecordField Name="divprincipal" />
                              <ext:RecordField Name="location" />
                              <ext:RecordField Name="kdbarang" />
                              <ext:RecordField Name="nmbarang" />                      
                              <ext:RecordField Name="stbarang" />                   
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="principal" Direction="ASC" />
                        </ext:Store>
                      </Store>
                      <ColumnModel DefaultSortable="true">
                      <Columns>
                      <ext:Column DataIndex="nomor" Header="No." Width="55" />
                      <ext:Column DataIndex="kdprincipal" Header="Kd. Principal" Width="150" Hidden="true"/>
                      <ext:Column DataIndex="principal" Header="Principal" Width="150" />
                      <ext:Column DataIndex="kddivprincipal" Header="Kd. Div Principal" Width="150" Hidden="true"/>
                      <ext:Column DataIndex="divprincipal" Header="Div Principal" Width="150" />
                      <ext:Column DataIndex="location" Header="Location" Width="75" Hidden="true"/>
                      <ext:Column DataIndex="kdbarang" Header="Kd. Brg" Width="75" />
                      <ext:Column DataIndex="nmbarang" Header="Nm. Brg" Width="175" />
                      <ext:Column DataIndex="stbarang" Header="Status" Width="75" />                                                       
                      </Columns>                
                    </ColumnModel>
                      <Listeners>
                        <DblClick Handler="PilihItemSO(#{gridDetailNewItem}.getSelectionModel().getSelected(), #{gridDetailNewItem}.getStore(), #{gridDetail}.getStore());" />    
                      </Listeners>
                    </ext:GridPanel>
                  </Items>
                </ext:Panel>
              </Center>
            </ext:BorderLayout>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="Button2" runat="server" Icon="Accept" Text="Pilih Item">
      <DirectEvents>
        <Click OnEvent="btnPilihPerProduk_OnClick">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetailNewItem});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin membuat Form SO." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetailNewItem}.getStore())" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    
  </Buttons>
 
</ext:Window>