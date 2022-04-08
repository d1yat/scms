<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MonitoringExpired.ascx.cs"
  Inherits="reporting_Inventory_MonitoringED" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>

<script type="text/javascript" language="javascript">
    var commandGridFunction = function(rec, comName,
    wndSP,
    gridSP,
    hidSP,
    wndGood,
    gridGood,
    hidGood,
    wndBad,
    gridBad,
    hidBad,
    //wndPO,
    //gridPO,
    //hidPO
    wndSiT,
    gridSiT,
    hidSiT
    ) {
        var itm = rec.get('Item');
        var store = '';

        switch (comName) {
            case 'QtySupName':
                if ((!Ext.isEmpty(wndSP)) && (!Ext.isEmpty(gridSP))) {
                    hidSP.setValue(itm);

                    wndSP.setTitle(String.format('Pending Surat Pesanan - {0}', rec.get('SupName')));
                    wndSP.hide = false;
                    wndSP.show();

                    store = gridSP.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;
            case 'QtyGood':
                if ((!Ext.isEmpty(wndGood)) && (!Ext.isEmpty(gridGood))) {
                    hidGood.setValue(itm);

                    wndGood.setTitle(String.format('Detail Stock Good - {0}', rec.get('ItemName')));
                    wndGood.hide = false;
                    wndGood.show();

                    store = gridGood.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;
            case 'QtyBad':
                if ((!Ext.isEmpty(wndBad)) && (!Ext.isEmpty(gridBad))) {
                    hidBad.setValue(itm);

                    wndBad.setTitle(String.format('Detail Stock Bad - {0}', rec.get('ItemName')));
                    wndBad.hide = false;
                    wndBad.show();

                    store = gridBad.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;
            case 'QtyBatch':
                if ((!Ext.isEmpty(wndSiT)) && (!Ext.isEmpty(gridSiT))) {
                    hidSiT.setValue(itm);

                    wndSiT.setTitle(String.format('Pending Stok in Transit - {0}', rec.get('Batch')));
                    wndSiT.hide = false;
                    wndSiT.show();

                    store = gridSiT.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;
//            case 'PurcOrder':
//                if ((!Ext.isEmpty(wndPO)) && (!Ext.isEmpty(gridPO))) {
//                    hidPO.setValue(itm);

//                    wndPO.setTitle(String.format('Pending Purchase Order - {0}', rec.get('ItemName')));
//                    wndPO.hide = false;
//                    wndPO.show();

//                    store = gridPO.getStore();
//                    store.removeAll();
//                    store.reload();
//                }
//                break;
        }
    }
    var saveData = function(grid, gridPan) {
        grid.setValue(Ext.encode(gridPan.getRowsValues({ selectedOnly: false })));
    };
</script>

<script runat="server">
    protected void ToExcel(object sender, EventArgs e)
    {
        string json = hfGridData.Value.ToString();
        StoreSubmitDataEventArgs eSubmit = new StoreSubmitDataEventArgs(json, null);
        XmlNode xml = eSubmit.Xml;

        this.Response.Clear();
        this.Response.ContentType = "application/vnd.ms-excel";
        this.Response.AddHeader("Content-Disposition", "attachment; filename=submittedData.xls");
        XslCompiledTransform xtExcel = new XslCompiledTransform();
        xtExcel.Load(Server.MapPath("Excel.xsl"));
        xtExcel.Transform(xml, null, this.Response.OutputStream);
        this.Response.End();
    }
</script>

<Content>
    <ext:Hidden ID="hfGridData" runat="server" />
    <ext:Hidden ID="hidWndDown" runat="server" />    
</Content>
<ext:Viewport ID="Viewport1" runat="server" Layout="FitLayout">
  <Items>
    <ext:Panel ID="Panel1" runat="server">
     
      <Items>
        
        <ext:BorderLayout ID="bllayout" runat="server">
          <North MinHeight="125" MaxHeight="125" Collapsible="false">
            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Border="false"
              Padding="5" Height="125">
              <Items>
              <%--Posisi Stock--%>
                <ext:ComboBox ID="cbPosisiStok" runat="server" FieldLabel="Posisi Stock" ValueField="c_gdg"
                  DisplayField="v_gdgdesc" Width="175" AllowBlank="true" ForceSelection="false" EmptyText="Pilihan...">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store5" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2033" />
                        <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
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
                    <Change Handler="reloadFilterGrid(#{gridMain});" />
                  </Listeners>
                </ext:ComboBox>
                <%-- Tanggal  --%>
                <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Tanggal">
                  <Items>
                    <ext:DateField ID="txTanggal" runat="server" AllowBlank="false" Format="dd-MM-yyyy"
                      EnableKeyEvents="true" Disabled="true" />
                  </Items>
                </ext:CompositeField>
              <%--Posisi Transaksi--%>  
                <ext:ComboBox ID="cbPosisiTrx" runat="server" FieldLabel="Posisi Transaksi" ValueField="c_gdg"
                  DisplayField="v_gdgdesc" Width="175" AllowBlank="true" ForceSelection="false" EmptyText="Pilihan..." Hidden="true">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store6" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2033" />
                        <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
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
                    <Change Handler="reloadFilterGrid(#{gridMain});" />
                  </Listeners>
                </ext:ComboBox>
              <%--Pemasok--%>  
                <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
                  DisplayField="v_nama" Width="350" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
                  AllowBlank="true" ForceSelection="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store7" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="={10}" />
                        <ext:Parameter Name="model" Value="2021" />
                        <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                            ['l_aktif == @0', true, 'System.Boolean'],
                            ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]" Mode="Raw" />
                            
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
                  <Template ID="Template1" runat="server">
                    <Html>
                    <table cellpading="0" cellspacing="0" style="width: 500px">
                  <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                  <tpl for=".">
                    <tr class="search-item">
                      <td>{c_nosup}</td><td>{v_nama}</td>
                    </tr>
                  </tpl>
                  </table>
                    </Html>
                  </Template>
                  <Listeners>
                    <Change Handler="reloadFilterGrid(#{gridMain});" />
                  </Listeners>
                </ext:ComboBox>
                <%--Type Stock--%>
                      
                 <ext:SelectBox ID="SelectBoxTipeStock" runat="server" FieldLabel="Type Stock" SelectedIndex="0" AllowBlank="false" Hidden="true">
                  <Items>  
                      <ext:ListItem Value="1" Text="Type Good & Bad Header" />
                      <ext:ListItem Value="2" Text="Type Good Detail" />
                      <ext:ListItem Value="3" Text="Type Bad Detail" />
                      
                  </Items>
                  </ext:SelectBox>
                  
                                            
                  
                  
                 <%--Type Stock END--%>
                 
              </Items>
              
            </ext:FormPanel>
          </North>
          <Center>
            <ext:Panel ID="Panel2" runat="server" Layout="FitLayout">
            
            
              <Items>
                <ext:GridPanel ID="gridMain" runat="server" Layout="Fit">
                  <LoadMask ShowMask="true" />
                  <Listeners>
                    <Command Handler="commandGridFunction(record, command, #{winDetailSP}, #{gpDetailSP}, #{hidItemSp}, #{winDetailGood}, #{gpDetailGood}, #{hidItemGood}, #{winDetailBad}, #{gpDetailBad}, #{hidItemBad}, #{winDetailPO}, #{gpDetailPO}, #{hidItemPO}, #{winDetailSiT}, #{gpDetailSiT}, #{hidItemSiT});" />
                  </Listeners>
                  <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                  </SelectionModel>
                  
                   <%--awal cut--%>
                  <Store>
              <ext:Store ID="storeGridItem" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="10011-a" />
                                    
                 <ext:Parameter Name="parameters" Value="[
                             
                             ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char'],
                             ['gdgTrx', paramValueGetter(#{cbPosisiTrx}, '0') , 'System.Char'],
                             ['c_nosup', paramValueMultiGetter(#{cbSuplier}) , 'System.String[]'],
                             ['pilihgood', paramValueGetter(#{SelectBoxTipeStock}, '0') , 'System.Char'],
                         
                              
                              ['c_iteno', paramValueGetter(#{txItemIDFltr}), ''],
                              ['v_itnam', paramValueGetter(#{txItemNameFltr}), '']
                              
                              
                              
                              ]"
                    Mode="Raw" />
                   
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_iteno">
                    <Fields>
                      <ext:RecordField Name="c_nosup" />
                      <ext:RecordField Name="v_nama" />
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="v_itnam" />
                      <ext:RecordField Name="Good_1_3" />
                      <ext:RecordField Name="Goodvalue_1" />
                      <ext:RecordField Name="Good_4_6" />
                      <ext:RecordField Name="Goodvalue__4" />
                      <ext:RecordField Name="Good_7_9" />
                      <ext:RecordField Name="Goodvalue_7" />
                      <ext:RecordField Name="Good_10_12" />
                      <ext:RecordField Name="Goodvalue_10" />
                      <ext:RecordField Name="Bad_1_3" />
                      <ext:RecordField Name="Badvalue_1" />
                      <ext:RecordField Name="Bad_4_6" />
                      <ext:RecordField Name="Badvalue__4" />
                      <ext:RecordField Name="Bad_7_9" />
                      <ext:RecordField Name="Badvalue_7" />
                      <ext:RecordField Name="Bad_10_12" />
                      <ext:RecordField Name="Badvalue_10" />
                      <ext:RecordField Name="EDGOOD" />
                      <ext:RecordField Name="EDBAD" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="v_itnam" Direction="ASC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_nosup" DataIndex="c_nosup" Header=" Kode Supplier" Width="50" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama Supplier" Width="150" />
                <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode" Width="50" />
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama" Width="150" />
                <ext:NumberColumn ColumnID="Good_1_3" DataIndex="Good_1_3" Header="NEG(1 < X ≤ 3 mos)" Width="100" >
                    <Commands>
                        
                    </Commands>
                </ext:NumberColumn>
                <ext:NumberColumn ColumnID="Goodvalue_1" DataIndex="Goodvalue_1" Header="Value NEG (Rp.)" Width="100" />
                <ext:NumberColumn ColumnID="Bad_1_3" DataIndex="Bad_1_3" Header="NEB(1 < X ≤ 3 mos)" Width="100" />
                <ext:NumberColumn ColumnID="Badvalue_1" DataIndex="Badvalue_1" Header="Value NEB (Rp.)" Width="100"  />
                <ext:NumberColumn ColumnID="Good_4_6" DataIndex="Good_4_6" Header="NEG(3 < X ≤ 6 mos)" Width="100" />
                <ext:NumberColumn ColumnID="Goodvalue__4" DataIndex="Goodvalue__4" Header="Value NEG (Rp.)" Width="100" />
                <ext:NumberColumn ColumnID="Bad_4_6" DataIndex="Bad_4_6" Header="NEB(3 < X ≤ 6 mos)" Width="100"/>
                <ext:NumberColumn ColumnID="Badvalue__4" DataIndex="Badvalue__4" Header="Value NEB (Rp.)" Width="100" />
                <ext:NumberColumn ColumnID="Good_7_9" DataIndex="Good_7_9" Header="NEG(6 < X ≤ 9 mos)" Width="100" />
                <ext:NumberColumn ColumnID="Goodvalue_7" DataIndex="Goodvalue_7" Header="Value NEG (Rp.)" Width="100" />
                <ext:NumberColumn ColumnID="Bad_7_9" DataIndex="Bad_7_9" Header="NEB(6 < X ≤ 9 mos)" Width="100" />
                <ext:NumberColumn ColumnID="Badvalue_7" DataIndex="Badvalue_7" Header="Value NEB (Rp.)" Width="100" />
                <ext:NumberColumn ColumnID="Good_10_12" DataIndex="Good_10_12" Header="NEG(10 ≥ mos)" Width="100" />
                <ext:NumberColumn ColumnID="Goodvalue_10" DataIndex="Goodvalue_10" Header="Value NEG (Rp.)" Width="100"  />
                <ext:NumberColumn ColumnID="Bad_10_12" DataIndex="Bad_10_12" Header="NEB(10 ≥ mos)" Width="100" />
                <ext:NumberColumn ColumnID="Badvalue_10" DataIndex="Badvalue_10" Header="Value NEB (Rp.)" Width="100" />
                <ext:NumberColumn ColumnID="EDGOOD" DataIndex="EDGOOD" Header="ED GOOD" Width="100" />
                <ext:NumberColumn ColumnID="EDBAD" DataIndex="EDBAD" Header="ED BAD" Width="100" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, 
                                                                #{txItemIDFltr}, 
                                                                #{cbSuplier}, 
                                                                #{cbPrincipalFltr},
                                                                #{txItemNameFltr}, 
                                                                #{SelectBoxTipeStock}
                                                                
                                                                
                                                                );reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      
                      <%--Kode--%>
                      <ext:HeaderColumn>
                       <Component>
                          <ext:TextField ID="txItemIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      
                      <%--Nama--%>
                      <ext:HeaderColumn>
                         <Component>
                          <ext:TextField ID="txItemNameFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                     </ext:HeaderColumn>
                      
                      <ext:HeaderColumn />
                      
                          <%--Principle--%>  
                          
                                 
                      <%-- <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="cbPrincipalFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>--%>
                          
                                          
                     <%-- <ext:HeaderColumn>
                                                                
                         <Component>
                          <ext:ComboBox ID="cbPrincipalFltr" runat="server" DisplayField="v_nama" ValueField="v_nama"
                            Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="350" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store1" runat="server">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="={0}" />
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2021" />
                                  <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean'],
                                              ['l_hide = @0', false, 'System.Boolean'],
                                              ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalFltr}), '']]"
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
                            <Template ID="Template6" runat="server">
                              <Html>
                              <table cellpading="0" cellspacing="0" style="width: 350px">
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
                      </ext:HeaderColumn>--%>
                      
                       <%--kosong--%>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                      
                       <ext:HeaderColumn />
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
              </ext:GridView>
            </View>
            
            <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                        <Items>                           
                            <ext:Button ID="Button1" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                                <Listeners>
                                    <Click Handler="saveData(#{hfGridData},#{gridMain});" />
                                </Listeners>
                            </ext:Button>
                            
                          <%--Catak--%>
                              <ext:ToolbarSeparator />
                              <ext:button id="Button7" runat="server" icon="printer" text="cetak per batch">
                                  <directevents>
                                    <click onevent="report_ongenerate">
                                      <eventmask showmask="true" />
                                      <extraparams>
                                        <ext:parameter name="PosisiStok" value="#{cbPosisiStok}.getValue()" mode="raw"/>
                                        <ext:parameter name="TipeStock" value="#{SelectBoxTipeStock}.getValue()" mode="raw" />
                                        <ext:parameter name="Suplier" value="#{cbSuplier}.getValue()" mode="raw" />
                                      </extraparams>
                                    </click>
                                  </directevents>
                                </ext:button>
                      
                        
                             </Items>
                    </ext:Toolbar>
                </TopBar>
            
            <BottomBar>
              <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="100">
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
                  
                  <%--akhir cut--%>
                </ext:GridPanel>
              </Items>
              
            
            </ext:Panel>
          </Center>
        </ext:BorderLayout>
      </Items>
      
      
    </ext:Panel>
  </Items>
</ext:Viewport>
<ext:Window ID="winDetailSP" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemSp" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailSP" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store2" runat="server" RemoteSort="true">
          <Proxy>
            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
              CallbackParam="soaScmsCallback" />
          </Proxy>
          <AutoLoadParams>
            <ext:Parameter Name="start" Value="={0}" />
            <ext:Parameter Name="limit" Value="={100}" />
          </AutoLoadParams>
          <BaseParams>
            <ext:Parameter Name="start" Value="0" />
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingSPBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50001" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemSp}), ''],
              ['gdgTrx', paramValueGetter(#{cbPosisiTrx}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
              IDProperty="c_spno">
              <Fields>
                <ext:RecordField Name="c_spno" />
                <ext:RecordField Name="c_sp" />
                <ext:RecordField Name="d_spdate" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="v_cunam" />
                <ext:RecordField Name="n_pending" Type="Float" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="d_spdate" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="c_spno" Header="Nomor" />
          <ext:Column DataIndex="c_sp" Header="Referensi" />
          <ext:DateColumn DataIndex="d_spdate" Header="Tanggal" Format="dd-MM-yyyy" />
          <ext:Column DataIndex="v_cunam" Header="Cabang" />
          <ext:NumberColumn DataIndex="n_pending" Header="Jumlah" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
     <%-- <TopBar>
        <ext:Toolbar ID="Toolbar2" runat="server">
            <Items>                            
                <ext:Button ID="Button1" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailSP});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>--%>
      
      
     
      <BottomBar>
        <ext:PagingToolbar ID="gmPagingSPBB" runat="server" PageSize="100">
          <Items>
            <ext:Label ID="Label2" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingSPBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="100" />
              <Listeners>
                <Select Handler="#{gmPagingSPBB}.pageSize = parseInt(this.getValue()); #{gmPagingSPBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>
<ext:Window ID="winDetailGood" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="625" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemGood" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailGood" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store8" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingGoodBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50031" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemGood}), ''],
              ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
              <Fields>
                <ext:RecordField Name="c_gdg" />
                <ext:RecordField Name="c_iteno" />
                <ext:RecordField Name="v_itnam" />
                <ext:RecordField Name="c_batch" />
                <ext:RecordField Name="n_gsisa" />
                <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />                                                                 
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="c_batch" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="c_gdg" Header="Gudang" />
          <ext:Column DataIndex="c_iteno" Header="Nomor Item" />
          <ext:Column DataIndex="v_itnam" Header="Nama Item" />
          <ext:Column DataIndex="c_batch" Header="Batch" />
          <ext:DateColumn ColumnID="d_expired" DataIndex="d_expired" Header="Expired" Format="dd-MM-yyyy" />                
          <ext:NumberColumn DataIndex="n_gsisa" Header="Jumlah" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <TopBar>
        <ext:Toolbar ID="Toolbar3" runat="server">
            <Items>                            
                <ext:Button ID="Button3" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailGood});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
      <BottomBar>
        <ext:PagingToolbar ID="GmPagingGoodBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label5" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer5" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingGoodBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingGoodBB}.pageSize = parseInt(this.getValue()); #{gmPagingGoodBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>
<ext:Window ID="winDetailBad" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="625" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemBad" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailBad" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store9" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBadBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50032" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemBad}), ''],
              ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success" >
              <Fields>
                <ext:RecordField Name="c_gdg" />
                <ext:RecordField Name="c_iteno" />
                <ext:RecordField Name="v_itnam" />
                <ext:RecordField Name="c_batch" />
                <ext:RecordField Name="n_bsisa" />
                <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />                                                                                 
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="c_batch" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="c_gdg" Header="Gudang" />
          <ext:Column DataIndex="c_iteno" Header="Nomor Item" />
          <ext:Column DataIndex="v_itnam" Header="Nama Item" />
          <ext:Column DataIndex="c_batch" Header="Batch" />
          <ext:DateColumn ColumnID="d_expired" DataIndex="d_expired" Header="Expired" Format="dd-MM-yyyy" />                          
          <ext:NumberColumn DataIndex="n_bsisa" Header="Jumlah" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <TopBar>
        <ext:Toolbar ID="Toolbar4" runat="server">
            <Items>                            
                <ext:Button ID="Button4" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailBad});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
      <BottomBar>
        <ext:PagingToolbar ID="GmPagingBadBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label6" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer6" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingBadBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingBadBB}.pageSize = parseInt(this.getValue()); #{gmPagingBadBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>
<ext:Window ID="winDetailPO" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemPO" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailPO" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store3" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingPOBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50021" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemPO}), ''],
              ['gdgTrx', paramValueGetter(#{cbPosisiTrx}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
              IDProperty="c_pono">
              <Fields>
                <ext:RecordField Name="c_pono" />
                <ext:RecordField Name="d_podate" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="n_sisa" Type="Float" />
                <ext:RecordField Name="l_import" Type="Boolean" />
                <ext:RecordField Name="v_ket"  />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="d_podate" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="c_pono" Header="Nomor" />
          <ext:DateColumn DataIndex="d_podate" Header="Tanggal" Format="dd-MM-yyyy" />
          <ext:NumberColumn DataIndex="n_sisa" Header="Jumlah" Format="0.000,00/i" />
          <ext:CheckColumn DataIndex="l_import" Header="Import" Width="50" />
          <ext:Column DataIndex="v_ket" Header="Keterangan" />
        </Columns>
      </ColumnModel>
      <TopBar>
        <ext:Toolbar ID="Toolbar5" runat="server">
            <Items>                            
                <ext:Button ID="Button5" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailPO});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
      <BottomBar>
        <ext:PagingToolbar ID="gmPagingPOBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label3" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer3" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingPOBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingPOBB}.pageSize = parseInt(this.getValue()); #{gmPagingPOBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>
<ext:Window ID="winDetailSiT" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemSiT" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailSiT" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store4" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingSiTBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50011" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemSiT}), ''],
              ['gdgTrx', paramValueGetter(#{cbPosisiTrx}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
              IDProperty="RefNo">
              <Fields>
                <ext:RecordField Name="GdgDesc" />
                <ext:RecordField Name="RefNo" />
                <ext:RecordField Name="DateRef" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="GQty" Type="Float" />
                <ext:RecordField Name="BQty" Type="Float" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="DateRef" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <%--<ext:Column DataIndex="GdgDesc" Header="Gudang" Width="" />--%>
          <ext:Column DataIndex="RefNo" Header="Nomor" />
          <ext:DateColumn DataIndex="DateRef" Header="Tanggal" Format="dd-MM-yyyy" />
          <ext:NumberColumn DataIndex="GQty" Header="Jumlah (Baik)" Format="0.000,00/i" />
          <ext:NumberColumn DataIndex="BQty" Header="Jumlah (Rusak)" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <TopBar>
        <ext:Toolbar ID="Toolbar6" runat="server">
            <Items>                            
                <ext:Button ID="Button6" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailSiT});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
      <BottomBar>
        <ext:PagingToolbar ID="gmPagingSiTBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label4" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer4" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingSiTBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingSiTBB}.pageSize = parseInt(this.getValue()); #{gmPagingSiTBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>