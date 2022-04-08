<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CurrentStock.ascx.cs"
  Inherits="reporting_Inventory_CurrentStock" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>

<script type="text/javascript" language="javascript">
    var commandGridFunction = function(rec, comName, wndSP, gridSP, hidSP, wndGood, gridGood, hidGood, wndBad, gridBad, hidBad, wndPO, gridPO, hidPO, wndSiT, gridSiT, hidSiT, wndSoT, gridSoT, hidSoT, wndSPG, gridSPG, hidSPG, wndTot, gridTot, hidTot, wndPLBK, gridPLBK, hidPLBK) {
        var itm = rec.get('Item');
        var store = '';

        switch (comName) {




            case 'QtyOrder':
                if ((!Ext.isEmpty(wndSP)) && (!Ext.isEmpty(gridSP))) {
                    hidSP.setValue(itm);

                    wndSP.setTitle(String.format('Pending Surat Pesanan - {0}', rec.get('ItemName')));
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
            case 'SiT':
                if ((!Ext.isEmpty(wndSiT)) && (!Ext.isEmpty(gridSiT))) {
                    hidSiT.setValue(itm);

                    wndSiT.setTitle(String.format('Pending Stok in Transit - {0}', rec.get('ItemName')));
                    wndSiT.hide = false;
                    wndSiT.show();

                    store = gridSiT.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;




            case 'SoT':
                if ((!Ext.isEmpty(wndSoT)) && (!Ext.isEmpty(gridSoT))) {
                    hidSoT.setValue(itm);

                    wndSoT.setTitle(String.format('Pending Stok in Transit - {0}', rec.get('ItemName')));
                    wndSoT.hide = false;
                    wndSoT.show();

                    store = gridSoT.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;

            case 'PurcOrder':
                if ((!Ext.isEmpty(wndPO)) && (!Ext.isEmpty(gridPO))) {
                    hidPO.setValue(itm);

                    wndPO.setTitle(String.format('Pending Purchase Order - {0}', rec.get('ItemName')));
                    wndPO.hide = false;
                    wndPO.show();

                    store = gridPO.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;


            case 'SPG':
                if ((!Ext.isEmpty(wndSPG)) && (!Ext.isEmpty(gridSPG))) {
                    hidSPG.setValue(itm);

                    wndSPG.setTitle(String.format('Pending Purchase Order - {0}', rec.get('ItemName')));
                    wndSPG.hide = false;
                    wndSPG.show();

                    store = gridSPG.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;


            case 'Tot_Stock':
                if ((!Ext.isEmpty(wndTot)) && (!Ext.isEmpty(gridTot))) {
                    hidTot.setValue(itm);

                    wndTot.setTitle(String.format('Pending Purchase Order - {0}', rec.get('ItemName')));
                    wndTot.hide = false;
                    wndTot.show();

                    store = gridTot.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;



            case 'Boking':
                if ((!Ext.isEmpty(wndPLBK)) && (!Ext.isEmpty(gridPLBK))) {
                    hidPLBK.setValue(itm);

                    wndPLBK.setTitle(String.format('Pending Surat Pesanan - {0}', rec.get('ItemName')));
                    wndPLBK.hide = false;
                    wndPLBK.show();

                    store = gridPLBK.getStore();
                    store.removeAll();
                    store.reload();
                }
                break;




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
                      <ext:ListItem Value="1" Text="Type All" />
                      <%--<ext:ListItem Value="2" Text="Type Good" />
                      <ext:ListItem Value="3" Text="Type Bad" />--%>
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
                    <Command Handler="commandGridFunction(record, command, #{winDetailSP}, #{gpDetailSP}, #{hidItemSp}, #{winDetailGood}, #{gpDetailGood}, #{hidItemGood}, #{winDetailBad}, #{gpDetailBad}, #{hidItemBad}, #{winDetailPO}, #{gpDetailPO}, #{hidItemPO}, #{winDetailSiT}, #{gpDetailSiT}, #{hidItemSiT},  #{winDetailSoT}, #{gpDetailSoT}, #{hidItemSoT},#{winDetailSPG}, #{gpDetailSPG}, #{hidItemSPG}, #{winDetailTot}, #{gpDetailTot}, #{hidItemTot} ,#{winDetailPLBoking}, #{gpDetailPLBoking}, #{hidItemPLBoking}  );" />
                  </Listeners>
                  <SelectionModel>
                    <ext:RowSelectionModel SingleSelect="true" />
                  </SelectionModel>
                  <Store>
                    <ext:Store ID="store1" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <AutoLoadParams>
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="={50}" />
                      </AutoLoadParams>
                      <BaseParams>
                        <ext:Parameter Name="start" Value="0" />
                        <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                        <ext:Parameter Name="model" Value="10006" />
                        <ext:Parameter Name="parameters" Value="[['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char'],
                          ['gdgTrx', paramValueGetter(#{cbPosisiTrx}, '0') , 'System.Char'],
                          ['noSup', paramValueMultiGetter(#{cbSuplier}) , 'System.String[]'],
                          ['noSup', paramValueMultiGetter(#{txItemSupFilter}) , 'System.String[]'],
                          ['itemCode', paramValueGetter(#{txItemCodeFltr}), ''],
                          ['kddivams', paramValueGetter(#{cbDivAms}), ''],
                          ['kddivpri', paramValueGetter(#{cbDivPrinsipal}), ''],
                          ['itemSup', paramValueGetter(#{txItemSupFilter}), ''],
                          ['itemName', paramValueGetter(#{txItemNameFltr}), ''],
                          ['itemUndes', paramValueGetter(#{txItemUndes}), '']]" Mode="Raw" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                          IDProperty="Item">
                          <Fields>
                            <ext:RecordField Name="Item"/>
                            <ext:RecordField Name="ItemName" />
                            <ext:RecordField Name="KodeDatsup"/>
                            <ext:RecordField Name="KodeDivAms"/>
                            <ext:RecordField Name="KodeDivPrin"/>
                            
                            <ext:RecordField Name="KodeDivAms"/>
                            <ext:RecordField Name="NamaDivAms"/>
                            
                            <ext:RecordField Name="KodeDivPrin"/>
                            <ext:RecordField Name="NamaDivPrin"/>
                            
                            <ext:RecordField Name="NamaDatsup" />
                            <ext:RecordField Name="ItemUndes" />
                            
                            <ext:RecordField Name="PLBoking" Type="Float" />
                            
                            
                            <ext:RecordField Name="SOH_GOOD" Type="Float" />
                            <ext:RecordField Name="SOH_BAD" Type="Float" />
                            <ext:RecordField Name="ORD_QTY" Type="Float" />
                            <ext:RecordField Name="SIT_QTY" Type="Float" />
                            <ext:RecordField Name="SOT_QTY" Type="Float" />
                            <ext:RecordField Name="qREQ_QTY" Type="Float" />
                            <ext:RecordField Name="SPG_QTY" Type="Float" />
                            <ext:RecordField Name="Total_Stock_All" Type="Float" />
                            <ext:RecordField Name="ItemPrice" />
                            <ext:RecordField Name="IsAktif" Type="Boolean" />
                            <ext:RecordField Name="IsHide" Type="Boolean" />
                            
                            <ext:RecordField Name="total_good"  />
                            <ext:RecordField Name="total_bad"  />
                            
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                      <SortInfo Field="Item" Direction="ASC" />
                    </ext:Store>
                  </Store>
                  <ColumnModel>
                    <Columns>
                      
                     <ext:CommandColumn Width="25" Resizable="false">
                          <Commands>
                            <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" Hidden = "true" />
                            <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />--%>
                          </Commands>
                      </ext:CommandColumn>
                      <ext:Column ColumnID="KodeDatsup" DataIndex="KodeDatsup" Header="Kode Supplier" Width="85" />
                      <ext:Column ColumnID="NamaDatsup" DataIndex="NamaDatsup" Header="Nama Supplier" Width="100" />
                      
                      <ext:Column ColumnID="NamaDivAms" DataIndex="NamaDivAms" Header="Nama Divisi AMS" Width="100" />
                      <ext:Column ColumnID="NamaDivPrin" DataIndex="NamaDivPrin" Header="Nama Divisi Principal" Width="100" />
                      <ext:Column ColumnID="Item" DataIndex="Item" Header="Kode Brg" Width="85" />
                      <ext:Column ColumnID="ItemName" DataIndex="ItemName" Header="Nama Barang" Width="265" />
                      <ext:Column ColumnID="ItemUndes" DataIndex="ItemUndes" Header="Pack Size" Width="85" />
                       <ext:NumberColumn ColumnID="ItemPrice" DataIndex="ItemPrice" Header="HNA" Format="0.000,00/i"
                        Width="75" />
                      <ext:NumberColumn ColumnID="qREQ_QTY" DataIndex="qREQ_QTY" Header="Surat Pesanan Cabang " Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="Book" CommandName="QtyOrder" ToolTip-Text="Rincian Surat Permintaan"
                            ToolTip-Title="Command" />
                        </Commands>
                      </ext:NumberColumn>
                      <ext:NumberColumn ColumnID="SOH_GOOD" DataIndex="SOH_GOOD" Header="SoH Good" Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="Book" CommandName="QtyGood" ToolTip-Text="Rincian Stock Good"
                            ToolTip-Title="Command" />
                        </Commands>
                       </ext:NumberColumn>
                      <ext:NumberColumn ColumnID="SOH_BAD" DataIndex="SOH_BAD" Header="SoH Bad" Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="Book" CommandName="QtyBad" ToolTip-Text="Rincian Stock Bad"
                            ToolTip-Title="Command" />
                        </Commands>
                       </ext:NumberColumn>
                       
                       <%--PL Boking--%>
                       
                      <ext:NumberColumn ColumnID="PLBoking" DataIndex="PLBoking" Header="PL Booking" Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="Book" CommandName="Boking" ToolTip-Text="Rincian Boking"
                            ToolTip-Title="Command" />
                        </Commands>
                      </ext:NumberColumn>
                       
                       
                       
                       <ext:NumberColumn ColumnID="SOT_QTY" DataIndex="SOT_QTY" Header="GIT OUT" Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="BookGo" CommandName="SoT" ToolTip-Text="Rincian Stok Keluar dalam Transit"
                            ToolTip-Title="Command" />
                        </Commands>
                      </ext:NumberColumn>
                      
                      
                                            
                      
                       <ext:NumberColumn ColumnID="Total_Stock_All" DataIndex="Total_Stock_All" Header="Total Stock" Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="BookGo" CommandName="Tot_Stock" ToolTip-Text="Total Stock"
                            ToolTip-Title="Command" />
                        </Commands>
                       </ext:NumberColumn>
                      
                      
                      <ext:NumberColumn ColumnID="total_good" DataIndex="total_good" Header="Total Value IDR Good" Format="0.000,00/i"        Width="120" />
                        
                         <ext:NumberColumn ColumnID="total_bad" DataIndex="total_bad" Header="Total Value IDR Bad" Format="0.000,00/i"          Width="120" />
                       
                       <ext:NumberColumn ColumnID="SIT_QTY" DataIndex="SIT_QTY" Header="GIT IN" Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="BookGo" CommandName="SiT" ToolTip-Text="Rincian Stok dalam Transit"
                            ToolTip-Title="Command" />
                        </Commands>
                      </ext:NumberColumn>
                      
                    
                      
                      
                      <ext:NumberColumn ColumnID="ORD_QTY" DataIndex="ORD_QTY" Header="PO To Principle" Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="BookKey" CommandName="PurcOrder" ToolTip-Text="Rincian Purchase Order"
                            ToolTip-Title="Command" />
                        </Commands>
                      </ext:NumberColumn>
                      
                      <ext:NumberColumn ColumnID="SPG_QTY" DataIndex="SPG_QTY" Header="SP Dari DC" Format="0.000,00/i"
                        Width="100">
                        <Commands>
                          <ext:ImageCommand Icon="BookKey" CommandName="SPG" ToolTip-Text="Surat Pesan Gudang"
                            ToolTip-Title="Command" />
                        </Commands>
                      </ext:NumberColumn>
                      
                      <%--   <ext:NumberColumn ColumnID="BE_Stock" DataIndex="BE_Stock" Header="BE Stock" Format="0.000,00/i"
                        Width="75" />--%>
                        
                     
                        
                        
                        
                         
                         
                         
                      <ext:CheckColumn ColumnID="IsAktif" DataIndex="IsAktif" Header="Aktif" Width="50" />
                      <ext:CheckColumn ColumnID="IsHide" DataIndex="IsHide" Header="Sembunyi" Width="50" />
                    </Columns>
                  </ColumnModel>
                  <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>                           
                            <ext:Button ID="Button2" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel" >
                                <Listeners>
                                    <Click Handler="saveData(#{hfGridData},#{gridMain});" />
                                </Listeners>
                            </ext:Button>
                            
                          <%--Catak--%>
                              <ext:ToolbarSeparator />
                              <ext:button id="btnprint" runat="server" icon="printer" text="Cetak ke Excel">
                                  <directevents>
                                    <click onevent="report_ongenerate">
                                      <eventmask showmask="true" />
                                      <extraparams>
                                        <ext:parameter name="PosisiStok" value="#{cbPosisiStok}.getValue()" mode="raw"/>
                                        <ext:parameter name="TipeStock" value="#{SelectBoxTipeStock}.getValue()" mode="raw" />
                                        <ext:parameter name="Suplier" value="#{cbSuplier}.getValue()" mode="raw" />
                                        <ext:parameter name="itemCode" value="#{txItemCodeFltr}.getValue()" mode="raw" />
                                        <ext:parameter name="SuplierGrid" value="#{txItemSupFilter}.getValue()" mode="raw" />
                                        <ext:parameter name="kddivams" value="#{cbDivAms}.getValue()" mode="raw" />
                                        <ext:parameter name="kddivpri" value="#{cbDivPrinsipal}.getValue()" mode="raw" />
                                      </extraparams>
                                    </click>
                                  </directevents>
                                </ext:button>
                      
                        
                             </Items>
                    </ext:Toolbar>
                </TopBar>
                  <BottomBar>
                    <ext:PagingToolbar ID="gmPagingBB" runat="server" PageSize="50">
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
                          <SelectedItem Value="50" />
                          <Listeners>
                            <Select Handler="#{gmPagingBB}.pageSize = parseInt(this.getValue()); #{gmPagingBB}.doLoad();" />
                          </Listeners>
                        </ext:ComboBox>
                      </Items>
                    </ext:PagingToolbar>
                  </BottomBar>
                  <View>
                    <ext:GridView ID="gvFilter" runat="server" StandardHeaderRow="true">
                      <HeaderRows>
                      
                        <ext:HeaderRow>
                          <Columns>
                          
                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                        <Component>
                          <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                            <Listeners>
                              <Click Handler="clearFilterGridHeader(#{gridMain}, 
                                                                #{txItemSupFilter}, 
                                                                #{cbDivAms}, 
                                                                #{cbDivPrinsipal}, 
                                                                #{txItemCodeFltr}, 
                                                                #{txItemNameFltr}
                                                               
                                                                );reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                     
                      
                      </ext:HeaderColumn>
                      
                
                      
                      
                            <ext:HeaderColumn>
                              <Component>
                                <ext:TextField ID="txItemSupFilter" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                  <Listeners>
                                    <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                  </Listeners>
                                </ext:TextField>
                              </Component>
                            </ext:HeaderColumn>
                            
                            <ext:HeaderColumn />
                        
                            <%-- div ams--%>
                              <ext:HeaderColumn>
                        <Component>
                        <ext:ComboBox ID="cbDivAms" runat="server"  ValueField="c_kddivams"
                              DisplayField="v_nmdivams" Width="500" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
                              AllowBlank="true">
                              <CustomConfig>
                                <ext:ConfigItem Name="allowBlank" Value="true" />
                              </CustomConfig>
                              <Store>
                                <ext:Store ID="Store13" runat="server">
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="model" Value="2041" />
                                    <%--<ext:Parameter Name="parameters" Value="[['@contains.c_kddivams.Contains(@0) || @contains.v_nmdivams.Contains(@0)', paramTextGetter(#{cbDivAms}), '']]"
                                      Mode="Raw" />--%>
                                      
                                      <ext:Parameter Name="parameters" Value="[ ['l_aktif = @0', true, 'System.Boolean'],
                                                                                ['l_hide = @0', false, 'System.Boolean'], 
                                                                               ['@contains.v_nmdivams.Contains(@0) || @contains.c_kddivams.Contains(@0)', paramTextGetter(#{cbDivAms}), '']]"
                                                                       Mode="Raw" />
                                      
                                    <ext:Parameter Name="sort" Value="c_kddivams" />
                                    <ext:Parameter Name="dir" Value="ASC" />
                                  </BaseParams>
                                  <Reader>
                                    <ext:JsonReader IDProperty="c_kddivams" Root="d.records" SuccessProperty="d.success"
                                      TotalProperty="d.totalRows">
                                      <Fields>
                                        <ext:RecordField Name="c_kddivams" />
                                        <ext:RecordField Name="v_nmdivams" />
                                        <%--<ext:RecordField Name="v_divams_desc" />--%>
                                      </Fields>
                                    </ext:JsonReader>
                                  </Reader>
                                </ext:Store>
                              </Store>
                              <Template ID="Template3" runat="server">
                                <Html>
                                <table cellpading="0" cellspacing="0" style="width: 500px">
                                <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                                <tpl for=".">
                                  <tr class="search-item">
                                    <td>{c_kddivams}</td><td>{v_nmdivams}</td>
                                  </tr>
                                </tpl>
                                </table>
                                </Html>
                              </Template>                              
                             <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>                              
                            </ext:ComboBox> 
                        </Component>
                      </ext:HeaderColumn>
                            
                             <%-- div Prin--%>
                             <ext:HeaderColumn>
                                <Component>
                                    <ext:combobox ID="cbDivPrinsipal" runat="server"  ValueField="c_kddivpri"
                                      DisplayField="v_nmdivpri" Width="500" ListWidth="500" WrapBySquareBrackets="true"
                                      PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" Delimiter=";">
                                      <CustomConfig>
                                        <ext:ConfigItem Name="allowBlank" Value="true" />
                                      </CustomConfig>
                                      <Store>
                                        <ext:Store ID="Store14" runat="server">
                                          <Proxy>
                                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                              CallbackParam="soaScmsCallback" />
                                          </Proxy>
                                          <BaseParams>
                                            <ext:Parameter Name="start" Value="={0}" />
                                            <ext:Parameter Name="limit" Value="={10}" />
                                            <ext:Parameter Name="model" Value="2051" />
                                            <%--<ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />--%>
                                            <ext:Parameter Name="parameters" Value="[['@contains.v_nmdivpri.Contains(@0) || @contains.c_kddivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]"
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
                                        <table cellpading="0" cellspacing="0" style="width: 500px">
                                        <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                                        <tpl for=".">
                                          <tr class="search-item">
                                            <td>{c_kddivpri}</td><td>{v_nmdivpri}</td>
                                          </tr>
                                        </tpl>
                                        </table>
                                        </Html>
                                      </Template>
                                                    
                                      
                                      <Listeners>
                                      <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                                    </Listeners>
                                    
                                    </ext:combobox>      
                                </Component>
                            </ext:HeaderColumn>
                             
                            <ext:HeaderColumn>
                              <Component>
                                <ext:TextField ID="txItemCodeFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                  <Listeners>
                                    <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                  </Listeners>
                                </ext:TextField>
                              </Component>
                            </ext:HeaderColumn>
                            <ext:HeaderColumn>
                              <Component>
                                <ext:TextField ID="txItemNameFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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
                          <%--  <ext:HeaderColumn />--%>
                            
                            
                          </Columns>
                        </ext:HeaderRow>
                      </HeaderRows>
                    </ext:GridView>
                  </View>
                </ext:GridPanel>
              </Items>
            </ext:Panel>
          </Center>
        </ext:BorderLayout>
      </Items>
    </ext:Panel>
  </Items>
</ext:Viewport>
<%--Indra 20190128FM--%>
<%--<ext:Window ID="winDetailSP" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">--%>
<ext:Window ID="winDetailSP" runat="server" Width="718" Height="320" Hidden="true"
  MinWidth="718" MinHeight="320" Layout="FitLayout" Maximizable="true">  
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
              ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
              IDProperty="c_spno">
              <Fields>
                <ext:RecordField Name="c_spno" />
                <ext:RecordField Name="c_sp" />
                <ext:RecordField Name="d_spdate" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="ETD" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="ETA" Type="Date" DateFormat="M$" />
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
          <ext:Column DataIndex="c_spno" Header="Nomor SP" />
          <ext:Column DataIndex="c_sp" Header="Referensi" />
          <ext:DateColumn DataIndex="d_spdate" Header="Tanggal" Format="dd-MM-yyyy" />
          <ext:Column DataIndex="v_cunam" Header="Cabang" />
          <ext:NumberColumn DataIndex="n_pending" Header="Jumlah" Format="0.000,00/i" />
          <ext:DateColumn DataIndex="ETD" Header="ETD" Format="dd-MM-yyyy" />
          <ext:DateColumn DataIndex="ETA" Header="ETA" Format="dd-MM-yyyy" />
        </Columns>
      </ColumnModel>
      <TopBar>
        <ext:Toolbar ID="Toolbar2" runat="server">
            <Items>                            
                <ext:Button ID="Button1" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailSP});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
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


<%--PLBoking--%>




<ext:Window ID="winDetailPLBoking" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemPLBoking" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailPLBoking" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store15" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingPLBoking}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50038" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemPLBoking}), ''],
              ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
              <Fields>
               <ext:RecordField Name="Gudang" />
                <ext:RecordField Name="NamaCabang" />
                <ext:RecordField Name="NomorPL" />
                <ext:RecordField Name="DatePL" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="Item" />
                <ext:RecordField Name="NamaItem" />
                <ext:RecordField Name="Qty" Type="Float" />
                <ext:RecordField Name="Sisa" Type="Float" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="Item" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          
          <ext:Column DataIndex="Gudang" Header="Gudang" Width="80" />
          <ext:Column DataIndex="NamaCabang" Header="Nama Cabang" Width="80" />
          <ext:Column DataIndex="NomorPL" Header="Nomor PL" Width="80" />
          <ext:DateColumn DataIndex="DatePL" Header="Tanggal PL" Format="dd-MM-yyyy" />
          <ext:Column DataIndex="Item" Header="Kode Item" />
          <ext:Column DataIndex="NamaItem" Header="Nama Item" />
          <ext:NumberColumn DataIndex="Qty" Header="Jumlah Qty" Format="0.000,00/i" />
          <ext:NumberColumn DataIndex="Sisa" Header="Jumlah Sisa" Format="0.000,00/i" />

        </Columns>
      </ColumnModel>
      <TopBar>
        <ext:Toolbar ID="Toolbar10" runat="server">
            <Items>                            
                <ext:Button ID="Button10" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailPLBoking});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
      <BottomBar>
        <ext:PagingToolbar ID="gmPagingPLBoking" runat="server" PageSize="100">
          <Items>
            <ext:Label ID="Label10" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer10" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingPLBoking" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="100" />
              <Listeners>
                <Select Handler="#{gmPagingPLBoking}.pageSize = parseInt(this.getValue()); #{gmPagingPLBoking}.doLoad();" />
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
              ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
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


<ext:Window ID="winDetailSiT" runat="server" Width="700" Height="320" Hidden="true"
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
              ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
              IDProperty="RefNo">
              <Fields>
                <ext:RecordField Name="gdgAsal" />
                <ext:RecordField Name="gdgTujuan" />
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
          
          <ext:Column DataIndex="gdgAsal" Header="Ggd Asal" Width="80" />
          <ext:Column DataIndex="gdgTujuan" Header="Gdg Tujuan" Width="80" />
          
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

<%--SOT--%>
<ext:Window ID="winDetailSoT" runat="server" Width="700" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemSoT" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailSoT" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store10" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingSoTBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50035" />
            <ext:Parameter Name="parameters" Value="[['item_sot', paramValueGetter(#{hidItemSoT}), ''],
              ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
              IDProperty="RefNo_sot">
              <Fields>
                <ext:RecordField Name="gdgAsal_sot" />
                <ext:RecordField Name="gdgTujuan_sot" />
                <ext:RecordField Name="GdgDesc_sot" />
                <ext:RecordField Name="RefNo_sot" />
                <ext:RecordField Name="DateRef_sot" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="GQty_sot" Type="Float" />
                <ext:RecordField Name="BQty_sot" Type="Float" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="DateRef_sot" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <%--<ext:Column DataIndex="GdgDesc" Header="Gudang" Width="" />--%>
          
          <ext:Column DataIndex="gdgAsal_sot" Header="Ggd Asal" Width="80" />
          <ext:Column DataIndex="gdgTujuan_sot" Header="Gdg Tujuan" Width="80" />
          
          <ext:Column DataIndex="RefNo_sot" Header="Nomor" />
          <ext:DateColumn DataIndex="DateRef_sot" Header="Tanggal" Format="dd-MM-yyyy" />
          <ext:NumberColumn DataIndex="GQty_sot" Header="Jumlah (Baik)" Format="0.000,00/i" />
          <ext:NumberColumn DataIndex="BQty_sot" Header="Jumlah (Rusak)" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <TopBar>
        <ext:Toolbar ID="Toolbar7" runat="server">
            <Items>                            
                <ext:Button ID="Button7" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailSoT});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
      <BottomBar>
        <ext:PagingToolbar ID="gmPagingSoTBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label7" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer7" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingSoTBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingSoTBB}.pageSize = parseInt(this.getValue()); #{gmPagingSoTBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>
<%--SPG--%>

<ext:Window ID="winDetailSPG" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemSPG" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailSPG" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store11" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingSPGBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50036" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemSPG}), ''],
             ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.Char']]" Mode="Raw" />
             
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
        <ext:Toolbar ID="Toolbar8" runat="server">
            <Items>                            
                <ext:Button ID="Button8" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailSPG});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
      <BottomBar>
        <ext:PagingToolbar ID="gmPagingSPGBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label8" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer8" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingSPGBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingSPGBB}.pageSize = parseInt(this.getValue()); #{gmPagingSPGBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>

<%--Tot--%>

<ext:Window ID="winDetailTot" runat="server" Width="525" Height="320" Hidden="true"
  MinWidth="625" MinHeight="320" Layout="FitLayout" Maximizable="true">
  <Content>
    <ext:Hidden ID="hidItemTot" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gpDetailTot" runat="server" Layout="Fit">
      <LoadMask ShowMask="true" />
      <Store>
        <ext:Store ID="store12" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingTotBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="50037" />
            <ext:Parameter Name="parameters" Value="[['item', paramValueGetter(#{hidItemTot}), ''],
             ['gdgStok', paramValueGetter(#{cbPosisiStok}, '0') , 'System.char']
             
              
              ]" Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
              <Fields>
                <ext:RecordField Name="tipe_tot" />
                <ext:RecordField Name="gudang_asal_tot" />
                <ext:RecordField Name="gudang_tujuan_tot" />
                <ext:RecordField Name="c_iteno_Tot" />
                <ext:RecordField Name="v_itnam_Tot" />
                <ext:RecordField Name="c_batch_Tot" />
                <ext:RecordField Name="n_gsisa_Tot" />
                <ext:RecordField Name="n_bsisa_Tot" />
                <ext:RecordField Name="d_expired_Tot" Type="Date" DateFormat="M$" />                                                                 
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="c_batch_Tot" Direction="ASC" />
        </ext:Store>
      </Store>
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <ColumnModel>
        <Columns>
          <ext:Column DataIndex="tipe_tot" Header="Tipe Stock" />
          <ext:Column DataIndex="gudang_asal_tot" Header="Gudang Asal" />
          <ext:Column DataIndex="gudang_tujuan_tot" Header="Gudang Tujuan" />
          <ext:Column DataIndex="c_iteno_Tot" Header="Nomor Item" />
          <ext:Column DataIndex="v_itnam_Tot" Header="Nama Item" />
          <ext:Column DataIndex="c_batch_Tot" Header="Batch" />
          <ext:DateColumn ColumnID="d_expired_Tot" DataIndex="d_expired_Tot" Header="Expired" Format="dd-MM-yyyy" />                
          <ext:NumberColumn DataIndex="n_gsisa_Tot" Header="Jumlah Good" Format="0.000,00/i" />
          <ext:NumberColumn DataIndex="n_bsisa_Tot" Header="Jumlah Bad" Format="0.000,00/i" />
        </Columns>
      </ColumnModel>
      <TopBar>
        <ext:Toolbar ID="Toolbar9" runat="server">
            <Items>                            
                <ext:Button ID="Button9" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel">
                    <Listeners>
                        <Click Handler="saveData(#{hfGridData},#{gpDetailTot});" />
                    </Listeners>
                </ext:Button>
            </Items>
        </ext:Toolbar>
      </TopBar>
      <BottomBar>
        <ext:PagingToolbar ID="GmPagingTotBB" runat="server" PageSize="20">
          <Items>
            <ext:Label ID="Label9" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer9" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingTotBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingTotBB}.pageSize = parseInt(this.getValue()); #{gmPagingTotBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
</ext:Window>
