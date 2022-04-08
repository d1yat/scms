<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ServiceLevelCabang.ascx.cs"
  Inherits="reporting_ServiceLevel_Cabang" %>
 
<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>


<script type="text/javascript">


    var validateRadio = function(SelectBoxTipeServiceLevelJava, gridMainMenitjava, gridMainMenit_Detailjava, TabPanelLogisticMenitJava, TabPanelLogMenitDetailjava, TabpanelPPIC, gridPPIC, tabpnlDC, GridDC) {
        console.log(SelectBoxTipeServiceLevelJava, TabpanelPPIC, TabPanelLogisticMenitJava, TabPanelLogMenitDetailjava);
        if (SelectBoxTipeServiceLevelJava == "1") {
            TabpanelPPIC.setVisible(true);
            tabpnlDC.setVisible(false);
            TabPanelLogisticMenitJava.setVisible(false);
            TabPanelLogMenitDetailjava.setVisible(false);
        }
        else if (SelectBoxTipeServiceLevelJava == "2") {
            TabpanelPPIC.setVisible(false);
            tabpnlDC.setVisible(true);
            TabPanelLogisticMenitJava.setVisible(false);
            TabPanelLogMenitDetailjava.setVisible(false);
        }
        else if (SelectBoxTipeServiceLevelJava == "3") {
            TabpanelPPIC.setVisible(false);
            tabpnlDC.setVisible(false);
            TabPanelLogisticMenitJava.setVisible(true);
            TabPanelLogMenitDetailjava.setVisible(false);
        }
        else if (SelectBoxTipeServiceLevelJava == "4") {
            TabpanelPPIC.setVisible(false);
            tabpnlDC.setVisible(false);
            TabPanelLogisticMenitJava.setVisible(false);
            TabPanelLogMenitDetailjava.setVisible(true);
        };


    };



//    var validateRadioDC = function(SelectBoxTipeServiceLevelJava, gridMainDCjava, pnlGridDCjava) {

//        if (SelectBoxTipeServiceLevelJava == "1") {

//            pnlGridDCjava.setVisible(false);
//            pnlGridDCjava.clearValue();

//        }
//        else {

//            pnlGridDCjava.setVisible(true);
//            pnlGridDCjava.clearValue();

//        };

//    };


    
    

        
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
    <ext:Hidden ID="hfUserID" runat="server" />    
</Content>
<ext:Viewport ID="Viewport1" runat="server" Layout="FitLayout">
  <Items>
   <ext:Panel ID="Panel1" runat="server" >
     <Items>
       <ext:BorderLayout ID="bllayout" runat="server" >
          <North MinHeight="125" MaxHeight="125" Collapsible="false">
            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Border="True" Padding="5" Height="160">
              <Items>
               <%--Periode--%>  
               <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Periode SP">
                 <Items>
                    <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy" AllowBlank="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
                      </CustomConfig>
                      <Listeners>
                       <Change Handler="reloadFilterGrid(#{gridMainMenit}) , reloadFilterGrid(#{gridMainMenit_Detail}) ;" />
                      </Listeners>
                    </ext:DateField>
                    <ext:Label ID="Label1" runat="server" Text="-" />
                    <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                      AllowBlank="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" /> 
                      </CustomConfig>
                       <Listeners>
                       <Change Handler="reloadFilterGrid(#{gridMainMenit}) , reloadFilterGrid(#{gridMainMenit_Detail}), reloadFilterGrid(#{gridPPIC}),reloadFilterGrid(#{GridDC}) ;" />
                      </Listeners>
                    </ext:DateField>
                 </Items>
               </ext:CompositeField>
               <%--Gudang--%>
               <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" ValueField="c_gdg" DisplayField="v_gdgdesc" Width="175" AllowBlank="true" ForceSelection="false" EmptyText="Pilihan...">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store3" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000" CallbackParam="soaScmsCallback" />
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
                  <Change Handler="reloadFilterGrid(#{gridMainMenit}) , reloadFilterGrid(#{gridMainMenit_Detail}), reloadFilterGrid(#{gridPPIC}), reloadFilterGrid(#{GridDC}) ;" />
                  </Listeners>
                </ext:ComboBox>
               <%--Cabang--%>            
               <ext:ComboBox ID="cbCustomerHdr" runat="server"  FieldLabel="Cabang" DisplayField="v_cunam" 
                                         ValueField="c_cusno" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" 
                                         MinChars="3" AllowBlank="true"  ForceSelection="false">
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
                                                <ext:Parameter Name="model" Value="2011" />
                                                <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                                  ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]" Mode="Raw" />
                                                <ext:Parameter Name="sort" Value="v_cunam" />
                                                <ext:Parameter Name="dir" Value="ASC" />
                                              </BaseParams>
                                              <Reader>
                                                <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                  TotalProperty="d.totalRows">
                                                  <Fields>
                                                    <ext:RecordField Name="c_cusno" />
                                                    <ext:RecordField Name="v_cunam" />
                                                    <ext:RecordField Name="c_cab" />
                                                  </Fields>
                                                </ext:JsonReader>
                                              </Reader>
                                            </ext:Store>
                                            
                                          </Store>
                                          <Template ID="Template1" runat="server">
                                            <Html>
                                            <table cellpading="0" cellspacing="1" style="width: 400px">
                                                <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                                                <tpl for="."><tr class="search-item">
                                                    <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
                                                </tr></tpl>
                                                </table>
                                            </Html>
                                          </Template>
                                          <Triggers>
                                            <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                                            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                          </Triggers>
                                          
                                                                                  
                                          <Listeners>
                                            <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                            <Change Handler="reloadFilterGrid(#{gridMainMenit}) , reloadFilterGrid(#{gridMainMenit_Detail}),reloadFilterGrid(#{gridPPIC}),reloadFilterGrid(#{GridDC}) ;" />
                                            <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                                          </Listeners>
                                          
                                                                                   
                                          
                                        </ext:ComboBox>
               <%--Type Stock--%>
               <ext:SelectBox ID="SelectBoxTipeServiceLevel" runat="server" FieldLabel="Jenis Laporan" SelectedIndex="2" AllowBlank="false" Width="350"   >
                  <Items>  
                      <ext:ListItem Value="1" Text="CSL PPIC"  />
                      <ext:ListItem Value="2" Text="CSL DC"/>
                      <ext:ListItem Value="3" Text="Fullfilment rate (FR) (Menit)" />
                      <ext:ListItem Value="4" Text="Line Fullfilment rate (LFR) (Menit)"/>
                  </Items>
                   <Listeners>
                     <Change Handler="reloadFilterGrid(#{gridMainMenit}),reloadFilterGrid(#{gridMainMenit_Detail}),reloadFilterGrid(#{gridPPIC}),validateRadio(#{SelectBoxTipeServiceLevel}.getValue(),#{gridMainMenit}, #{gridMainMenit_Detail}, #{TabPanelLogisticMenit},#{TabPanelLogMenitDetail},#{tabpnlPPIC},#{gridPPIC},#{tabpnlDC},#{GridDC});" /> 
                  </Listeners>
                                     
                 </ext:SelectBox>
               <%--Type Stock END--%>
              <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="true" hidden="true" />   
              </Items>
               <%--Proses--%>
              <Buttons>
                <ext:Button ID="Button1" runat="server" Text="Proses" Icon="CogStart" hidden="true">
                   <directevents>
                      <click onevent="report_ongenerate">
                         <eventmask showmask="true" />
                         <extraparams>
                           <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                           <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                           <ext:Parameter Name="Proses" Value="1" Mode="Raw" />
                         </extraParams>
                      </click>
                   </directEvents>
                </ext:Button>
                <ext:Button ID="Button3" runat="server" Text="Segarkan" Icon="ArrowRefresh">
                            <Listeners>
                              <Click Handler="refreshGrid(#{gridMainMenit}),refreshGrid(#{gridMainMenit_Detail}),refreshGrid(#{gridPPIC}),refreshGrid(#{GridDC});" />
                            </Listeners>
                          </ext:Button>
              </Buttons>
            </ext:FormPanel>
          </North>
          <Center>
            <ext:Panel ID="Panel2" runat="server" Layout="FitLayout" Frame="True" Border="True" >
              <Items>              
                <%--tes menit header--%>
                <ext:TabPanel ID="TabPanelLogisticMenit" runat="server" Height="300"  >
                  <Items>
                    <ext:Panel ID="pnlGridLogisticMenit" runat="server" Title="FR (Menit)" Layout="FitLayout">
                      <Items>
                        <ext:GridPanel ID="gridMainMenit"  runat="server">
                          <LoadMask ShowMask="true" />
                          <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="Store4" runat="server" RemoteSort="true">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000" CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <AutoLoadParams>
                                <ext:Parameter Name="start" Value="={0}" />
                                <ext:Parameter Name="limit" Value="={50}" />
                              </AutoLoadParams>
                              <BaseParams>
                                <ext:Parameter Name="start" Value="0" />
                                <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB_Menit}.getValue())" Mode="Raw" />
                                <ext:Parameter Name="model" Value="10060" />
                                <ext:Parameter Name="parameters" Value="[
                                  ['Nomor_SP', paramValueGetter(#{txNO_SPHO_Fltr_Menit}), ''],
                                  ['Nomor_SP_Cabang', paramValueGetter(#{txNO_SPCBG_Fltr_Menit}), ''],
                                  ['txPeriode1', paramRawValueGetter(#{txPeriode1}) , 'System.DateTime'],
                                  ['txPeriode2', paramRawValueGetter(#{txPeriode2}) , 'System.DateTime'],
                                  ['noSup', paramValueMultiGetter(#{cbCustomerHdr}) , 'System.String[]'],
                                  ['Gudang', paramValueGetter(#{cbGudang}, '0') , 'System.Char'],
                                  ['c_cusno',  paramValueGetter(#{cbCustomerHdr}), ''],
                                  ['Entry', #{hfUserID}.getValue(), 'System.String'],
                                  ['TypeService', paramValueGetter(#{SelectBoxTipeServiceLevel}), '']
                                  ]" Mode="Raw" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                  <Fields>
                                   <ext:RecordField Name="Nama_Cabang"/>
                                   <ext:RecordField Name="Nomor_SP_Cabang"/>
                                   <ext:RecordField Name="Nomor_SP"/>
                                   <ext:RecordField Name="Tgl_SP_Entry" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Nama_Jenis_SP"/>
                                   <ext:RecordField Name="Qty_Pesan_SP"/>
                                   <ext:RecordField Name="Qty_Acc_SP"/>
                                   <ext:RecordField Name="Nama_Gudang"/>
                                   <ext:RecordField Name="Nomor_PL"/>
                                   <ext:RecordField Name="Nama_Jenis_PL"/>
                                   <ext:RecordField Name="Tgl_PL_Entry" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Buat_Pl"/>
                                   <ext:RecordField Name="Nomor_Serah_Terima_PL"/>
                                   <ext:RecordField Name="Waktu_Serah_Terima_PL" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Serah_Terima_PL"/>
                                   <ext:RecordField Name="Nomor_Goods_Picker"/>
                                   <ext:RecordField Name="Waktu_Goods_Picked" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Goods_Picked"/>
                                   <ext:RecordField Name="Nomor_Goods_checked"/>
                                   <ext:RecordField Name="Waktu_Goods_Checked" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Goods_Checked"/>
                                   <ext:RecordField Name="Nomor_DO"/>
                                   <ext:RecordField Name="Waktu_Buat_DO" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Buat_DO"/>
                                   <ext:RecordField Name="Nomor_Pakcing_Palletizing"/>
                                   <ext:RecordField Name="Waktu_Buat_WP" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Pakcing_Palletizing"/>
                                   <ext:RecordField Name="Nomor_EP"/>
                                   <ext:RecordField Name="Waktu_Buat_EP" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Buat_EP"/>
                                   <ext:RecordField Name="Waktu_EP_Berangkat" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Loading"/>
                                   <ext:RecordField Name="Nomor_RNCabang"/>
                                   <ext:RecordField Name="Tgl_RNCabang" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Durasi_Pengiriman"/>
                                   <ext:RecordField Name="Qty_Diterima"/> 
                                   <ext:RecordField Name="Total_Waktu_Pemenuhan_SP"/>
                                   <ext:RecordField Name="Outstanding_Qty"/>
                                   <ext:RecordField Name="Leadtime"/>
                                   <ext:RecordField Name="Deviasi_Waktu_Pemenuhan"/>
                                   <ext:RecordField Name="Score_By_QTY"/>
                                   <ext:RecordField Name="Status_Pemenuhan_Qty"/>
                                   <ext:RecordField Name="Score_By_Time"/>
                                   <ext:RecordField Name="Status_Pemenuhan_Waktu"/>
                                   <ext:RecordField Name="SLA"/>
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                              <SortInfo Field="Nomor_SP" Direction="ASC" />
                            </ext:Store>
                          </Store>
                          <ColumnModel>
                            <Columns>
                             <ext:CommandColumn Width="25"></ext:CommandColumn>
                             <ext:Column ColumnID="Nomor_SP_Cabang" DataIndex="Nomor_SP_Cabang" Header="Nomor SP Cabang" Width="85" />
                             <ext:Column ColumnID="Nomor_SP" DataIndex="Nomor_SP" Header="Nomor SP" Width="100"  />
                             <ext:DateColumn ColumnID="Tgl_SP_Entry" DataIndex="Tgl_SP_Entry" Header="Waktu SP dibuat" Format="dd-MM-yyyy" />
                             <ext:Column ColumnID="Nama_Jenis_SP" DataIndex="Nama_Jenis_SP" Header="Nama Jenis SP" Width="85" />
                             <ext:Column ColumnID="Qty_Pesan_SP" DataIndex="Qty_Pesan_SP" Header="Qty Pesan SP" Width="85" />
                             <ext:Column ColumnID="Qty_Acc_SP" DataIndex="Qty_Acc_SP" Header="Qty Acc SP" Width="85" />
                             <ext:Column ColumnID="Nama_Gudang" DataIndex="Nama_Gudang" Header="Nama Gudang" Width="85" />
                             <ext:Column ColumnID="Nomor_PL" DataIndex="Nomor_PL" Header="Nomor PL" Width="85" />
                             <ext:Column ColumnID="Nama_Jenis_PL" DataIndex="Nama_Jenis_PL" Header="Jenis_PL" Width="85" />
                             <ext:DateColumn ColumnID="Tgl_PL_Entry" DataIndex="Tgl_PL_Entry" Header="Waktu PL dibuat" Format="dd-MM-yyyy" />
                             <ext:Column ColumnID="Durasi_Buat_Pl" DataIndex="Durasi_Buat_Pl" Header="Durasi Buat PL" Width="85" />
                             <ext:Column ColumnID="Nomor_Serah_Terima_PL" DataIndex="Nomor_Serah_Terima_PL" Header="Nomor Serah Terima PL" Width="85" />
                             <ext:DateColumn ColumnID="Waktu_Serah_Terima_PL" DataIndex="Waktu_Serah_Terima_PL" Header="Waktu Serah Terima PL" Format="dd-MM-yyyy" />                       
                             <ext:Column ColumnID="Durasi_Serah_Terima_PL" DataIndex="Durasi_Serah_Terima_PL" Header="Durasi Serah Terima PL" Width="85" />
                             <ext:Column ColumnID="Nomor_Goods_Picker" DataIndex="Nomor_Goods_Picker" Header="Nomor Goods Picker" Width="85" />
                             <ext:DateColumn ColumnID="Waktu_Goods_Picked" DataIndex="Waktu_Goods_Picked" Header="Waktu Goods Picked" Format="dd-MM-yyyy" />                       
                             <ext:Column ColumnID="Durasi_Goods_Picked" DataIndex="Durasi_Goods_Picked" Header="Durasi Goods Picked" Width="85" />
                             <ext:Column ColumnID="Nomor_Goods_checked" DataIndex="Nomor_Goods_checked" Header="Nomor Goods checked" Width="85" />
                             <ext:DateColumn ColumnID="Waktu_Goods_Checked" DataIndex="Waktu_Goods_Checked" Header="Waktu Goods Checked" Format="dd-MM-yyyy" />                         
                             <ext:Column ColumnID="Durasi_Goods_Checked" DataIndex="Durasi_Goods_Checked" Header="Durasi Goods Checked" Width="85" />
                             <ext:Column ColumnID="Nomor_DO" DataIndex="Nomor_DO" Header="Nomor DO" Width="85" />
                             <ext:DateColumn ColumnID="Waktu_Buat_DO" DataIndex="Waktu_Buat_DO" Header="Waktu Buat DO" Format="dd-MM-yyyy" />  
                             <ext:Column ColumnID="Durasi_Buat_DO" DataIndex="Durasi_Buat_DO" Header="Durasi Buat DO" Width="85" />
                             <ext:Column ColumnID="Nomor_Pakcing_Palletizing" DataIndex="Nomor_Pakcing_Palletizing" Header="Nomor Pakcing Palletizing" Width="85" />
                             <ext:DateColumn ColumnID="Waktu_Buat_WP" DataIndex="Waktu_Buat_WP" Header="Waktu_Buat_WP" Format="dd-MM-yyyy" />
                             <ext:Column ColumnID="Durasi_Pakcing_Palletizing" DataIndex="Durasi_Pakcing_Palletizing" Header="Durasi Pakcing Palletizing" Width="85" />
                             <ext:Column ColumnID="Nomor_EP" DataIndex="Nomor_EP" Header="Nomor EP" Width="85" />
                             <ext:DateColumn ColumnID="Waktu_Buat_EP" DataIndex="Waktu_Buat_EP" Header="Waktu Buat EP" Format="dd-MM-yyyy" />  
                             <ext:Column ColumnID="Durasi_Buat_EP" DataIndex="Durasi_Buat_EP" Header="Durasi Buat EP" Width="85" />
                             <ext:DateColumn ColumnID="Waktu_EP_Berangkat" DataIndex="Waktu_EP_Berangkat" Header="Waktu EP Berangkat" Format="dd-MM-yyyy" />
                             <ext:Column ColumnID="Durasi_Loading" DataIndex="Durasi_Loading" Header="Durasi Loading" Width="85" />
                             <ext:Column ColumnID="Nomor_RNCabang" DataIndex="Nomor_RNCabang" Header="Nomor RNCabang" Width="85" />
                             <ext:DateColumn ColumnID="Tgl_RNCabang" DataIndex="Tgl_RNCabang" Header="Tgl RNCabang" Format="dd-MM-yyyy" /> 
                             <ext:Column ColumnID="Durasi_Pengiriman" DataIndex="Durasi_Pengiriman" Header="Durasi Pengiriman" Width="85" />
                             <ext:Column ColumnID="Qty_Diterima" DataIndex="Qty_Diterima" Header="Qty Diterima" Width="85" />
                             <ext:Column ColumnID="Total_Waktu_Pemenuhan_SP" DataIndex="Total_Waktu_Pemenuhan_SP" Header="Total Waktu Pemenuhan SP" Width="85" />
                             <ext:Column ColumnID="Outstanding_Qty" DataIndex="Outstanding_Qty" Header="Outstanding Qty" Width="85" />
                             <ext:Column ColumnID="Leadtime" DataIndex="Leadtime" Header="Leadtime" Width="85" />
                             <ext:Column ColumnID="Deviasi_Waktu_Pemenuhan" DataIndex="Deviasi_Waktu_Pemenuhan" Header="Deviasi Waktu Pemenuhan" Width="85" />
                             <ext:Column ColumnID="Score_By_QTY" DataIndex="Score_By_QTY" Header="Score By QTY" Width="85" />
                             <ext:Column ColumnID="Status_Pemenuhan_Qty" DataIndex="Status_Pemenuhan_Qty" Header="Status Pemenuhan Qty" Width="85" />
                             <ext:Column ColumnID="Score_By_Time" DataIndex="Score_By_Time" Header="Score By Time" Width="85" />
                             <ext:Column ColumnID="Status_Pemenuhan_Waktu" DataIndex="Status_Pemenuhan_Waktu" Header="Status Pemenuhan Waktu" Width="85" />
                             <ext:Column ColumnID="SLA" DataIndex="SLA" Header="SLA" Width="85" />
                            </Columns>
                          </ColumnModel>
                          <TopBar>
                            <ext:Toolbar ID="Toolbar3" runat="server">
                                <Items>                           
                                   <ext:Button ID="Button5" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel"  hidden ="true">
                                        <Listeners>
                                            <Click Handler="saveData(#{hfGridData},#{gridMainMenit});" />
                                        </Listeners>
                                   </ext:Button>
                                   <%--Catak--%>
                                   <ext:ToolbarSeparator />
                                   <ext:button id="btnprintMenit" runat="server" icon="printer" text="Cetak ke Excel">
                                     <directevents>
                                       <click onevent="report_ongenerate">
                                         <eventmask showmask="true" />
                                         <extraparams>
                                            <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                                            <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                                            <ext:Parameter Name="cabang" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                                            <ext:Parameter Name="Gudang" Value="#{cbGudang}.getValue()" Mode="Raw" />
                                            <ext:Parameter Name="noRN" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                            <ext:Parameter Name="TypeService" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                            <ext:Parameter Name="noSPCBG" Value="#{txNO_SPCBG_Fltr_Menit}.getValue()" Mode="Raw" />
                                            <ext:Parameter Name="noSPHO" Value="#{txNO_SPHO_Fltr_Menit}.getValue()" Mode="Raw" />
                                         </extraparams>
                                       </click>
                                     </directevents>
                                   </ext:button>
                                </Items>
                            </ext:Toolbar>
                          </TopBar>
                          <BottomBar>
                            <ext:PagingToolbar ID="gmPagingBBMenit" runat="server" PageSize="50">
                              <Items>
                                <ext:Label ID="Label3" runat="server" Text="Page size:" />
                                <ext:ToolbarSpacer ID="ToolbarSpacer3" runat="server" Width="10" />
                                <ext:ComboBox ID="cbGmPagingBB_Menit" runat="server" Width="80">
                                  <Items>
                                    <ext:ListItem Text="5" />
                                    <ext:ListItem Text="10" />
                                    <ext:ListItem Text="20" />
                                    <ext:ListItem Text="50" />
                                    <ext:ListItem Text="100" />
                                  </Items>
                                  <SelectedItem Value="50" />
                                  <Listeners>
                                    <Select Handler="#{gmPagingBBMenit}.pageSize = parseInt(this.getValue()); #{gmPagingBBMenit}.doLoad();" />
                                  </Listeners>
                                </ext:ComboBox>
                              </Items>
                            </ext:PagingToolbar>
                          </BottomBar>
                          <View>
                              <ext:GridView ID="gvFilterMenit" runat="server" StandardHeaderRow="true">
                                 <HeaderRows>
                                    <ext:HeaderRow>
                                        <Columns>
                                           <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                                <Component>
                                                  <ext:Button ID="Button7" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                                    <Listeners>
                                                       <Click Handler="clearFilterGridHeader(#{gridMainMenit}, 
                                                                                           #{txNO_SP_Fltr},
                                                                                           #{txNO_SP_Fltr_DC},
                                                                                           #{txNO_SPCBG_Fltr_Menit},
                                                                                           #{txNO_SPHO_Fltr_Menit},
                                                                                           #{txNO_SP_Fltr_Menit_Detail}    
                                                                                            );reloadFilterGrid(#{gridMainMenit});
                                                                                              reloadFilterGrid(#{gridMainMenit_Detail});"
                                                        Buffer="300" Delay="300" />
                                                    </Listeners>
                                                  </ext:Button>
                                                </Component>
                                           </ext:HeaderColumn>
                                           <ext:HeaderColumn>
                                              <Component>
                                                <ext:TextField ID="txNO_SPCBG_Fltr_Menit" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                  <Listeners>
                                                    <KeyUp Handler="reloadFilterGrid(#{gridMainMenit})" Buffer="700" Delay="700" />
                                                  </Listeners>
                                                </ext:TextField>
                                              </Component>
                                           </ext:HeaderColumn>
                                           <ext:HeaderColumn>
                                              <Component>
                                                <ext:TextField ID="txNO_SPHO_Fltr_Menit" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                  <Listeners>
                                                    <KeyUp Handler="reloadFilterGrid(#{gridMainMenit})" Buffer="700" Delay="700" />
                                                  </Listeners>
                                                </ext:TextField>
                                              </Component>
                                           </ext:HeaderColumn>
                                        </Columns>
                                    </ext:HeaderRow>
                                 </HeaderRows>
                              </ext:GridView>
                          </View>
                        </ext:GridPanel>
                      </Items>
                    </ext:Panel>
                  </Items>
                </ext:TabPanel>
                <%--tes menit detail--%>
                <ext:TabPanel ID="TabPanelLogMenitDetail" runat="server" Height="295">
                  <Items>
                    <ext:Panel ID="pnlGridLogisticMenitDetail" runat="server" Title="LFR (Menit)" Layout="FitLayout">
                      <Items>
                        <ext:GridPanel ID="gridMainMenit_Detail"  runat="server"> 
                          <LoadMask ShowMask="true" />
                          <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="Store6" runat="server" RemoteSort="true">
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
                            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB_Menit_Detail}.getValue())" Mode="Raw" />
                            <ext:Parameter Name="model" Value="10060" />
                            <ext:Parameter Name="parameters" Value="[
                              ['Nomor_SP', paramValueGetter(#{txNO_SPHO_Fltr_Menit}), ''],
                              ['Nomor_SP_Cabang', paramValueGetter(#{txNO_SPCBG_Fltr_Menit}), ''],
                              ['txPeriode1', paramRawValueGetter(#{txPeriode1}) , 'System.DateTime'],
                              ['txPeriode2', paramRawValueGetter(#{txPeriode2}) , 'System.DateTime'],
                              ['noSup', paramValueMultiGetter(#{cbCustomerHdr}) , 'System.String[]'],
                              ['Gudang', paramValueGetter(#{cbGudang}, '0') , 'System.Char'],
                              ['c_cusno',  paramValueGetter(#{cbCustomerHdr}), ''],
                              ['Entry', #{hfUserID}.getValue(), 'System.String'],
                              ['TypeService', paramValueGetter(#{SelectBoxTipeServiceLevel}), '']                   
                              ]" Mode="Raw" />
                          </BaseParams>
                              <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                              <Fields>
                               <ext:RecordField Name="Nama_Cabang"/>
                               <ext:RecordField Name="Nomor_SP_Cabang"/>
                                <ext:RecordField Name="Kode_Barang"/>
                               <ext:RecordField Name="Nama_barang"/>
                               <ext:RecordField Name="Nomor_SP"/>
                               <ext:RecordField Name="Tgl_SP_Entry" Type="Date" DateFormat="M$" />
                               <ext:RecordField Name="Nama_Jenis_SP"/>
                               <ext:RecordField Name="Qty_Pesan_SP"/>
                               <ext:RecordField Name="Qty_Acc_SP"/>
                               <ext:RecordField Name="Nama_Gudang"/>
                                <ext:RecordField Name="Nomor_PL"/>
                                <ext:RecordField Name="Nama_Jenis_PL"/>
                                <ext:RecordField Name="Tgl_PL_Entry" Type="Date" DateFormat="M$" />
                               <ext:RecordField Name="Durasi_Buat_Pl"/>
                               <ext:RecordField Name="Nomor_Serah_Terima_PL"/>
                               <ext:RecordField Name="Waktu_Serah_Terima_PL" Type="Date" DateFormat="M$" />
                               <ext:RecordField Name="Durasi_Serah_Terima_PL"/>
                               <ext:RecordField Name="Nomor_Goods_Picker"/>
                                 <ext:RecordField Name="Waktu_Goods_Picked" Type="Date" DateFormat="M$" />
                                 <ext:RecordField Name="Durasi_Goods_Picked"/>
                                 <ext:RecordField Name="Nomor_Goods_checked"/>
                                 <ext:RecordField Name="Waktu_Goods_Checked" Type="Date" DateFormat="M$" />
                                 <ext:RecordField Name="Durasi_Goods_Checked"/>
                                 <ext:RecordField Name="Nomor_DO"/>
                                 <ext:RecordField Name="Waktu_Buat_DO" Type="Date" DateFormat="M$" />
                                 <ext:RecordField Name="Durasi_Buat_DO"/>
                                 <ext:RecordField Name="Nomor_Pakcing_Palletizing"/>
                                 <ext:RecordField Name="Waktu_Buat_WP" Type="Date" DateFormat="M$" />
                                 <ext:RecordField Name="Durasi_Pakcing_Palletizing"/>
                                 <ext:RecordField Name="Nomor_EP"/>
                                 <ext:RecordField Name="Waktu_Buat_EP" Type="Date" DateFormat="M$" />
                                 <ext:RecordField Name="Durasi_Buat_EP"/>
                                 <ext:RecordField Name="Waktu_EP_Berangkat" Type="Date" DateFormat="M$" />
                                 <ext:RecordField Name="Durasi_Loading"/>
                                  <ext:RecordField Name="Nomor_RNCabang"/>
                                 <ext:RecordField Name="Tgl_RNCabang" Type="Date" DateFormat="M$" />
                                 <ext:RecordField Name="Durasi_Pengiriman"/>
                                 <ext:RecordField Name="Qty_Diterima"/>
                                 <ext:RecordField Name="Total_Waktu_Pemenuhan_SP"/>
                                 <ext:RecordField Name="Outstanding_Qty"/>
                                 <ext:RecordField Name="Leadtime"/>
                                 <ext:RecordField Name="Deviasi_Waktu_Pemenuhan"/>
                                 <ext:RecordField Name="Score_By_QTY"/>
                                 <ext:RecordField Name="Status_Pemenuhan_Qty"/>
                                 <ext:RecordField Name="Score_By_Time"/>
                                 <ext:RecordField Name="Status_Pemenuhan_Waktu"/>
                                 <ext:RecordField Name="SLA"/>
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                              <SortInfo Field="Nomor_SP" Direction="ASC" />
                            </ext:Store>
                          </Store>
                          <ColumnModel>
                        <Columns>
                          <ext:CommandColumn Width="25">
                          
                            
                          </ext:CommandColumn>

                                
                  
                  
                               <ext:Column ColumnID="Nomor_SP_Cabang" DataIndex="Nomor_SP_Cabang" Header="Nomor_SP_Cabang" Width="85" />
                               <ext:Column ColumnID="Nomor_SP" DataIndex="Nomor_SP" Header="Nomor_SP" Width="100"  />
                               
                               
                               <ext:Column ColumnID="Kode_Barang" DataIndex="Kode_Barang" Header="Kode_Barang" Width="85" />
                               <ext:Column ColumnID="Nama_barang" DataIndex="Nama_barang" Header="Nama_barang" Width="100"  />
                               
                               
                               <ext:DateColumn ColumnID="Tgl_SP_Entry" DataIndex="Tgl_SP_Entry" Header="Sp Received" Format="dd-MM-yyyy" />                  
                              
                              
                              <ext:Column ColumnID="Nama_Jenis_SP" DataIndex="Nama_Jenis_SP" Header="Nama_Jenis_SP" Width="85" />
                              <ext:Column ColumnID="Qty_Pesan_SP" DataIndex="Qty_Pesan_SP" Header="Qty_Pesan_SP" Width="85" />
                              <ext:Column ColumnID="Qty_Acc_SP" DataIndex="Qty_Acc_SP" Header="Qty_Acc_SP" Width="85" />
                              <ext:Column ColumnID="Nama_Gudang" DataIndex="Nama_Gudang" Header="Nama_Gudang" Width="85" />
                              
                              
                                                           
                                <ext:Column ColumnID="Nomor_PL" DataIndex="Nomor_PL" Header="Nomor_PL" Width="85" />
                                <ext:Column ColumnID="Nama_Jenis_PL" DataIndex="Nama_Jenis_PL" Header="Nama_Jenis_PL" Width="85" />
                                <ext:DateColumn ColumnID="Tgl_PL_Entry" DataIndex="Tgl_PL_Entry" Header="Tgl_PL_Entry" Format="dd-MM-yyyy" />                       
                               
                               
                                                             
                               
                                <ext:Column ColumnID="Durasi_Buat_Pl" DataIndex="Durasi_Buat_Pl" Header="Durasi_Buat_Pl" Width="85" />
                                <ext:Column ColumnID="Nomor_Serah_Terima_PL" DataIndex="Nomor_Serah_Terima_PL" Header="Nomor_Serah_Terima_PL" Width="85" />
                                <ext:DateColumn ColumnID="Waktu_Serah_Terima_PL" DataIndex="Waktu_Serah_Terima_PL" Header="Waktu_Serah_Terima_PL" Format="dd-MM-yyyy" />                       
                               <ext:Column ColumnID="Durasi_Serah_Terima_PL" DataIndex="Durasi_Serah_Terima_PL" Header="Durasi_Serah_Terima_PL" Width="85" />
                                <ext:Column ColumnID="Nomor_Goods_Picker" DataIndex="Nomor_Goods_Picker" Header="Nomor_Goods_Picker" Width="85" />
                               
                               
                                                                
                               <ext:DateColumn ColumnID="Waktu_Goods_Picked" DataIndex="Waktu_Goods_Picked" Header="Waktu_Goods_Picked" Format="dd-MM-yyyy" />                       
                               <ext:Column ColumnID="Durasi_Goods_Picked" DataIndex="Durasi_Goods_Picked" Header="Durasi_Goods_Picked" Width="85" />
                               <ext:Column ColumnID="Nomor_Goods_checked" DataIndex="Nomor_Goods_checked" Header="Nomor_Goods_checked" Width="85" />
                               <ext:DateColumn ColumnID="Waktu_Goods_Checked" DataIndex="Waktu_Goods_Checked" Header="Waktu_Goods_Checked" Format="dd-MM-yyyy" />                         
                               <ext:Column ColumnID="Durasi_Goods_Checked" DataIndex="Durasi_Goods_Checked" Header="Durasi_Goods_Checked" Width="85" />
                               
                               
                            
                                 
                               
                               <ext:Column ColumnID="Nomor_DO" DataIndex="Nomor_DO" Header="Nomor_DO" Width="85" />
                               <ext:DateColumn ColumnID="Waktu_Buat_DO" DataIndex="Waktu_Buat_DO" Header="Waktu_Buat_DO" Format="dd-MM-yyyy" />  
                               <ext:Column ColumnID="Durasi_Buat_DO" DataIndex="Durasi_Buat_DO" Header="Durasi_Buat_DO" Width="85" />
                               <ext:Column ColumnID="Nomor_Pakcing_Palletizing" DataIndex="Nomor_Pakcing_Palletizing" Header="Nomor_Pakcing_Palletizing" Width="85" />
                               <ext:DateColumn ColumnID="Waktu_Buat_WP" DataIndex="Waktu_Buat_WP" Header="Waktu_Buat_WP" Format="dd-MM-yyyy" />  
                               
                               
                               
                                                               
                               
                               <ext:Column ColumnID="Durasi_Pakcing_Palletizing" DataIndex="Durasi_Pakcing_Palletizing" Header="Durasi_Pakcing_Palletizing" Width="85" />
                               <ext:Column ColumnID="Nomor_EP" DataIndex="Nomor_EP" Header="Nomor_EP" Width="85" />
                               <ext:DateColumn ColumnID="Waktu_Buat_EP" DataIndex="Waktu_Buat_EP" Header="Waktu_Buat_EP" Format="dd-MM-yyyy" />  
                                <ext:Column ColumnID="Durasi_Buat_EP" DataIndex="Durasi_Buat_EP" Header="Durasi_Buat_EP" Width="85" />
                               <ext:DateColumn ColumnID="Waktu_EP_Berangkat" DataIndex="Waktu_EP_Berangkat" Header="Waktu_EP_Berangkat" Format="dd-MM-yyyy" /> 
                           
                              
                              
                                
                               <ext:Column ColumnID="Durasi_Loading" DataIndex="Durasi_Loading" Header="Durasi_Loading" Width="85" />
                               <ext:Column ColumnID="Nomor_RNCabang" DataIndex="Nomor_RNCabang" Header="Nomor_RNCabang" Width="85" />
                               <ext:DateColumn ColumnID="Tgl_RNCabang" DataIndex="Tgl_RNCabang" Header="Tgl_RNCabang" Format="dd-MM-yyyy" /> 
                               <ext:Column ColumnID="Durasi_Pengiriman" DataIndex="Durasi_Pengiriman" Header="Durasi_Pengiriman" Width="85" />
                               <ext:Column ColumnID="Qty_Diterima" DataIndex="Qty_Diterima" Header="Qty_Diterima" Width="85" /> 
                                
                                
                              
                                 
                                 
                                                                
                                 <ext:Column ColumnID="Total_Waktu_Pemenuhan_SP" DataIndex="Total_Waktu_Pemenuhan_SP" Header="Total_Waktu_Pemenuhan_SP" Width="85" />
                                 <ext:Column ColumnID="Outstanding_Qty" DataIndex="Outstanding_Qty" Header="Outstanding_Qty" Width="85" />
                                 <ext:Column ColumnID="Leadtime" DataIndex="Leadtime" Header="Leadtime" Width="85" />
                                 <ext:Column ColumnID="Deviasi_Waktu_Pemenuhan" DataIndex="Deviasi_Waktu_Pemenuhan" Header="Deviasi_Waktu_Pemenuhan" Width="85" />
                                 
                                 
                                                                    
                                                                  
                                  
                               <ext:Column ColumnID="Score_By_QTY" DataIndex="Score_By_QTY" Header="Score_By_QTY" Width="85" />
                               <ext:Column ColumnID="Status_Pemenuhan_Qty" DataIndex="Status_Pemenuhan_Qty" Header="Status_Pemenuhan_Qty" Width="85" />
                               <ext:Column ColumnID="Score_By_Time" DataIndex="Score_By_Time" Header="Score_By_Time" Width="85" />
                               <ext:Column ColumnID="Status_Pemenuhan_Waktu" DataIndex="Status_Pemenuhan_Waktu" Header="Status_Pemenuhan_Waktu" Width="85" />
                               <ext:Column ColumnID="SLA" DataIndex="SLA" Header="SLA" Width="85" />
                              
                                                  
                              
                             
                        </Columns>
                      </ColumnModel>
                          <TopBar>
                          <ext:Toolbar ID="Toolbar4" runat="server">
                        <Items>                           
                            <ext:Button ID="Button6" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel"  hidden ="true">
                                <Listeners>
                                    <Click Handler="saveData(#{hfGridData},#{gridMainMenit_Detail});" />
                                </Listeners>
                            </ext:Button>
                            
                          <%--Catak--%>
                              <ext:ToolbarSeparator />
                              <ext:button id="btnprintMenit_Detail" runat="server" icon="printer" text="Cetak ke Excel">
                                  <directevents>
                                    <click onevent="report_ongenerate">
                                      <eventmask showmask="true" />
                                      <extraparams>
                                      
                                          <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="cabang" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="Gudang" Value="#{cbGudang}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noRN" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="TypeService" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noSPCBG" Value="#{txNO_SPCBG_Fltr_Menit}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noSPHO" Value="#{txNO_SPHO_Fltr_Menit}.getValue()" Mode="Raw" />
                                        
                                      </extraparams>
                                    </click>
                                  </directevents>
                                </ext:button>
                      
                        
                             </Items>
                    </ext:Toolbar>
                       </TopBar>
                          <BottomBar>
                         
                         
                        <ext:PagingToolbar ID="gmPagingBBMenit_Detail" runat="server" PageSize="50">
                          <Items>
                            <ext:Label ID="Label4" runat="server" Text="Page size:" />
                            <ext:ToolbarSpacer ID="ToolbarSpacer4" runat="server" Width="10" />
                            <ext:ComboBox ID="cbGmPagingBB_Menit_Detail" runat="server" Width="80">
                              <Items>
                                <ext:ListItem Text="5" />
                                <ext:ListItem Text="10" />
                                <ext:ListItem Text="20" />
                                <ext:ListItem Text="50" />
                                <ext:ListItem Text="100" />
                              </Items>
                              <SelectedItem Value="50" />
                              <Listeners>
                                <Select Handler="#{gmPagingBBMenit_Detail}.pageSize = parseInt(this.getValue()); #{gmPagingBBMenit_Detail}.doLoad();" />
                              </Listeners>
                            </ext:ComboBox>
                          </Items>
                        </ext:PagingToolbar>
                    
                    
                  </BottomBar>
                          <View>
                    <ext:GridView ID="gvFilterMenit_Detail" runat="server" StandardHeaderRow="true">
                      <HeaderRows>
                      
                        <ext:HeaderRow>
                          <Columns>
                          
                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                        <Component>
                          <ext:Button ID="Button9" runat="server" Icon="Cancel" ToolTip="Clear filter">
                            <Listeners>
                               <Click Handler="clearFilterGridHeader(#{gridMainMenit_Detail}, 
                                                                   #{txNO_SP_Fltr},
                                                                   #{txNO_SP_Fltr_DC},
                                                                   #{txNO_SPCBG_Fltr_Menit_Detail},
                                                                   #{txNO_SPHO_Fltr_Menit_Header},
                                                                   
                                                                   #{txNO_SP_Fltr_Menit_Detail}    
                                                                    );reloadFilterGrid(#{gridMainMenit});
                                                                      reloadFilterGrid(#{gridMainMenit_Detail});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                     
                      
                        </ext:HeaderColumn>
                        <ext:HeaderColumn>
                              <Component>
                                <ext:TextField ID="txNO_SPCBG_Fltr_Menit_Detail" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                  <Listeners>
                                    <KeyUp Handler="reloadFilterGrid(#{gridMainMenit_Detail})" Buffer="700" Delay="700" />
                                  </Listeners>
                                </ext:TextField>
                              </Component>
                        </ext:HeaderColumn>
                        <ext:HeaderColumn>
                              <Component>
                                <ext:TextField ID="txNO_SPHO_Fltr_Menit_Header" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                  <Listeners>
                                    <KeyUp Handler="reloadFilterGrid(#{gridMainMenit_Detail})" Buffer="700" Delay="700" />
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
                </ext:TabPanel>
                <%--CSL PPIC --%>
                <ext:TabPanel ID="tabpnlPPIC" runat="server" Height="300"  >
                  <Items>
                    <ext:Panel ID="pnlPPIC" runat="server" Title="CSL PPIC" Layout="FitLayout">
                      <Items>
                        <ext:GridPanel ID="GridPPIC"  runat="server"> 
                          <LoadMask ShowMask="true" />
                          <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="Store1" runat="server" RemoteSort="true">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000" CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <AutoLoadParams>
                                <ext:Parameter Name="start" Value="={0}" />
                                <ext:Parameter Name="limit" Value="={50}" />
                              </AutoLoadParams>
                              <BaseParams>
                                <ext:Parameter Name="start" Value="0" />
                                <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB_Menit}.getValue())" Mode="Raw" />
                                <ext:Parameter Name="model" Value="10060" />
                                <ext:Parameter Name="parameters" Value="[
                                  ['Nomor_SP', paramValueGetter(#{txNO_SPHO_Fltr_Menit}), ''],
                                  ['Nomor_SP_Cabang', paramValueGetter(#{txNO_SPCBG_Fltr_Menit}), ''],
                                  ['txPeriode1', paramRawValueGetter(#{txPeriode1}) , 'System.DateTime'],
                                  ['txPeriode2', paramRawValueGetter(#{txPeriode2}) , 'System.DateTime'],
                                  ['noSup', paramValueMultiGetter(#{cbCustomerHdr}) , 'System.String[]'],
                                  ['Gudang', paramValueGetter(#{cbGudang}, '0') , 'System.Char'],
                                  ['c_cusno',  paramValueGetter(#{cbCustomerHdr}), ''],
                                  ['Entry', #{hfUserID}.getValue(), 'System.String'],
                                  ['TypeService', paramValueGetter(#{SelectBoxTipeServiceLevel}), '']
                                  ]" Mode="Raw" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                  <Fields>
                                    <ext:RecordField Name="Cabang"/>
                                    <ext:RecordField Name="SP_HO"/>
                                    <ext:RecordField Name="SP_Cabang"/>
                                    <ext:RecordField Name="Waktu_SP_dibuat" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Jenis_SP" />
                                    <ext:RecordField Name="QTY_SP_dibuat" />
                                    <ext:RecordField Name="QTY_SP_disetujui" />                                    
                                    <ext:RecordField Name="ETD_SP" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="ETA_SP" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="SP_PLAN" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="QTY_sisa_sp_belum_dilayani" />
                                    <ext:RecordField Name="Nomor_PL_yang_dibuat" />
                                    <ext:RecordField Name="Qty_PL_yang_dibuat" />
                                    <ext:RecordField Name="Gudang" />
                                    <ext:RecordField Name="Tanggal_PL_dibuat" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Tanggal_PL_diprint" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Tipe_PL" />
                                    <ext:RecordField Name="durasi_buat_pl" />
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                              <SortInfo Field="Nomor_SP" Direction="ASC" />
                            </ext:Store>
                          </Store>
                          <ColumnModel>
                            <Columns>
                              <ext:CommandColumn Width="25"></ext:CommandColumn>
                              <ext:Column ColumnID="Cabang" DataIndex="Cabang" Header="Cabang" Width="85" />
                              <ext:Column ColumnID="SPHO" DataIndex="SP_HO" Header="Nomor SP HO" Width="85" />
                              <ext:Column ColumnID="SPCabang" DataIndex="SP_Cabang" Header="Nomor SP Cabang" Width="85" />
                              <ext:DateColumn ColumnID="SPDate" DataIndex="Waktu_SP_dibuat" Header="Waktu SP Dibuat" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="SPType" DataIndex="Jenis_SP" Header="Jenis SP" Width="85" />
                              <ext:Column ColumnID="QTYSP" DataIndex="QTY_SP_dibuat" Header="Qty SP dibuat" Width="85" />
                              <ext:Column ColumnID="QTYSPACC" DataIndex="QTY_SP_disetujui" Header="Qty SP disetujui" Width="85" />
                              <ext:DateColumn ColumnID="ETDSP" DataIndex="ETD_SP" Header="ETD SP" Width="85" Format="dd-MM-yyyy" />
                              <ext:DateColumn ColumnID="ETASP" DataIndex="ETA_SP" Header="ETA SP" Width="85" Format="dd-MM-yyyy" />
                              <ext:DateColumn ColumnID="SPPLAN" DataIndex="SP_PLAN" Header="SP PLAN" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="QTYSisaSP" DataIndex="QTY_sisa_sp_belum_dilayani" Header="Sisa SP blm terlayani" Width="85" />
                              <ext:Column ColumnID="PL" DataIndex="Nomor_PL_yang_dibuat" Header="Nomor PL" Width="85" />
                              <ext:Column ColumnID="QTYPL" DataIndex="Qty_PL_yang_dibuat" Header="Total Qty PL" Width="85" />
                              <ext:Column ColumnID="GUDANG" DataIndex="Gudang" Header="Gudang asal pembuatan PL" Width="85" />
                              <ext:DateColumn ColumnID="PLDate" DataIndex="Tanggal_PL_dibuat" Header="Waktu buat PL" Width="85" Format="dd-MM-yyyy" />
                              <ext:DateColumn ColumnID="PLDatePrint" DataIndex="Tanggal_PL_diprint" Header="Waktu Print PL" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="PLType" DataIndex="Tipe_PL" Header="Jenis PL" Width="85" />
                              <ext:Column ColumnID="PLTime" DataIndex="durasi_buat_pl" Header="Durasi buat PL" Width="85" />
                            </Columns>
                          </ColumnModel>
                          <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">
                              <Items>                           
                                <ext:Button ID="Button2" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel"  hidden ="true">
                                    <Listeners>
                                        <Click Handler="saveData(#{hfGridData},#{gridMainMenit});" />
                                    </Listeners>
                                </ext:Button>
                              <%--Cetak--%>
                              <ext:ToolbarSeparator />
                              <ext:button id="Button4" runat="server" icon="printer" text="Cetak ke Excel">
                                  <directevents>
                                    <click onevent="report_ongenerate">
                                      <eventmask showmask="true" />
                                      <extraparams>
                                          <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="cabang" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="Gudang" Value="#{cbGudang}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noRN" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="TypeService" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noSPCBG" Value="#{txNO_SPCBG_Fltr_Menit}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noSPHO" Value="#{txNO_SPHO_Fltr_Menit}.getValue()" Mode="Raw" />
                                      </extraparams>
                                    </click>
                                  </directevents>
                                </ext:button>
                              </Items>
                            </ext:Toolbar>
                          </TopBar>
                          <BottomBar>
                            <ext:PagingToolbar ID="PagingToolbar1" runat="server" PageSize="50">
                              <Items>
                                <ext:Label ID="Label2" runat="server" Text="Page size:" />
                                <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                                <ext:ComboBox ID="ComboBox1" runat="server" Width="80">
                                  <Items>
                                    <ext:ListItem Text="5" />
                                    <ext:ListItem Text="10" />
                                    <ext:ListItem Text="20" />
                                    <ext:ListItem Text="50" />
                                    <ext:ListItem Text="100" />
                                  </Items>
                                  <SelectedItem Value="50" />
                                  <Listeners>
                                    <Select Handler="#{gmPagingBBMenit}.pageSize = parseInt(this.getValue()); #{gmPagingBBMenit}.doLoad();" />
                                  </Listeners>
                                </ext:ComboBox>
                              </Items>
                            </ext:PagingToolbar>
                          </BottomBar>
                          <View>
                            <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                              <HeaderRows>
                                <ext:HeaderRow>
                                  <Columns>
                                    <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                      <Component>
                                        <ext:Button ID="Button8" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                          <Listeners>
                                            <Click Handler="clearFilterGridHeader(#{gridMainMenit}, 
                                                                       #{txNO_SP_Fltr},
                                                                       #{txNO_SP_Fltr_DC},
                                                                       #{txNO_SPCBG_Fltr_Menit},
                                                                       #{txNO_SPHO_Fltr_Menit},
                                                                       #{txNO_SP_Fltr_Menit_Detail}    
                                                                        );reloadFilterGrid(#{gridMainMenit});
                                                                          reloadFilterGrid(#{gridMainMenit_Detail});reloadFilterGrid(#{gridPPIC});reloadFilterGrid(#{GridDC});"
                                                   Buffer="300" Delay="300" />
                                          </Listeners>
                                        </ext:Button>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                      <Component>
                                        <ext:TextField ID="TextField1" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                          <Listeners>
                                            <KeyUp Handler="reloadFilterGrid(#{gridMainMenit})" Buffer="700" Delay="700" />
                                          </Listeners>
                                        </ext:TextField>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                      <Component>
                                        <ext:TextField ID="TextField2" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                          <Listeners>
                                            <KeyUp Handler="reloadFilterGrid(#{gridMainMenit})" Buffer="700" Delay="700" />
                                          </Listeners>
                                        </ext:TextField>
                                      </Component>
                                    </ext:HeaderColumn>
                                  </Columns>
                                </ext:HeaderRow>
                              </HeaderRows>
                            </ext:GridView>
                          </View>
                        </ext:GridPanel>
                      </Items>
                    </ext:Panel>
                  </Items>
                </ext:TabPanel>
                <%--CSL DC --%>
                <ext:TabPanel ID="tabpnlDC" runat="server" Height="300"  >
                  <Items>
                    <ext:Panel ID="pnlDC" runat="server" Title="CSL DC" Layout="FitLayout">
                      <Items>
                        <ext:GridPanel ID="GridDC"  runat="server"> 
                          <LoadMask ShowMask="true" />
                          <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="Store5" runat="server" RemoteSort="true">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000" CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <AutoLoadParams>
                                <ext:Parameter Name="start" Value="={0}" />
                                <ext:Parameter Name="limit" Value="={50}" />
                              </AutoLoadParams>
                              <BaseParams>
                                <ext:Parameter Name="start" Value="0" />
                                <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB_Menit}.getValue())" Mode="Raw" />
                                <ext:Parameter Name="model" Value="10060" />
                                <ext:Parameter Name="parameters" Value="[
                                  ['Nomor_SP', paramValueGetter(#{txNO_SPHO_Fltr_Menit}), ''],
                                  ['Nomor_SP_Cabang', paramValueGetter(#{txNO_SPCBG_Fltr_Menit}), ''],
                                  ['txPeriode1', paramRawValueGetter(#{txPeriode1}) , 'System.DateTime'],
                                  ['txPeriode2', paramRawValueGetter(#{txPeriode2}) , 'System.DateTime'],
                                  ['noSup', paramValueMultiGetter(#{cbCustomerHdr}) , 'System.String[]'],
                                  ['Gudang', paramValueGetter(#{cbGudang}, '0') , 'System.Char'],
                                  ['c_cusno',  paramValueGetter(#{cbCustomerHdr}), ''],
                                  ['Entry', #{hfUserID}.getValue(), 'System.String'],
                                  ['TypeService', paramValueGetter(#{SelectBoxTipeServiceLevel}), '']
                                  ]" Mode="Raw" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                  <Fields>
                                    <ext:RecordField Name="v_cunam"/>
                                    <ext:RecordField Name="Nomor_SP"/>
                                    <ext:RecordField Name="Tanggal_SP" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Tipe_SP" />
                                    <ext:RecordField Name="Nomor_PL" />                                    
                                    <ext:RecordField Name="Tanggal_PL" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Tipe_PL" />
                                    <ext:RecordField Name="n_qty" />
                                    <ext:RecordField Name="Nomor_Serah_Terima_PL" />
                                    <ext:RecordField Name="Tanggal_Serah_Terima_PL" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Petugas_Serah_Terima_PL" />
                                    <ext:RecordField Name="Nomor_Picking" />
                                    <ext:RecordField Name="Tanggal_picking" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Petugas_Picking" />
                                    <ext:RecordField Name="Nomor_Checking" />
                                    <ext:RecordField Name="Tanggal_Checking" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Petugas_Checking" />
                                    <ext:RecordField Name="Gudang" />
                                    <ext:RecordField Name="Nomor_DO" />
                                    <ext:RecordField Name="Tanggal_DO" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Admin_DO" />
                                    <ext:RecordField Name="Nomor_Packing" />
                                    <ext:RecordField Name="Tanggal_packing" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Petugas_Packing" />
                                    <ext:RecordField Name="Koli_MB" />
                                    <ext:RecordField Name="Koli_receh" />
                                    <ext:RecordField Name="Nomor_Pengiriman" />
                                    <ext:RecordField Name="Tanggal_Kirim" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Admin_pengiriman" />
                                    <ext:RecordField Name="Jam_Berangkat" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="Nomor_RN_Cabang" />
                                    <ext:RecordField Name="Tanggal_RN_Cabang" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="ETD_SP" Type="Date" DateFormat="M$" />
                                    <ext:RecordField Name="ETA_SP" Type="Date" DateFormat="M$" />
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                              <SortInfo Field="Nomor_SP" Direction="ASC" />
                            </ext:Store>
                          </Store>
                          <ColumnModel>
                            <Columns>
                              <ext:CommandColumn Width="25"></ext:CommandColumn>
                              <ext:Column ColumnID="Cabang" DataIndex="v_cunam" Header="Cabang" Width="85" />
                              <ext:Column ColumnID="SPHO" DataIndex="Nomor_SP" Header="Nomor SP HO" Width="85" />
                              <ext:DateColumn ColumnID="SPDate" DataIndex="Tanggal_SP" Header="Waktu SP Dibuat" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="TipeSP" DataIndex="Tipe_SP" Header="Tipe SP" Width="85"/>
                              <ext:Column ColumnID="NomorPL" DataIndex="Nomor_PL" Header="Nomor PL" Width="85" />
                              <ext:DateColumn ColumnID="TanggalPL" DataIndex="Tanggal_PL" Header="Tanggal PL" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="TipePL" DataIndex="Tipe_PL" Header="Tipe PL" Width="85" />
                              <ext:Column ColumnID="n_qty" DataIndex="n_qty" Header="Qty PL" Width="85" />
                              <ext:Column ColumnID="NomorSerahTerimaPL" DataIndex="Nomor_Serah_Terima_PL" Header="Nomor Serah Terima PL" Width="85" />
                              <ext:DateColumn ColumnID="TanggalSerahTerimaPL" DataIndex="Tanggal_Serah_Terima_PL" Header="Tanggal Serah Terima PL" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="PetugasSerahTerimaPL" DataIndex="Petugas_Serah_Terima_PL" Header="Petugas Serah Terima PL" Width="85" />
                              <ext:Column ColumnID="NomorPicking" DataIndex="Nomor_Picking" Header="Nomor Picking" Width="85" />
                              <ext:DateColumn ColumnID="Tanggalpicking" DataIndex="Tanggal_picking" Header="Tanggal picking" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="PetugasPicking" DataIndex="Petugas_Picking" Header="Petugas Picking" Width="85" />
                              <ext:Column ColumnID="NomorChecking" DataIndex="Nomor_Checking" Header="Nomor Checking" Width="85" />
                              <ext:DateColumn ColumnID="TanggalChecking" DataIndex="Tanggal_Checking" Header="Tanggal Checking" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="PetugasChecking" DataIndex="Petugas_Checking" Header="Petugas Checking" Width="85" />
                              <ext:Column ColumnID="NomorDO" DataIndex="Nomor_DO" Header="Nomor DO" Width="85" />
                              <ext:DateColumn ColumnID="TanggalDO" DataIndex="Tanggal_DO" Header="Tanggal DO" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="Admin_DO" DataIndex="Admin_DO" Header="Admin DO" Width="85" />
                              <ext:Column ColumnID="NomorPacking" DataIndex="Nomor_Packing" Header="Nomor Packing" Width="85" />
                              <ext:DateColumn ColumnID="Tanggalpacking" DataIndex="Tanggal_packing" Header="Tanggal packing" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="PetugasPacking" DataIndex="Petugas_Packing" Header="Petugas Packing" Width="85" />
                              <ext:Column ColumnID="KoliMB" DataIndex="Koli_MB" Header="Koli MB" Width="85" />
                              <ext:Column ColumnID="Kolireceh" DataIndex="Koli_receh" Header="Koli receh" Width="85" />
                              <ext:Column ColumnID="NomorPengiriman" DataIndex="Nomor_Pengiriman" Header="Nomor Pengiriman" Width="85" />
                              <ext:DateColumn ColumnID="TanggalKirim" DataIndex="Tanggal_Kirim" Header="Tanggal Kirim" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="Adminpengiriman" DataIndex="Admin_pengiriman" Header="Admin pengiriman" Width="85" />
                              <ext:DateColumn ColumnID="JamBerangkat" DataIndex="Jam_Berangkat" Header="Jam Berangkat" Width="85" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="NomorRNCabang" DataIndex="Nomor_RN_Cabang" Header="Nomor RN Cabang" Width="85" />
                              <ext:DateColumn ColumnID="TanggalRNCabang" DataIndex="Tanggal_RN_Cabang" Header="Tanggal RN Cabang" Width="85" Format="dd-MM-yyyy" />
                              <ext:DateColumn ColumnID="ETDSP" DataIndex="ETD_SP" Header="ETD SP" Width="85" Format="dd-MM-yyyy" />
                              <ext:DateColumn ColumnID="ETASP" DataIndex="ETA_SP" Header="ETA SP" Width="85" Format="dd-MM-yyyy" />
                            </Columns>
                          </ColumnModel>
                          <TopBar>
                            <ext:Toolbar ID="Toolbar2" runat="server">
                              <Items>                           
                                <ext:Button ID="Button10" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel"  hidden ="true">
                                    <Listeners>
                                        <Click Handler="saveData(#{hfGridData},#{gridMainMenit});" />
                                    </Listeners>
                                </ext:Button>
                              <%--Cetak--%>
                              <ext:ToolbarSeparator />
                              <ext:button id="Button11" runat="server" icon="printer" text="Cetak ke Excel">
                                  <directevents>
                                    <click onevent="report_ongenerate">
                                      <eventmask showmask="true" />
                                      <extraparams>
                                          <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="cabang" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="Gudang" Value="#{cbGudang}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noRN" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="TypeService" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noSPCBG" Value="#{txNO_SPCBG_Fltr_Menit}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="noSPHO" Value="#{txNO_SPHO_Fltr_Menit}.getValue()" Mode="Raw" />
                                      </extraparams>
                                    </click>
                                  </directevents>
                                </ext:button>
                              </Items>
                            </ext:Toolbar>
                          </TopBar>
                          <BottomBar>
                            <ext:PagingToolbar ID="PagingToolbar2" runat="server" PageSize="50">
                              <Items>
                                <ext:Label ID="Label5" runat="server" Text="Page size:" />
                                <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
                                <ext:ComboBox ID="ComboBox2" runat="server" Width="80">
                                  <Items>
                                    <ext:ListItem Text="5" />
                                    <ext:ListItem Text="10" />
                                    <ext:ListItem Text="20" />
                                    <ext:ListItem Text="50" />
                                    <ext:ListItem Text="100" />
                                  </Items>
                                  <SelectedItem Value="50" />
                                  <Listeners>
                                    <Select Handler="#{gmPagingBBMenit}.pageSize = parseInt(this.getValue()); #{gmPagingBBMenit}.doLoad();" />
                                  </Listeners>
                                </ext:ComboBox>
                              </Items>
                            </ext:PagingToolbar>
                          </BottomBar>
                          <View>
                            <ext:GridView ID="GridView2" runat="server" StandardHeaderRow="true">
                              <HeaderRows>
                                <ext:HeaderRow>
                                  <Columns>
                                    <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                      <Component>
                                        <ext:Button ID="Button12" runat="server" Icon="Cancel" ToolTip="Clear filter">
                                          <Listeners>
                                            <Click Handler="clearFilterGridHeader(#{gridMainMenit}, 
                                                                       #{txNO_SP_Fltr},
                                                                       #{txNO_SP_Fltr_DC},
                                                                       #{txNO_SPCBG_Fltr_Menit},
                                                                       #{txNO_SPHO_Fltr_Menit},
                                                                       #{txNO_SP_Fltr_Menit_Detail}    
                                                                        );reloadFilterGrid(#{gridMainMenit});
                                                                          reloadFilterGrid(#{gridMainMenit_Detail});reloadFilterGrid(#{gridPPIC});reloadFilterGrid(#{GridDC});"
                                                   Buffer="300" Delay="300" />
                                          </Listeners>
                                        </ext:Button>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                      <Component>
                                        <ext:TextField ID="TextField3" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                          <Listeners>
                                            <KeyUp Handler="reloadFilterGrid(#{gridMainMenit})" Buffer="700" Delay="700" />
                                          </Listeners>
                                        </ext:TextField>
                                      </Component>
                                    </ext:HeaderColumn>
                                    <ext:HeaderColumn>
                                      <Component>
                                        <ext:TextField ID="TextField4" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                          <Listeners>
                                            <KeyUp Handler="reloadFilterGrid(#{gridMainMenit})" Buffer="700" Delay="700" />
                                          </Listeners>
                                        </ext:TextField>
                                      </Component>
                                    </ext:HeaderColumn>
                                  </Columns>
                                </ext:HeaderRow>
                              </HeaderRows>
                            </ext:GridView>
                          </View>
                        </ext:GridPanel>
                      </Items>
                    </ext:Panel>
                  </Items>
                </ext:TabPanel>
              </Items>
            </ext:Panel>
          </Center>
       </ext:BorderLayout>
     </Items>
   </ext:Panel>
  </Items>
</ext:Viewport>

<ext:Window ID="wndDown" runat="server" Hidden="true" />