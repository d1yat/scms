<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Recall.ascx.cs"
  Inherits="recall" %>
 
<%@ Import Namespace="System.Xml.Xsl" %>
<%@ Import Namespace="System.Xml" %>


<script type="text/javascript">
        
  
    var validateRadio = function(SelectBoxTipeServiceLevelJava, gridMainjava,gridMainDCJava ,TabPanelRecallHeaderJava, TabPanelRecallDetailJava) {



        if (SelectBoxTipeServiceLevelJava == "1") {

            TabPanelRecallHeaderJava.setVisible(true);
            TabPanelRecallDetailJava.setVisible(false);
           



        }
       else if (SelectBoxTipeServiceLevelJava == "2") {


            TabPanelRecallHeaderJava.setVisible(false);
            TabPanelRecallDetailJava.setVisible(true);
           
            





        };




    };






    var setGetBatch = function(chkDefault, store, chkDeclined) {
        var chk = chkDefault.getValue();

        if (chk) {
            store.each(function(e) {
                if (e.get('KOLOM1') != 'Kosong') {
                    e.set('KOLOM_1', true);
                }

                if (e.get('KOLOM2') != 'Kosong') {
                    e.set('KOLOM_2', true);
                }

                if (e.get('KOLOM3') != 'Kosong') {
                    e.set('KOLOM_3', true);
                }

                if (e.get('KOLOM4') != 'Kosong') {
                    e.set('KOLOM_4', true);
                }

            });
        }
        else {
            store.each(function(e) {
                e.set('KOLOM_1', false);
                e.set('KOLOM_2', false);
                e.set('KOLOM_3', false);
                e.set('KOLOM_4', false);
            });
        }
    }

    var commandGridFunction = function(wndSP) {
        var store = '';

        wndSP.hide = false;
        wndSP.show();

    }

    var storeToDetailGrid = function(grid, batch, chkBatch) {

        var store = grid.getStore();

        var nPart = "";
        var i = 0;
        var JumBatch = 0;

        if (!chkBatch.getValue()) {
            for (i = 0; i < store.data.items.length; i++) {


                if (store.data.items[i].data.KOLOM_1) {
                    if (store.data.items[i].data.KOLOM1 != "Kosong") {
                        nPart = nPart + "|" + store.data.items[i].data.KOLOM1;
                    }
                    JumBatch += 1;
                }

                if (store.data.items[i].data.KOLOM_2) {
                    if (store.data.items[i].data.KOLOM2 != "Kosong") {
                        nPart = nPart + "|" + store.data.items[i].data.KOLOM2;
                    }
                }

                if (store.data.items[i].data.KOLOM_3) {
                    if (store.data.items[i].data.KOLOM3 != "Kosong") {
                        nPart = nPart + "|" + store.data.items[i].data.KOLOM3;
                    }
                    JumBatch += 1;
                }

                if (store.data.items[i].data.KOLOM_4) {
                    if (store.data.items[i].data.KOLOM4 != "Kosong") {
                        nPart = nPart + "|" + store.data.items[i].data.KOLOM4;
                    }
                    JumBatch += 1;
                }
            }

            nPart = nPart.substring(1);
            batch.setValue(nPart);
            
        }
        else {
            batch.setValue("ALL BATCH");
        }


    }

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

<%--Indra 20181011FM--%>
<%--<Content>--%>
<ext:Hidden ID="hfGridData" runat="server" />
<ext:Hidden ID="hidWndDown" runat="server" />
<ext:Hidden ID="hfUserID" runat="server" />    
<%--</Content>--%>
<ext:Viewport runat="server" Layout="FitLayout">
  <Items>
   <ext:Panel runat="server" >
      <Items>        
        <ext:BorderLayout ID="bllayout" runat="server" >
          <North MinHeight="125" MaxHeight="160" Collapsible="false">
            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Border="True" Padding="5" Height="160" >
              <Items>              
               <%--Periode Sales--%>  
                 <%--Indra 20181011FM--%>
                 <%--<ext:CompositeField runat="server" FieldLabel="Periode Sales">--%>
                 <ext:CompositeField runat="server" FieldLabel="Periode Distribusi">
                  <Items>
                    <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                      AllowBlank="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
                      </CustomConfig>
                      <Listeners>
                       <%--<Change Handler="reloadFilterGrid(#{gridMain}),reloadFilterGrid(#{gridMainDC}) ;" />--%>
                      </Listeners>                          
                    </ext:DateField>
                    <%--Indra 20181011FM--%>
                    <%--<ext:Label runat="server" Text="-" />--%>
                    <ext:Label ID="Label3" runat="server" Text="s.d" />
                    <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                      AllowBlank="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" /> 
                      </CustomConfig>                          
                       <Listeners>
                       <%--<Change Handler="reloadFilterGrid(#{gridMain}),reloadFilterGrid(#{gridMainDC}) ;" />--%>
                      </Listeners>                          
                    </ext:DateField>
                  </Items>                     
                 </ext:CompositeField>                    
                 <%--Indra 20181011FM--%> 
                 <%--<ext:CompositeField ID="CompositeField4" runat="server" FieldLabel="Tgl Penarikan">--%>
                 <ext:CompositeField ID="CompositeField4" runat="server" FieldLabel="Tgl. Mulai Recall">
                  <Items>
                      <%--<ext:DateField ID="txDayResiHdr" runat="server" FieldLabel="Tgl Penarikan" AllowBlank="false" Format="dd-MM-yyyy" />                                  --%>  
                      <ext:DateField ID="txPeriode3" runat="server" FieldLabel="Tgl. Mulai Recall" AllowBlank="false" Format="dd-MM-yyyy" > 
                      <CustomConfig>
                        <ext:ConfigItem Name="startDateField" Value="#{txPeriode2}" Mode="Value" /> 
                      </CustomConfig> 
                      </ext:DateField>
                  </Items>
                 </ext:CompositeField>
                 <%--Product--%>  
                 <%--Indra 20181011FM--%>
                 <%--<ext:ComboBox ID="cbItems" runat="server" FieldLabel="Produk" ValueField="c_iteno"--%>
                 <ext:ComboBox ID="cbItems" runat="server" FieldLabel="Nama Produk" ValueField="c_iteno"
                  DisplayField="v_itnam" Width="350" ListWidth="500" 
                  PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
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
                        <ext:Parameter Name="model" Value="2141" />
                        <ext:Parameter Name="parameters" Value="[['@in.c_kddivams', paramValueMultiGetter(#{cbDivAms}), 'System.String[]'],
                          ['@in.c_nosup', paramValueMultiGetter(#{cbSuplier}), 'System.String[]'],
                          ['@in.c_kddivpri', paramValueMultiGetter(#{cbDivPrinsipal}), 'System.String[]'],
                          ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItems}), '']]" Mode="Raw" />
                        <ext:Parameter Name="sort" Value="v_itnam" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_iteno" Root="d.records" SuccessProperty="d.success"
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_iteno" />
                            <ext:RecordField Name="c_itenopri" />
                            <ext:RecordField Name="v_itnam" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Template ID="Template4" runat="server">
                    <Html>
                    <table cellpading="0" cellspacing="0" style="width: 500px">
                    <tr><td class="body-panel">Kode</td><td class="body-panel">Kode Pemasok</td><td class="body-panel">Nama</td></tr>
                    <tpl for=".">
                      <tr class="search-item">
                        <td>{c_iteno}</td><td>{c_itenopri}</td><td>{v_itnam}</td>
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
                    <%--<Change Handler="reloadFilterGrid(#{gridMain}),reloadFilterGrid(#{gridMainDC}) ;" />--%>
                    <Change Handler="reloadFilterGrid(#{GridPanel5});" />
                    <Select Handler="this.triggers[0].show();" />
                    <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                    <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                  </Listeners>
                </ext:ComboBox>                           
                 <%--Type Stock--%>                
                 <%--Batch--%>    
                 <%--Indra 20181011FM--%>
                 <%--<ext:ComboBox ID="cbBatDtl" runat="server" FieldLabel="Batch" ValueField="c_iteno"--%>
                 <%--<ext:ComboBox ID="cbBatDtl" runat="server" FieldLabel="No. Batch" ValueField="c_iteno"
                    DisplayField="v_itnam" Width="350" ListWidth="500" 
                    PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">--%>
                  <ext:ComboBox ID="cbBatDtl" runat="server" FieldLabel="No. Batch" ValueField="c_iteno"
                    DisplayField="v_itnam" Width="350" ListWidth="500" 
                    PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false" Hidden="true">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
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
                        <ext:Parameter Name="model" Value="2111" />
                        <ext:Parameter Name="parameters" Value="[
                                                                       ['@contains.c_batch.Contains(@0) || @contains.c_iteno.Contains(@0)', paramTextGetter(#{cbBatDtl}), '']]"
                                Mode="Raw" />
                        <ext:Parameter Name="sort" Value="c_batch" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_batch" Root="d.records" SuccessProperty="d.success"
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_batch" />
                            <ext:RecordField Name="c_iteno" />
                            <ext:RecordField Name="d_expired" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Template ID="Template1" runat="server">
                    <Html>
                    <table cellpading="0" cellspacing="0" style="width: 500px">
                    <tr><td class="body-panel">Kode batch</td>
                    <tpl for=".">
                      <tr class="search-item">
                        <td>{c_batch}</td>
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
                    <%--<Change Handler="reloadFilterGrid(#{gridMain}),reloadFilterGrid(#{gridMainDC}) ;" />--%>
                    <Select Handler="this.triggers[0].show();" />
                    <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                    <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                  </Listeners>
                 </ext:ComboBox>
                 <%--<ext:SelectBox ID="SelectBoxTipeServiceLevel" runat="server" FieldLabel="Jenis Laporan" SelectedIndex="0" AllowBlank="false" Width="350"   >--%>
                 <ext:SelectBox ID="SelectBoxTipeServiceLevel" runat="server" FieldLabel="Jenis Laporan" SelectedIndex="0" AllowBlank="false" Width="350" Hidden="true" >
                  <Items>  
                      <ext:ListItem Value="1" Text="Recall Header"  />
                      <ext:ListItem Value="2" Text="Recall Detail"/>                      
                  </Items>
                  <Listeners>                     
                    <%--<Change Handler="reloadFilterGrid(#{gridMain}),reloadFilterGrid(#{gridMainDC}), validateRadio(#{SelectBoxTipeServiceLevel}.getValue(), #{gridMain},  #{gridMainDC}, #{TabPanelRecallHeader}, #{TabPanelRecallDetail} );  " />                    --%>
                  </Listeners>                                     
                 </ext:SelectBox>
                 <%--Type Stock END--%>
                  <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="true" hidden="true" />   
                  <ext:TextField ID="txBatch" runat="server" FieldLabel="Batch" Width="600" ReadOnly="true" Icon="TableAdd">
                    <Listeners>                        
                        <IconClick Handler="commandGridFunction(#{winPrintData});" />
                    </Listeners> 
                  </ext:TextField>
              </Items>           
              <%--Proses--%>              
              <Buttons>
                <%--<ext:Button ID="Button1" runat="server" Text="Proses" Icon="CogStart">
                  <DirectEvents>
                    <Click onevent="report_ongenerate">
                      <eventmask showmask="true" />
                      <ExtraParams>                      
                      <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                      <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                      <ext:Parameter Name="Proses" Value="1" Mode="Raw" />                            
                      </ExtraParams>                          
                    </Click>                        
                  </DirectEvents>                      
                </ext:Button>--%>
                <%--<ext:Button ID="Button3" runat="server" Text="Segarkan" Icon="ArrowRefresh">--%>
                <ext:Button ID="Button3" runat="server" Text="Tampilkan Data" Icon="ArrowRefresh">
                  <Listeners>
                    <%--Indra 20181011FM--%> 
                    <%--<Click Handler="refreshGrid(#{gridMain}),refreshGrid(#{gridMainDC});" />--%>
                    <Click Handler="refreshGrid(#{gridMain});" />
                  </Listeners>
                </ext:Button>
              </Buttons>
            </ext:FormPanel>
          </North>
          <Center>
            <ext:Panel ID="Panel1" runat="server" Layout="FitLayout" Frame="True" Border="True" >
              <Items>              
                <ext:TabPanel ID="TabPanelRecallHeader" runat="server" >
                  <Items>              
                    <ext:Panel ID="pnlGridRecallHeader" runat="server" Title="Recall Header" Layout="FitLayout">                 
                      <Items>
                        <ext:GridPanel ID="gridMain"  runat="server"> 
                          <LoadMask ShowMask="true" />
                          <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="Store5" runat="server" RemoteSort="true">
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
                                <ext:Parameter Name="model" Value="10070" />
                                <%--<ext:Parameter Name="parameters" Value="[
                                  ['txPeriode1', paramRawValueGetter(#{txPeriode1}) , 'System.DateTime'],
                                  ['txPeriode2', paramRawValueGetter(#{txPeriode2}) , 'System.DateTime'],
                                  ['txPeriode3', paramRawValueGetter(#{txPeriode3}) , 'System.DateTime'],
                                  ['itemCode', paramValueGetter(#{cbItems}), ''],
                                  ['Batch', paramValueGetter(#{cbBatDtl}), ''],
                                  
                                  ['TypeService', paramValueGetter(#{SelectBoxTipeServiceLevel}), '']
                                                      
                                  ]" Mode="Raw" />--%>
                                 <ext:Parameter Name="parameters" Value="[
                                  ['txPeriode1', paramRawValueGetter(#{txPeriode1}) , 'System.DateTime'],
                                  ['txPeriode2', paramRawValueGetter(#{txPeriode2}) , 'System.DateTime'],
                                  ['txPeriode3', paramRawValueGetter(#{txPeriode3}) , 'System.DateTime'],
                                  ['itemCode', paramValueGetter(#{cbItems}), ''],
                                  ['Batch', paramValueGetter(#{txBatch}), '']                                                      
                                  ]" Mode="Raw" />
                                  
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                  <Fields>
                                    
                                             
                                  
                                  <%-- <ext:RecordField Name="gudang_asal"/>
                                   <ext:RecordField Name="tujuan"/>
                                   <ext:RecordField Name="c_cusno"/>
                                   <ext:RecordField Name="Tgl_do_sj" Type="Date" DateFormat="M$" />
                                   <ext:RecordField Name="Kode_Barang"/>
                                   <ext:RecordField Name="Nama_barang"/>
                                   <ext:RecordField Name="n_qty_do_sj"/>
                                   <ext:RecordField Name="Kode_Supplier"/>
                                   <ext:RecordField Name="Nama_Supplier"/>
                                   <ext:RecordField Name="c_batch"/>
                                   <ext:RecordField Name="n_qty_rc_sj"/>
                                   <ext:RecordField Name="perhitungan"/>
                                   <ext:RecordField Name="kembali"/>--%>                                  
                                   
                                   <ext:RecordField Name="CABANG_DC"/>
                                   <ext:RecordField Name="SOH_GOOD"/>
                                   <ext:RecordField Name="SOH_BAD"/>
                                   <ext:RecordField Name="DISTIRBUSI"/>
                                   <ext:RecordField Name="RECALLN_QTY"/>
                                   <ext:RecordField Name="SUPPLIER"/>
                                   <ext:RecordField Name="PEMBELIAN"/>
                                    
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                              <%--<SortInfo Field="c_cusno" Direction="ASC" />--%>
                            </ext:Store>
                            
                          </Store>
                          <ColumnModel>
                            <Columns>
                             <ext:CommandColumn Width="25">                              
                                  </ext:CommandColumn>
                                  <%--<ext:Column ColumnID="gudang_asal" DataIndex="gudang_asal" Header="Gudang Asal" Width="85" />--%>
                                  
                                  <%--<ext:Column ColumnID="tujuan" DataIndex="tujuan" Header="Tujuan" Width="100"  />
                                  <ext:DateColumn ColumnID="Tgl_do_sj" DataIndex="Tgl_do_sj" Header="Tanggal" Format="dd-MM-yyyy" />
                                  <ext:Column ColumnID="Kode_Barang" DataIndex="Kode_Barang" Header="Kode Barang" Width="85" />
                                  <ext:Column ColumnID="Nama_barang" DataIndex="Nama_barang" Header="Nama Barang" Width="85" /> --%>
                                  
                                  <%--<ext:Column ColumnID="n_qty_do_sj" DataIndex="n_qty_do_sj" Header="Qty DO/SJ" Width="85" />--%>                                   
                                  <%--<ext:Column ColumnID="Kode_Supplier" DataIndex="Kode_Supplier" Header="Kode Supplier" Width="85" />
                                  <ext:Column ColumnID="Nama_Supplier" DataIndex="Nama_Supplier" Header="Nama Supplier" Width="85" />--%>
                                  
                                  <%--<ext:Column ColumnID="c_batch" DataIndex="c_batch" Header="Batch" Width="85" /> --%>
                                  
                                  <%--<ext:Column ColumnID="n_qty_rc_sj" DataIndex="n_qty_rc_sj" Header="Qty RC/SJ" Width="85" />--%>
                                  
                                  <%--<ext:Column ColumnID="perhitungan" DataIndex="perhitungan" Header="Distribusi" Width="85" />
                                  <ext:Column ColumnID="kembali" DataIndex="kembali" Header="Kembali" Width="85" />--%>
                                  
                                  <ext:Column ColumnID="CABANG_DC" DataIndex="CABANG_DC" Header="Cabang/DC" Width="100" />
                                  <ext:Column ColumnID="SOH_GOOD" DataIndex="SOH_GOOD" Header="SOH DC Good" Width="100" />
                                  <ext:Column ColumnID="SOH_BAD" DataIndex="SOH_BAD" Header="SOH DC Bad" Width="100" />
                                  <ext:Column ColumnID="DISTIRBUSI" DataIndex="DISTIRBUSI" Header="Jumlah Distribusi" Width="100" />
                                  <ext:Column ColumnID="RECALLN_QTY" DataIndex="RECALLN_QTY" Header="Jumlah Kembali" Width="100" />
                                  <ext:Column ColumnID="SUPPLIER" DataIndex="SUPPLIER" Header="Jumlah Retur Supplier" Width="150" />
                                  <ext:Column ColumnID="PEMBELIAN" DataIndex="PEMBELIAN" Header="Jumlah Pembelian ke Principals" Width="200" />
                                                                    
                            </Columns>
                          </ColumnModel>                          
                          <TopBar>                                                    
                              <ext:Toolbar ID="Toolbar2" runat="server" StyleSpec="color:white;">
                                <Items>                           
                                  <ext:Button ID="Button2" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel"  hidden ="true">
                                    <Listeners>
                                        <Click Handler="saveData(#{hfGridData},#{gridMain});" />
                                    </Listeners>
                                  </ext:Button>                                
                                  <%--Catak--%>
                                  <ext:button id="btnprint" runat="server" icon="printer" text="Cetak ke Excel">
                                    <directevents>
                                        <click onevent="report_ongenerate" >
                                          <eventmask showmask="true" />
                                          <extraparams>                                          
                                              <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                                              <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                                              <ext:Parameter Name="Batch" Value="#{cbBatDtl}.getValue()" Mode="Raw" />
                                              <ext:Parameter Name="TypeService" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />                                             
                                          </extraparams>
                                        </click>
                                    </directevents>
                                  </ext:button>
                                  <ext:ToolbarSeparator />
                                  <ext:Label ID="lbTipeLaporan" runat="server" FieldLabel="Tipe Laporan" LabelAlign="Right" ></ext:Label>
                                  <ext:SelectBox ID="sbTipeLaporan" runat="server" Width="110">
                                    <Items>
                                      <ext:ListItem Text="&nbsp;" Value="" />
                                      <ext:ListItem Text="Per Batch" Value="1" />                                     
                                      <ext:ListItem Text="Per Grouping Batch" Value="2" />
                                    </Items>
                                  </ext:SelectBox>
                                  <ext:ToolbarSeparator />
                                  <ext:Label ID="lbFormatLaporan" runat="server" FieldLabel="Format Laporan" LabelAlign="Right" ></ext:Label>
                                  <ext:SelectBox ID="sbFormatLaporan" runat="server" Width="110">
                                    <Items>
                                      <ext:ListItem Text="&nbsp;" Value="" />                                     
                                      <ext:ListItem Text="Excel Data Only" Value="02" />
                                      <ext:ListItem Text="Excel" Value="03" />
                                    </Items>
                                  </ext:SelectBox>
                                 </Items>
                              </ext:Toolbar>
                           </TopBar>
                          <BottomBar>
                            <%--<ext:PagingToolbar ID="gmPagingBB" runat="server" PageSize="50">--%>
                            <ext:PagingToolbar ID="gmPagingBB" runat="server" PageSize="50" Hidden="true">
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
                                  <%--<ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">--%>
                                  <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter" Hidden ="true">
                                    <Listeners>
                                                                                                       
                                      <%--<Click Handler="clearFilterGridHeader(#{gridMain}, 
                                                                           #{txNO_SP_Fltr},
                                                                           #{txNO_SP_Fltr_DC},
                                                                           #{txNO_SP_Fltr_Menit},
                                                                           #{txNO_SP_Fltr_Menit_Detail}    
                                                                            );reloadFilterGrid(#{gridMain});
                                                                              reloadFilterGrid(#{gridMainDC});"                                                                                                                                                                                      
                                        Buffer="300" Delay="300" />--%>
                                        <Click Handler="clearFilterGridHeader(#{gridMain}, 
                                                                           #{txNO_SP_Fltr},
                                                                           #{txNO_SP_Fltr_DC},
                                                                           #{txNO_SP_Fltr_Menit},
                                                                           #{txNO_SP_Fltr_Menit_Detail}    
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
                                <ext:HeaderColumn />
                                <ext:HeaderColumn />
                                <ext:HeaderColumn />
                                <ext:HeaderColumn />
                                <ext:HeaderColumn />
                                <ext:HeaderColumn />
                                <ext:HeaderColumn />
                                <ext:HeaderColumn />
                                <ext:HeaderColumn />
                               <%-- <ext:HeaderColumn>
                                      <Component>
                                        <ext:TextField ID="txNO_SP_Fltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                          <Listeners>
                                            <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                                          </Listeners>
                                        </ext:TextField>
                                      </Component>
                                </ext:HeaderColumn>--%>
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
                <ext:TabPanel ID="TabPanelRecallDetail" runat="server" Height="255" Hidden ="true" >
              <Items>
              
                <ext:Panel ID="pnlGridRecallDeatil" runat="server" Title=" Recall Detail" Layout="FitLayout" >
                
                  <Items>
                    <ext:GridPanel ID="gridMainDC"  runat="server"> 
                      <LoadMask ShowMask="true" />
                      <SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                      </SelectionModel>
                      <Store>
                        <ext:Store ID="Store1" runat="server" RemoteSort="true">
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
                            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB_DC}.getValue())" Mode="Raw" />
                            <ext:Parameter Name="model" Value="10070" />
                             <ext:Parameter Name="parameters" Value="[
                              ['txPeriode1', paramRawValueGetter(#{txPeriode1}) , 'System.DateTime'],
                              ['txPeriode2', paramRawValueGetter(#{txPeriode2}) , 'System.DateTime'],
                              ['itemCode', paramValueGetter(#{cbItems}), ''],
                              ['Batch', paramValueGetter(#{cbBatDtl}), ''],
                              ['TypeService', paramValueGetter(#{SelectBoxTipeServiceLevel}), '']
                                                  
                              ]" Mode="Raw" />
                              
                              
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                              <Fields>
                                           
                               
                               <ext:RecordField Name="gudang_asal"/>
                               <ext:RecordField Name="tujuan"/>
                               <ext:RecordField Name="c_cusno"/>
                               <ext:RecordField Name="Nomor_do_sj"/>
                               <ext:RecordField Name="Tgl_do_sj" Type="Date" DateFormat="M$" />
                               <ext:RecordField Name="Kode_Barang"/>                          
                               <ext:RecordField Name="Nama_barang"/>
                               
                               <ext:RecordField Name="n_qty_do_sj"/>
                               <ext:RecordField Name="v_undes"/>
                               <ext:RecordField Name="Kode_Supplier"/>
                               <ext:RecordField Name="Nama_Supplier"/>
                                                          
                               <ext:RecordField Name="c_batch"/>
                               <ext:RecordField Name="Nomor_rc_sj"/>
                               <ext:RecordField Name="Tgl_rc_sj"/>
                               
                               <ext:RecordField Name="Nomor_do_rn"/>
                               
                               
                                <ext:RecordField Name="n_qty_rc_sj"/>
                               <ext:RecordField Name="perhitungan"/>
                               <ext:RecordField Name="c_rcno_all"/>
                               <ext:RecordField Name="d_rcdate_all"/>
                               <ext:RecordField Name="kembali"/>
                                
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                          <SortInfo Field="gudang_asal" Direction="ASC" />
                        </ext:Store>
                        
                      </Store>
                      <ColumnModel>
                        <Columns>
                          <ext:CommandColumn Width="25">
                          
                            
                          </ext:CommandColumn>
                          
                              <ext:Column ColumnID="gudang_asal" DataIndex="gudang_asal" Header="Gudang Asal" Width="85" />
                              <ext:Column ColumnID="tujuan" DataIndex="tujuan" Header="Tujuan" Width="100"  />
                                                           
                              <ext:Column ColumnID="Nomor_do_sj" DataIndex="Nomor_do_sj" Header="Nomor DO/SJ" Width="85" />
                              <ext:DateColumn ColumnID="Tgl_do_sj" DataIndex="Tgl_do_sj" Header="Tanggal" Format="dd-MM-yyyy" />
                              <ext:Column ColumnID="Kode_Barang" DataIndex="Kode_Barang" Header="Kode Barang" Width="100"  />
                              <ext:Column ColumnID="Nama_barang" DataIndex="Nama_barang" Header="Nama Barang" Width="85" />
                              
                                 
                               <ext:Column ColumnID="n_qty_do_sj" DataIndex="n_qty_do_sj" Header="Qty DO/SJ" Width="85" />
                               <ext:Column ColumnID="n_sisa_do_sj" DataIndex="n_sisa_do_sj" Header="Sisa DO/SJ" Width="85" />
                               <ext:Column ColumnID="v_undes" DataIndex="v_undes" Header="Kemasan" Width="85" />
                               <ext:Column ColumnID="Kode_Supplier" DataIndex="Kode_Supplier" Header="Kode Supplier" Width="85" />
                               <ext:Column ColumnID="Nama_Supplier" DataIndex="Nama_Supplier" Header="Nama Supplier" Width="85" />
                              
                  
                                <ext:Column ColumnID="c_batch" DataIndex="c_batch" Header="Batch" Width="85" />
                                <ext:Column ColumnID="Nomor_rc_sj" DataIndex="Nomor_rc_sj" Header="Nomor RC/SJ" Width="85" />
                                <ext:DateColumn ColumnID="Tgl_rc_sj" DataIndex="Tgl_rc_sj" Header="Tgl RC/SJ" Format="dd-MM-yyyy" />
                                <ext:Column ColumnID="Nomor_do_rn" DataIndex="Nomor_do_rn" Header="Nomor DO/RN" Width="85" />
                               
                                                            
                                
                                <ext:Column ColumnID="n_sisa_do_rn" DataIndex="n_sisa_do_rn" Header="Sisa DO/RN" Width="85" />
                                
                                <ext:Column ColumnID="perhitungan" DataIndex="perhitungan" Header="Distribusi" Width="85" />
                                <ext:Column ColumnID="c_rcno_all" DataIndex="c_rcno_all" Header="No RC All" Width="85" />
                                <ext:Column ColumnID="d_rcdate_all" DataIndex="d_rcdate_all" Header="tgl RC all" Width="85" />
                                <ext:Column ColumnID="n_qty_rc_sj" DataIndex="n_qty_rc_sj" Header="Kembali" Width="85" />
                              
                               
                            
                             
                        </Columns>
                      </ColumnModel>
                      
                       <TopBar>
                          <ext:Toolbar ID="Toolbar1" runat="server">
                        <Items>                           
                            <ext:Button ID="Button4" runat="server" Text="To Excel" AutoPostBack="true" OnClick="ToExcel" Icon="PageExcel"  hidden ="true">
                                <Listeners>
                                    <Click Handler="saveData(#{hfGridData},#{gridMainDC});" />
                                </Listeners>
                            </ext:Button>
                            
                          <%--Catak--%>
                              <ext:ToolbarSeparator />
                              <ext:button id="btnprintdc" runat="server" icon="printer" text="Cetak ke Excel">
                                  <directevents>
                                    <click onevent="report_ongenerate">
                                      <eventmask showmask="true" />
                                      <extraparams>
                                      
                                          <ext:Parameter Name="txPeriode1" Value="#{txPeriode1}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="txPeriode2" Value="#{txPeriode2}.getValue()" Mode="Raw" />
                                          <ext:Parameter Name="TypeService" Value="#{SelectBoxTipeServiceLevel}.getValue()" Mode="Raw" />
                                         
                                        
                                      </extraparams>
                                    </click>
                                  </directevents>
                                </ext:button>
                      
                        
                             </Items>
                    </ext:Toolbar>
                       </TopBar>
                       
                       
                      <BottomBar>
                         
                         
                        <ext:PagingToolbar ID="gmPagingBB_DC" runat="server" PageSize="50">
                          <Items>
                            <ext:Label ID="Label2" runat="server" Text="Page size:" />
                            <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
                            <ext:ComboBox ID="cbGmPagingBB_DC" runat="server" Width="80">
                              <Items>
                                <ext:ListItem Text="5" />
                                <ext:ListItem Text="10" />
                                <ext:ListItem Text="20" />
                                <ext:ListItem Text="50" />
                                <ext:ListItem Text="100" />
                              </Items>
                              <SelectedItem Value="50" />
                              <Listeners>
                                <Select Handler="#{gmPagingBB_DC}.pageSize = parseInt(this.getValue()); #{gmPagingBB_DC}.doLoad();" />
                              </Listeners>
                            </ext:ComboBox>
                          </Items>
                        </ext:PagingToolbar>
                    
                    
                  </BottomBar>
                  
                  
                        <View>
                    <ext:GridView ID="gvFilter_DC" runat="server" StandardHeaderRow="true">
                      <HeaderRows>
                      
                        <ext:HeaderRow>
                          <Columns>
                          
                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                        <Component>
                          <ext:Button ID="ClearFilterButton_DC" runat="server" Icon="Cancel" ToolTip="Clear filter">
                            <Listeners>
                                                        
                                
                                <Click Handler="clearFilterGridHeader(#{gridMain}, 
                                                                   #{txNO_SP_Fltr},
                                                                   #{txNO_SP_Fltr_DC},
                                                                   #{txNO_SP_Fltr_Menit},
                                                                   #{txNO_SP_Fltr_Menit_Detail}    
                                                                    );reloadFilterGrid(#{gridMain});
                                                                      reloadFilterGrid(#{gridMainDC});"
                                                                           
                                
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                     
                      
                        </ext:HeaderColumn>
                      <%--  <ext:HeaderColumn>
                              <Component>
                                <ext:TextField ID="txNO_SP_Fltr_DC" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                  <Listeners>
                                    <KeyUp Handler="reloadFilterGrid(#{gridMainDC})" Buffer="700" Delay="700" />
                                  </Listeners>
                                </ext:TextField>
                              </Component>
                        </ext:HeaderColumn>--%>
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
              </Items>
            </ext:Panel>
          </Center>
        </ext:BorderLayout>
      </Items>
    </ext:Panel>
  </Items>
</ext:Viewport>

<ext:Window ID="wndDown" runat="server" Hidden="true" />

<ext:Window ID="winPrintData" runat="server" Width="580" Height="380" Hidden="true" Title="Daftar No. Batch"
  MinWidth="580" MinHeight="380" Layout="FitLayout" Maximizable="false" Resizable="false">
  <Content>
    <ext:Hidden ID="hidItemSp" runat="server" />
  </Content>
  <Items>
    <ext:FormPanel ID="frmReportKriteria" runat="server" Padding="5" Frame="True" Layout="Form">
      <Items>
        <ext:Checkbox ID="chkBatch" runat="server" FieldLabel="Check/Uncheck" >
            <ToolTips>
             <ext:ToolTip ID="TTPrint" runat="server" Html="Check/Uncheck Cabang">
             </ext:ToolTip>
            </ToolTips>
            <Listeners>
             <Check Handler="setGetBatch(this, #{GridPanel5}.getStore(),#{chkBatch});" />
            </Listeners>
        </ext:Checkbox>
        <ext:GridPanel ID="GridPanel5" runat="server" Cls="mygrid" Height="270" EnableColumnHide="false" MaskDisabled="true" >
        <LoadMask ShowMask="false" />
        <SelectionModel>
            <ext:RowSelectionModel SingleSelect="true" />
        </SelectionModel>
        <Store>
            <ext:Store ID="store12" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
                <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                        CallbackParam="soaScmsCallback" />
                </Proxy>
                <AutoLoadParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="={10}" />
                </AutoLoadParams>
                <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="parseInt(-1)" Mode="Raw" />
                    <ext:Parameter Name="model" Value="2194" />
                    <ext:Parameter Name="parameters" Value="[['itemCode', paramValueGetter(#{cbItems}), '']]" Mode="Raw" />
                </BaseParams>                                           
                <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                        <Fields>
                            <ext:RecordField Name="KOLOM_1" />
                            <ext:RecordField Name="KOLOM1" />                                                                                                                
                            <ext:RecordField Name="KOLOM_2" />
                            <ext:RecordField Name="KOLOM2" />                                                        
                            <ext:RecordField Name="KOLOM_3" />
                            <ext:RecordField Name="KOLOM3" />                                                        
                            <ext:RecordField Name="KOLOM_4" />                                        
                            <ext:RecordField Name="KOLOM4" />                                                        
                        </Fields>
                    </ext:JsonReader>
                </Reader>
                <%--<SortInfo Field="Urut" Direction="ASC" />--%>
            </ext:Store>
        </Store>
        <ColumnModel>
            <Columns>  
                <ext:CheckColumn DataIndex="KOLOM_1" Width="30" Editable="true" Sortable="false"/>
                <ext:Column DataIndex="KOLOM1" Header="" Width="100" Editable="false" Sortable="false"/>
                <ext:CheckColumn DataIndex="KOLOM_2" Width="30" Editable="true" Sortable="false"/> 
                <ext:Column DataIndex="KOLOM2" Header="" Width="100" Editable="false" Sortable="false"/>
                <ext:CheckColumn DataIndex="KOLOM_3" Width="30" Editable="true" Sortable="false"/> 
                <ext:Column DataIndex="KOLOM3" Header="" Width="100" Editable="false" Sortable="false"/>
                <ext:CheckColumn DataIndex="KOLOM_4" Width="30" Editable="true" Sortable="false"/> 
                <ext:Column DataIndex="KOLOM4" Header="" Width="100" Editable="false" Sortable="false"/>
            </Columns>
        </ColumnModel>
        <Listeners>
          <Click Handler="cekPilihDriver(this, #{txNoPol});" />
        </Listeners>
        <View>
            <ext:GridView ID="GridView5" runat="server" StandardHeaderRow="true">                                       
            </ext:GridView>
        </View>
        </ext:GridPanel>                                  
        <ext:Button id="btnPilihBatch" runat="server" icon="BookAdd" Text="Pilih batch">
        <Listeners>
            <Click Handler="storeToDetailGrid(#{GridPanel5}, #{txBatch}, #{chkBatch});" />
        </Listeners>
        </ext:Button>
      </Items>      
    </ext:FormPanel>
  </Items>
</ext:Window>