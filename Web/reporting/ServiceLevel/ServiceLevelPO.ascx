<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ServiceLevelPO.ascx.cs"
  Inherits="reporting_ServiceLevel_PO" %>

<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>





<script type="text/javascript">


    var validateRadio = function(SelectBoxTipeServiceLevelJava, gridMainjava, cbDivPrinsipaljava, v_nmdivamsjava, cbdivisiAMSjava) {



        if (SelectBoxTipeServiceLevelJava == "1") {

            cbDivPrinsipaljava.setVisible(false);
            cbDivPrinsipaljava.clearValue();

            cbdivisiAMSjava.setVisible(false);
            cbdivisiAMSjava.clearValue();




        }
        else {


            cbDivPrinsipaljava.setVisible(true);
            cbDivPrinsipaljava.clearValue();

            cbdivisiAMSjava.setVisible(true);
            cbdivisiAMSjava.clearValue();





        };




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
<ext:Viewport runat="server" Layout="FitLayout">
  <Items>
    <ext:Panel runat="server">
     
      <Items>
        
        <ext:BorderLayout ID="bllayout" runat="server">
          <North MinHeight="125" MaxHeight="125" Collapsible="false">
            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Border="false"
              Padding="5" Height="170">
              <Items>
              
               <%--Periode--%>  
                    <ext:CompositeField runat="server" FieldLabel="Periode ETA">
                      <Items>
                        <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                          AllowBlank="false">
                          <CustomConfig>
                            <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
                          </CustomConfig>
                          
                           <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridMain});" />
                          </Listeners>
                          
                        </ext:DateField>
                        <ext:Label runat="server" Text="-" />
                        <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                          AllowBlank="false">
                          <CustomConfig>
                            <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" />
                          </CustomConfig>
                          
                           <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridMain});" />
                          </Listeners>
                          
                        </ext:DateField>
                      </Items>
                     
                    </ext:CompositeField>
                         
              <%--Pemasok--%>  
                <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Supplier" ValueField="c_nosup"
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
                               
              <%--Divisi Pemasok--%>  
                            
                <ext:ComboBox ID="cbDivPrinsipal" runat="server" FieldLabel="Divisi Supplier" ValueField="c_kddivpri" Hidden ="true"
                  DisplayField="v_nmdivpri" Width="500" ListWidth="500" 
                  PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
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
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="={10}" />
                        <ext:Parameter Name="model" Value="2051" />
                        <ext:Parameter Name="parameters" Value="[['@in.c_nosup', paramValueMultiGetter(#{cbSuplier}), 'System.String[]'],
                          ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]" Mode="Raw" />
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
                  <Template ID="Template3" runat="server">
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
                  <Triggers>
                    <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                    <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                  </Triggers>
                  <Listeners>
                                <Change Handler="reloadFilterGrid(#{gridMain});" />
                  </Listeners>
                </ext:ComboBox>
                
              <%--Divisi AMS--%>   
               <ext:ComboBox ID="cbdivisiAMS" runat="server" FieldLabel="Divisi Ams" ValueField="c_kddivams" Hidden ="true"
                              DisplayField="v_nmdivams" Width="500" ListWidth="500" 
                              PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
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
                                    <ext:Parameter Name="model" Value="2041" />
                                    <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                                      ['@contains.c_kddivams.Contains(@0) || @contains.v_nmdivams.Contains(@0) || @contains.c_kddivams.Contains(@0)', paramTextGetter(#{cbDivAms}), '']]"
                                      Mode="Raw" />
                                    <ext:Parameter Name="sort" Value="v_nmdivams" />
                                    <ext:Parameter Name="dir" Value="ASC" />
                                  </BaseParams>
                                  <Reader>
                                    <ext:JsonReader IDProperty="c_kddivams" Root="d.records" SuccessProperty="d.success"
                                      TotalProperty="d.totalRows">
                                      <Fields>
                                        <ext:RecordField Name="c_kddivams" />
                                        <ext:RecordField Name="v_nmdivams" />
                                        <ext:RecordField Name="v_divams_desc" />
                                      </Fields>
                                    </ext:JsonReader>
                                  </Reader>
                                </ext:Store>
                              </Store>
                              <Template ID="Template2" runat="server">
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
                              <Triggers>
                                <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                              </Triggers>
                             <Listeners>
                                <Change Handler="reloadFilterGrid(#{gridMain});" />
                              </Listeners>
                            </ext:ComboBox>
                           
                <%--Type Stock--%>
                      
                 <ext:SelectBox ID="SelectBoxTipeServiceLevel" runat="server" FieldLabel="Jenis Laporan" SelectedIndex="0" AllowBlank="false"  >
                  <Items>  
                      <ext:ListItem Value="1" Text="Fulfilment Rate"  />
                      <ext:ListItem Value="2" Text="Line Fulfilment Rate"/>
                      
                      
                  </Items>
                
                   <Listeners>
                        <Change Handler="validateRadio(#{SelectBoxTipeServiceLevel}.getValue(), #{gridMain}, #{cbDivPrinsipal}, #{v_nmdivams}, #{cbdivisiAMS}); reloadFilterGrid(#{gridMain}); " />
                   </Listeners>
                               
                                     
                 </ext:SelectBox>
                 
                 
                 <%--Type Stock END--%>
                 
              </Items>
              
            </ext:FormPanel>
          </North>
          <Center>
            <ext:Panel ID="Panel1" runat="server" Layout="FitLayout">
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
                        <ext:Parameter Name="model" Value="20031" />
                        <ext:Parameter Name="parameters" Value="[
                          ['txPeriode1', paramRawValueGetter(#{txPeriode1}) , 'System.DateTime'],
                          ['txPeriode2', paramRawValueGetter(#{txPeriode2}) , 'System.DateTime'],
                          ['noSup2',  paramValueGetter(#{cbSuplier}), ''],
                          ['noSup2',  paramValueGetter(#{txItemSupFilter}), ''],
                          ['kddivams', paramValueGetter(#{cbdivisiAMS}), ''],
                          ['kddivpri', paramValueGetter(#{cbDivPrinsipal}), ''],
                          ['kddivams', paramValueMultiGetter(#{txItemSupFilter}) , 'System.String[]'],
                          ['itemPOCode', paramValueGetter(#{txNO_PO_Fltr}), ''],
                          ['itemCode', paramValueGetter(#{txItemCodeFltr}), ''],
                          ['itemSup', paramValueGetter(#{txItemSupFilter}), ''],
                          ['TypeService', paramValueGetter(#{SelectBoxTipeServiceLevel}), ''],
                          ['itemUndes', paramValueGetter(#{txItemUndes}), '']]" Mode="Raw" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                          IDProperty="Item">
                          <Fields>
                          
                            <ext:RecordField Name="c_nosup"/>
                            <ext:RecordField Name="v_nama" />
                            <ext:RecordField Name="c_kddivams"/>
                            <ext:RecordField Name="v_nmdivams"/>
                            <ext:RecordField Name="c_kddivpri"/>
                            <ext:RecordField Name="v_nmdivpri"/>
                            <ext:RecordField Name="c_gdg"/>
                            <ext:RecordField Name="c_pono"/>
                            <ext:RecordField Name="c_iteno" />
                            <ext:RecordField Name="v_itnam"/>
                            <ext:RecordField Name="Qty_PO"/>
                            <ext:RecordField Name="Tgl_Submit_PO" Type="Date" DateFormat="M$" />
                            <ext:RecordField Name="Tgl_Kirim_PO" Type="Date" DateFormat="M$"/>
                            <ext:RecordField Name="LeadTime"/>
                            <ext:RecordField Name="ETA" Type="Date" DateFormat="M$"/>
                            <ext:RecordField Name="Nomor_ST"/>
                            <ext:RecordField Name="Tgl_ST" Type="Date" DateFormat="M$" />
                            
                            <ext:RecordField Name="Tgl_DO_Prin" Type="Date" DateFormat="M$" />
                            <ext:RecordField Name="Kode_DO_Prin" />
                            <ext:RecordField Name="Nomor_RN"/>
                            <ext:RecordField Name="Tgl_RN" Type="Date" DateFormat="M$" />
                            <ext:RecordField Name="Qty_Received"/>
                            <ext:RecordField Name="Outstanding_PO"/>
                            <ext:RecordField Name="Total_Waktu_Pelayanan"/>
                            <ext:RecordField Name="Over_Lead_Time"/>
                            <ext:RecordField Name="Over_Lead_Time_Ket"/>
                            <ext:RecordField Name="On_Time_Order"/>
                           
                            <ext:RecordField Name="Date_Minus" Type="Date" DateFormat="M$" />
                            <ext:RecordField Name="Date_Plus" Type="Date" DateFormat="M$" />
                            <ext:RecordField Name="Value_PO_Hit_MIN"/>
                            <ext:RecordField Name="Value_PO_Hit_Plus"/>
                            <ext:RecordField Name="Score_By_QTY"/>  
                            <ext:RecordField Name="Score_By_Time"/>                         
                            
                            
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                      <SortInfo Field="c_pono" Direction="ASC" />
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
                      
                      <ext:Column ColumnID="c_nosup" DataIndex="c_nosup" Header="Kode Supplier" Width="85" />
                      <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama Supplier" Width="100" />
                      <ext:Column ColumnID="v_nmdivams" DataIndex="v_nmdivams" Header="Nama Divisi AMS" Width="100"  />
                      <ext:Column ColumnID="v_nmdivpri" DataIndex="v_nmdivpri" Header="Nama Divisi Principal" Width="100"  />
                      <ext:Column ColumnID="c_pono" DataIndex="c_pono" Header="Nomor PO" Width="85" />
                      <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode Barang" Width="85" />
                      <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Barang" Width="265" />
                      <ext:Column ColumnID="Qty_PO" DataIndex="Qty_PO" Header="QTY PO" Width="60" />
                      <ext:DateColumn ColumnID="Tgl_Submit_PO" DataIndex="Tgl_Submit_PO" Header="Tgl PO" Format="dd-MM-yyyy" />
                      <ext:DateColumn ColumnID="Tgl_Kirim_PO" DataIndex="Tgl_Kirim_PO" Header="Tgl Kirim PO" Format="dd-MM-yyyy" />
                      <ext:Column ColumnID="LeadTime" DataIndex="LeadTime" Header="Lead Time" Width="60" />
                      <ext:DateColumn ColumnID="ETA" DataIndex="ETA" Header="ETA" Format="dd-MM-yyyy" />
                      
                      <ext:Column ColumnID="Nomor_ST" DataIndex="Nomor_ST" Header="Nomor ST" Width="85" />
                      <ext:DateColumn ColumnID="Tgl_ST" DataIndex="Tgl_ST" Header="Tgl ST" Format="dd-MM-yyyy" />
                      <ext:DateColumn ColumnID="Tgl_DO_Prin" DataIndex="Tgl_DO_Prin" Header="Tgl DO Principal" Format="dd-MM-yyyy" />
                      <ext:Column ColumnID="Kode_DO_Prin" DataIndex="Kode_DO_Prin" Header="Kode Do Prin" Width="85" />
                      <ext:Column ColumnID="Nomor_RN" DataIndex="Nomor_RN" Header="Nomor RN" Width="100" />
                      <ext:DateColumn ColumnID="Tgl_RN" DataIndex="Tgl_RN" Header="Tgl RN" Format="dd-MM-yyyy" />
                      <ext:Column ColumnID="Qty_Received" DataIndex="Qty_Received" Header="QTY Received" Width="90" />
                      <ext:Column ColumnID="Outstanding_PO" DataIndex="Outstanding_PO" Header="Outstanding PO" Width="90" />
                      <ext:Column ColumnID="Total_Waktu_Pelayanan" DataIndex="Total_Waktu_Pelayanan" Header="Total Waktu Pelayanan" Width="120" />
                      <ext:Column ColumnID="Over_Lead_Time" DataIndex="Over_Lead_Time" Header="Over Lead Time" Width="95" />
                      <ext:Column ColumnID="Over_Lead_Time_Ket" DataIndex="Over_Lead_Time_Ket" Header="Keterangan" Width="80" />
                      <ext:Column ColumnID="On_Time_Order" DataIndex="On_Time_Order" Header="On Time Order" Width="80" />
                      <ext:Column ColumnID="In_Full_by_Qty" DataIndex="In_Full_by_Qty" Header="In Full by Qty" Width="80" Hidden ="true" />
                      <ext:Column ColumnID="In_Full_by_Item" DataIndex="In_Full_by_Item" Header="In Full by Item" Width="80" Hidden ="true"/>
                      <ext:Column ColumnID="Value_PO_Hit_MIN" DataIndex="Value_PO_Hit_MIN" Header="Value PO Hit MIN" Width="100" />
                      <ext:Column ColumnID="Value_PO_Hit_Plus" DataIndex="Value_PO_Hit_Plus" Header="Value PO Hit PLUS" Width="100" />
                      <ext:Column ColumnID="Score_By_QTY" DataIndex="Score_By_QTY" Header="Score By QTY" Width="80" />
                      <ext:DateColumn ColumnID="Date_Minus" DataIndex="Date_Minus" Header="Date Minus" Format="dd-MM-yyyy" />
                      <ext:DateColumn ColumnID="Date_Plus" DataIndex="Date_Plus" Header="Date Plus" Format="dd-MM-yyyy" />
                      <ext:Column ColumnID="Score_By_Time" DataIndex="Score_By_Time" Header="Score By Time" Width="80" />
              
                       
                    
                      
                    </Columns>
                  </ColumnModel>
                  <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>                           
                            <ext:Button ID="Button2" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel"  hidden ="true">
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
                                        <ext:parameter name="Date1" value="#{txPeriode1}.getValue()" mode="raw"/>
                                        <ext:parameter name="Date2" value="#{txPeriode2}.getValue()" mode="raw" />
                                        <ext:parameter name="Suplier" value="#{cbSuplier}.getValue()" mode="raw" />
                                        <ext:parameter name="kddivpri" value="#{cbDivPrinsipal}.getValue()" mode="raw" />
                                        <ext:parameter name="kddivams" value="#{cbdivisiAMS}.getValue()" mode="raw" />
                                        <ext:parameter name="TipeService" value="#{SelectBoxTipeServiceLevel}.getValue()" mode="raw" />
                                        <ext:parameter name="NoPO" value="#{txNO_PO_Fltr}.getValue()" mode="raw" />
                                                                                                                     
                                        
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
                                                                #{txNO_PO_Fltr}, 
                                                                #{txItemCodeFltr}
                                                               
                                                                );reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                     
                      
                        </ext:HeaderColumn>
                        <ext:HeaderColumn />
                        <ext:HeaderColumn />
                        <ext:HeaderColumn />
                        <ext:HeaderColumn />
                        <ext:HeaderColumn>
                              <Component>
                                <ext:TextField ID="txNO_PO_Fltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
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





<ext:Window ID="wndDown" runat="server" Hidden="true" />