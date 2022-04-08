<%--Created By Indra Monitoring Process 20180523FM--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MonitoringPL.aspx.cs" Inherits="transaksi_wp_MonitoringPL"
    MasterPageFile="~/Master.master" %>
 
<%--<%@ Register Src="MonitoringPLSummary.ascx" TagName="MonitoringPLSummary" TagPrefix="uc" %>--%>
<%@ Register Src="MonitoringPLEkspedisi.ascx" TagName="MonitoringPLEkspedisi" TagPrefix="uc" %>
<%@ Register Src="MonitoringPLGridDtlPL.ascx" TagName="MonitoringPLGridDtlPL" TagPrefix="uc" %>
<%@ Register Src="PackingListCtrl.ascx" TagName="PackingListCtrl" TagPrefix="uc" %>
<%@ Register Src="Legend.ascx" TagName="Legend" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

<script type="text/javascript" language="javascript">

    var getRowClass = function(record) {
        var cStatus = record.get('Tipe');

        if (cStatus == '0') {
            return "grey";
        }
        
        if (cStatus == '1') {
            return "red";
        }

        if (cStatus == '2') {
            return "orange";
        }
        
        if (cStatus == '3') {
            return "green";
        }

        if (cStatus == '4') {
            return "magenta";
        }
    }

    var validateRadio = function(rd01, rd02, gdg, tgl, chkcabang, grid) {
        if (rd01) {
            gdg.setDisabled(false);
            tgl.setDisabled(false);
            chkcabang.setDisabled(false);
            grid.setDisabled(false);

        } else {
            gdg.setDisabled(true);
            tgl.setDisabled(true);
            chkcabang.setDisabled(true);
            grid.setDisabled(true);
        
        };
    };


    var setPrint = function(chkDefault, store, chkDeclined) {
        var chk = chkDefault.getValue();

        if (chk) {
            store.each(function(e) {
                if (e.get('AF') != '-') {
                    e.set('A_F', true);
                }
                
                if (e.get('GL') != '-') {
                    e.set('G_L', true);
                }
                
                if (e.get('MR') != '-') {
                    e.set('M_R', true);
                }
                
                if (e.get('SZ') != '-') {
                    e.set('S_Z', true);
                }

            });
        }
        else {
            store.each(function(e) {
                e.set('A_F', false);
                e.set('G_L', false);
                e.set('M_R', false);
                e.set('S_Z', false);
            });
        }
    }

    
    
</script>

<style type="text/css">
    .grey {
        background: #a3c2c2;
        color : #191970; 
    }
    
    .red {
        background: #F9A2A4;
        color : #191970; 
    }
    
    .orange {
        background: #FFCC66;  
        color : #191970; 
    } 
    
    .magenta { 
        background: #E4E4E4; 
        color: #990000; 
    }  
     
    .green{ 
        background: #AAF07B;	         	        
        color : #191970;         
    }
            
    .mygrid .x-grid3-row td, .mygrid .x-grid3-summary-row td {
        font-weight:bold;
    }
    
    .keterangan{
    	color : #191970;
    }
    
</style>    
       
<ext:Container Layout="FitLayout" runat="server" StyleSpec="font-weight:bold;font-size:150%;color:#000033;text-align:right;padding-right:9pt;" > 
    <Content>            
        <ext:Label ID="LocalTimeLabel" runat="server" />                  
    </Content>        
</ext:Container>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
    <ext:Hidden ID="hfMode" runat="server" />
    <ext:Hidden ID="hfType" runat="server" />
    <ext:Hidden ID="hidWndDown" runat="server" />    
    <ext:Container runat="server" StyleSpec="padding-left:9pt;" Layout="ColumnLayout" >
      <Content>
        <ext:ComboBox ID="cbPosisiStok" runat="server" FieldLabel="Posisi Gudang" ValueField="c_gdg" 
        DisplayField="v_gdgdesc" Width="250" AllowBlank="true" ForceSelection="false" EmptyText="Pilihan..." >
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
            <ext:Parameter Name="model" Value="2031" />
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
        <Change Handler="reloadFilterGrid(#{gridMain});reloadFilterGrid(#{GridPanel1});reloadFilterGrid(#{GridPanel3});" />
      </Listeners>
    </ext:ComboBox>
        <ext:ToolbarSeparator />
        <ext:ComboBox ID="cbItems" runat="server" ValueField="c_iteno" EmptyText="Pilih Item" FieldLabel="  " LabelWidth="20"
          DisplayField="v_itnam" Width="300" ListWidth="300" 
          PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
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
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2141" />
                <ext:Parameter Name="parameters" Value="[['@contains.v_itnam.Contains(@0) || @contains.c_iteno.Contains(@0)', paramTextGetter(#{cbItems}), '']]" Mode="Raw" />
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
          <Template ID="Template5" runat="server">
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
            <Select Handler="this.triggers[0].show();" />
            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
            <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); }" />
            <Change Handler="reloadFilterGrid(#{gridMain});reloadFilterGrid(#{GridPanel1});reloadFilterGrid(#{GridPanel3});" />
          </Listeners>
        </ext:ComboBox>
        <%--<ext:Label ID="lbStartStopServerTime" runat="server" Text="" Width="10"></ext:Label>
        <ext:Button ID="StopServerTime" runat="server" Text="" Icon="StopBlue" ToolTip="To Stop Refresh Grid">
            <Listeners>
                <Click Handler="#{taskMgr}.stopTask('servertime');#{lblStatus}.setText('on Stop');" />
            </Listeners>
        </ext:Button>
        <ext:ToolbarSeparator />
        <ext:Button ID="StartServerTime" runat="server" Text="" Icon="PlayBlue" ToolTip="To Run Refresh Grid">
          <Listeners>
              <Click Handler="#{taskMgr}.startTask('servertime');#{lblStatus}.setText('on Start');" />
          </Listeners>
        </ext:Button>       
        <ext:ToolbarSeparator />        
        <ext:Label ID="lbRefreshGrid" runat="server" Text="" Width="10"></ext:Label>
        <ext:Label Text="Refresh Grid - " runat="server" ID="RefreshGrid"></ext:Label>
        <ext:Label Text="on Start" runat="server" ID="lblStatus"></ext:Label>   --%>
      </Content>        
    </ext:Container>

    <ext:FormPanel ID="frmHeaders" runat="server" Title="" Height="530" width="1355"
          Layout="Fit" ButtonAlign="Center" MonitorValid="true" MinHeight="180" MaskDisabled="true">
        
        <Items>        
            <ext:Panel ID="Panel1" Padding="5" Layout="Column" runat="server" Width="841" MaskDisabled="true" >
               <Items>
                    <ext:Panel ID="pnlMainControl" runat="server" Border="false" Header="false" Layout="Form" Width="841" MaskDisabled="true">                             
                        <TopBar>
                            <ext:Toolbar runat="server">                   
                                <Items>                     
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:Container ID="Container1" runat="server" Layout="FitLayout"> 
                            <Content>
                                <ext:GridPanel ID="gridMain" runat="server" Cls="mygrid" Height="540" EnableColumnHide="false" MaskDisabled="true" >
                                    <LoadMask ShowMask="false" />
                                    <DirectEvents>
                                      <Command OnEvent="gridMainCommand" Before="if(command != 'GridDtlPL') { return false; }">
                                        <EventMask ShowMask="false" />
                                        <ExtraParams>
                                          <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                                          <ext:Parameter Name="NOSPSG" Value="record.data.NoSP" Mode="Raw" />
                                          <ext:Parameter Name="NOPL" Value="record.data.NoPL" Mode="Raw" />
                                          <ext:Parameter Name="NOSJ" Value="record.data.NoDO" Mode="Raw" />                                         
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
                                                <ext:Parameter Name="limit" Value="={2000}" />
                                            </AutoLoadParams>
                                            <BaseParams>
                                                <ext:Parameter Name="start" Value="0" />
                                                <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                                                <ext:Parameter Name="model" Value="0380" />
                                                <ext:Parameter Name="parameters" Value="[['Gudang = @0', #{cbPosisiStok}.getValue(), 'System.Char'],
                                                                                         ['DivAms', paramValueGetter(#{cbDivAms}) + '%', 'System.String'],
                                                                                         ['Cabang', paramValueGetter(#{cbCustomerFltr}) + '%', ''],
                                                                                         ['CabNoSP', paramValueGetter(#{txSPCABFltr}) + '%', 'System.String'],
                                                                                         ['NoSP', paramValueGetter(#{txSPHOFltr}) + '%', ''],
                                                                                         ['Status', paramValueGetter(#{sbStatusFltr}) + '%', 'System.String'],
                                                                                         ['ETDFILTER', paramValueGetter(#{sbETDFltr}), 'System.String'],
                                                                                         ['ETAFILTER', paramValueGetter(#{sbETAFltr}), 'System.String'],
                                                                                         ['KodeItem', #{cbItems}.getValue(), 'System.String']]" Mode="Raw" />
                                            </BaseParams>
                                           
                                            
                                            <Reader>
                                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                    <Fields>
                                                        <ext:RecordField Name="DivAms" />
                                                        <ext:RecordField Name="Cabang" />
                                                        <ext:RecordField Name="CabNoSP" />
                                                        <ext:RecordField Name="NoSP" />
                                                        <ext:RecordField Name="NoPL" />
                                                        <ext:RecordField Name="NoDO" />
                                                        <ext:RecordField Name="NoEP" />
                                                        <ext:RecordField Name="Status" />
                                                        <ext:RecordField Name="LastTimeAct" Type="Date" DateFormat="M$"/>                                            
                                                        <ext:RecordField Name="ETD" Type="Date" DateFormat="M$" />
                                                        <ext:RecordField Name="ETA" Type="Date" DateFormat="M$" />
                                                        <ext:RecordField Name="SPTIME" Type="Date" DateFormat="M$" />
                                                        <ext:RecordField Name="Tipe" />
                                                    </Fields>
                                                </ext:JsonReader>
                                            </Reader>
                                            <SortInfo Field="LastTimeAct" Direction="DESC" />
                                        </ext:Store>
                                    </Store>
                                    <ColumnModel>
                                        <Columns>  
                                            <ext:Column ColumnID="DivAms" DataIndex="DivAms" Header="Div. AMS" Width="80" />
                                            <ext:Column ColumnID="Cabang" DataIndex="Cabang" Header="Cabang" Width="60" />
                                            <ext:Column ColumnID="CabNoSP" DataIndex="CabNoSP" Header="No. SP Cabang" Width="120" />
                                            <ext:Column ColumnID="NoSP" DataIndex="NoSP" Header="No. SP HO / SG" Width="90" />
                                            <ext:CommandColumn Width="55" DataIndex="SP/PL" Resizable="false">
                                                <Commands>
                                                    <ext:GridCommand Icon="BookOpen" CommandName="GridDtlPL" ToolTip-Text="Lihat detil Packing List" ToolTip-Title="Command" />
                                                </Commands>
                                            </ext:CommandColumn>
                                            <ext:Column ColumnID="NoPL" DataIndex="NoPL" Header="No. Picking List" Width="90" />                                                                                                                   
                                            <ext:Column ColumnID="NoDO" DataIndex="NoDO" Header="No. DO / SJ" Width="90" />                           
<%--                                            <ext:Column ColumnID="NoEP" DataIndex="NoEP" Header="No. Ekspedisi" Width="100" Hidden="true">
                                            </ext:Column>--%>
                                            <ext:Column ColumnID="Status" DataIndex="Status" Header="Status" Width="110" />  
                                            <ext:DateColumn ColumnID="LastTimeAct" DataIndex="LastTimeAct" Header="Last Time Act." Format ="dd/MM/yyyy H:i" Width="110"/>
                                            <ext:DateColumn ColumnID="SPTIME" DataIndex="SPTIME" Header="SP Received Time" Format ="dd/MM/yyyy H:i" Width="110"/>
                                            <ext:DateColumn ColumnID="ETD" DataIndex="ETD" Header="ETD" Format ="dd/MM/yyyy H:i" Width="110"/>
                                            <ext:DateColumn ColumnID="ETA" DataIndex="ETA" Header="ETA" Format ="dd/MM/yyyy H:i" Width="110"/>
                                            <ext:Column ColumnID="Tipe" DataIndex="Tipe" Header="Tipe" Width="150" Hidden="true" />  
                                        </Columns>
                                    </ColumnModel>
                                    <View>
                                        <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                                            <HeaderRows>
                                                <ext:HeaderRow>
                                                    <Columns>                                                    
                                                        <ext:HeaderColumn >
                                                            <Component>
                                                                <ext:ComboBox ID="cbDivAms" runat="server"  ValueField="v_nmdivams"
                                                                  DisplayField="v_nmdivams" Width="500" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
                                                                  AllowBlank="true" ForceSelection="false">
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
                                                                        <ext:Parameter Name="allQuery" Value="true" />
                                                                        <ext:Parameter Name="model" Value="2041" />
                                                                        <ext:Parameter Name="parameters" Value="[ ['@contains.v_nmdivams.Contains(@0) || @contains.c_kddivams.Contains(@0)', paramTextGetter(#{cbDivAms}), '']]"
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
                                                                  <Change Handler="reloadFilterGrid(#{gridMain});reloadFilterGrid(#{GridPanel3});" Buffer="300" Delay="300" />  
                                                                </Listeners>                              
                                                                </ext:ComboBox> 
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                                            <Component>
                                                                <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="c_cab_dcore" ValueField="c_cab_dcore"
                                                                    Width="60" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
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
                                                                          <ext:Parameter Name="model" Value="2011-b" />
                                                                          <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab_dcore.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
                                                                            Mode="Raw" />
                                                                          <ext:Parameter Name="sort" Value="v_cunam" />
                                                                          <ext:Parameter Name="dir" Value="ASC" />
                                                                        </BaseParams>
                                                                        <Reader>
                                                                          <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                                            TotalProperty="d.totalRows">
                                                                            <Fields>
                                                                              <ext:RecordField Name="c_cusno" />
                                                                              <ext:RecordField Name="c_cab_dcore" />
                                                                              <ext:RecordField Name="v_cunam" />
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
                                                                          <td>{c_cusno}</td><td>{c_cab_dcore}</td><td>{v_cunam}</td>
                                                                      </tr></tpl>
                                                                      </table>
                                                                      </Html>
                                                                    </Template>
                                                                    <Listeners>
                                                                      <Change Handler="reloadFilterGrid(#{gridMain});reloadFilterGrid(#{GridPanel3});" Buffer="300" Delay="300" />
                                                                    </Listeners>
                                                                  </ext:ComboBox>
                                                            </Component>
                                                        </ext:HeaderColumn>                    
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                              <ext:TextField ID="txSPCABFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                <Listeners>
                                                                  <KeyUp Handler="reloadFilterGrid(#{gridMain} )" Buffer="700" Delay="700" />
                                                                </Listeners>
                                                              </ext:TextField>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                              <ext:TextField ID="txSPHOFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                <Listeners>
                                                                  <KeyUp Handler="reloadFilterGrid(#{gridMain} )" Buffer="700" Delay="700" />
                                                                </Listeners>
                                                              </ext:TextField>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                              <ext:SelectBox ID="sbGetData" runat="server" Width="80">
                                                                <Items>
                                                                  <ext:ListItem Text="SP/SG" Value="SP/SG" />
                                                                  <ext:ListItem Text="PL/SJ" Value="PL/SJ" />
                                                                </Items>
                                                              </ext:SelectBox>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn>
                                                            <%--<Component>
                                                              <ext:TextField ID="txDOFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                <Listeners>
                                                                  <KeyUp Handler="reloadFilterGrid(#{gridMain} )" Buffer="700" Delay="700" />
                                                                </Listeners>
                                                              </ext:TextField>
                                                            </Component>--%>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn />
                                                        <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                                            <Component>
                                                              <ext:SelectBox ID="sbStatusFltr" runat="server" Width="110">
                                                                <Items>
                                                                  <ext:ListItem Text="&nbsp;" Value="" />
                                                                  <ext:ListItem Text="SP RECEIVED" Value="SP RECEIVED" />
                                                                  <ext:ListItem Text="PL CREATED" Value="PL CREATED" />
                                                                  <ext:ListItem Text="HANDED OVER" Value="HANDED OVER" />    
                                                                  <ext:ListItem Text="GOODS PICKED" Value="GOODS PICKED" />                                                              
                                                                  <ext:ListItem Text="GOODS CHECKED" Value="GOODS CHECKED" />
                                                                  <ext:ListItem Text="DO CREATED" Value="DO CREATED" />
                                                                  <ext:ListItem Text="GOODS P&P" Value="GOODS P&P" />
                                                                  <ext:ListItem Text="EKSPEDISI" Value="EKSPEDISI" />
                                                                  <ext:ListItem Text="DEPARTED" Value="DEPARTED" />
                                                                  <ext:ListItem Text="ARRIVED" Value="ARRIVED" />
                                                                </Items>
                                                                <Listeners>
                                                                  <Select handler="reloadFilterGrid(#{gridMain})" buffer="100" delay="100" />
                                                                </Listeners>
                                                              </ext:SelectBox>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn />
                                                        <ext:HeaderColumn />
                                                        <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                                            <Component>
                                                              <ext:SelectBox ID="sbETDFltr" runat="server" Width="110">
                                                                <Items>
                                                                  <ext:ListItem Text="&nbsp;" Value="" />
                                                                  <ext:ListItem Text="OVERDUE" Value="O" />
                                                                  <ext:ListItem Text="TODAY" Value="T" />
                                                                  <ext:ListItem Text="NEXT 3 DAYS" Value="3" />    
                                                                  <ext:ListItem Text="NEXT 7 DAYS" Value="7" />                                                              
                                                                </Items>
                                                                <Listeners>
                                                                  <Select handler="reloadFilterGrid(#{gridMain})" buffer="100" delay="100" />
                                                                </Listeners>
                                                              </ext:SelectBox>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                                            <Component>
                                                              <ext:SelectBox ID="sbETAFltr" runat="server" Width="110">
                                                                <Items>
                                                                  <ext:ListItem Text="&nbsp;" Value="" />
                                                                  <ext:ListItem Text="OVERDUE" Value="O" />
                                                                  <ext:ListItem Text="TODAY" Value="T" />
                                                                  <ext:ListItem Text="NEXT 3 DAYS" Value="3" />    
                                                                  <ext:ListItem Text="NEXT 7 DAYS" Value="7" />                                                              
                                                                </Items>
                                                                <Listeners>
                                                                  <Select handler="reloadFilterGrid(#{gridMain})" buffer="100" delay="100" />
                                                                </Listeners>
                                                              </ext:SelectBox>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                    </Columns>
                                                </ext:HeaderRow>
                                            </HeaderRows>
                                            <GetRowClass Fn="getRowClass" />                                                 
                                        </ext:GridView>
                                    </View>
                                    <BottomBar>
                                        <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="50">
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
                                                    <SelectedItem Value="2000" />
                                                    <Listeners>
                                                        <Select Handler="#{gmPagingBB}.pageSize = parseInt(this.getValue()); #{gmPagingBB}.doLoad();" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                            </Items>
                                        </ext:PagingToolbar>
                                    </BottomBar>
                                </ext:GridPanel>
                            </Content>
                            </ext:Container>
                        </Items>
                    </ext:Panel>                   
                    <ext:Panel ID="pnlSecondControl" runat="server" Border="false" Header="false" Layout="Form" ColumnWidth=".5" >
                        <TopBar>
                            <ext:Toolbar ID="Toolbar1" runat="server">                   
                                <Items>                     
                                </Items>
                            </ext:Toolbar>
                        </TopBar>
                        <Items>
                            <ext:Container ID="Container2" runat="server" Layout="FitLayout" > 
                            <Content>
                                <ext:GridPanel ID="GridPanel1" runat="server" Cls="mygrid" Height="145" EnableColumnHide="false" MaskDisabled="true">
                                    <LoadMask ShowMask="false" />
                                    <DirectEvents>
                                      <Command OnEvent="gridMainCommand2" Before="if(command != 'Select') { return false; }">
                                        <EventMask ShowMask="true" />
                                        <ExtraParams>
                                          <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                                          <ext:Parameter Name="Parameter" Value="c_expno" />
                                          <ext:Parameter Name="NOEXP" Value="record.data.c_expno" Mode="Raw" />
                                        </ExtraParams>
                                      </Command>
                                    </DirectEvents>
                                    <SelectionModel>
                                        <ext:RowSelectionModel SingleSelect="true" />
                                    </SelectionModel>
                                    <Store>
                                        <ext:Store ID="store4" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                                                <ext:Parameter Name="limit" Value="parseInt(100)" Mode="Raw" />
                                                <ext:Parameter Name="model" Value="0380-a" />
                                                <ext:Parameter Name="parameters" Value="[['c_gdg = @0', #{cbPosisiStok}.getValue(), 'System.Char'],
                                                                                         ['c_expno', paramValueGetter(#{txEPFltr}) + '%', ''],
                                                                                         ['c_cab_dcore', paramValueGetter(#{cbCustomerEksFltr}) + '%', ''],
                                                                                         ['c_resi', paramValueGetter(#{txResiFltr}) + '%', '']]" Mode="Raw" />
                                            </BaseParams>                                           
                                            <Reader>
                                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                    <Fields>
                                                        <ext:RecordField Name="c_expno" />
                                                        <ext:RecordField Name="c_cab_dcore" />                                                        
                                                        <ext:RecordField Name="c_resi" />
                                                        <ext:RecordField Name="d_resi" Type="Date" DateFormat="M$"/>     
                                                        <ext:RecordField Name="v_ket" />                                                   
                                                    </Fields>
                                                </ext:JsonReader>
                                            </Reader>
                                            <SortInfo Field="c_expno" Direction="DESC" />
                                        </ext:Store>
                                    </Store>
                                    <ColumnModel>
                                        <Columns>  
                                            <ext:CommandColumn Width="30" Resizable="false">
                                                <Commands>
                                                    <ext:GridCommand CommandName="Select" Icon="BookOpen" ToolTip-Title="" ToolTip-Text="Lihat detil Ekspedisi" />
                                                </Commands>
                                            </ext:CommandColumn>
                                            <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="No. Ekspedisi" Width="90" />
                                            <ext:Column ColumnID="c_cab_dcore" DataIndex="c_cab_dcore" Header="Cabang" Width="60" />
                                            <ext:Column ColumnID="c_resi" DataIndex="c_resi" Header="No. Resi" Width="100" />        
                                            <ext:DateColumn ColumnID="d_resi" DataIndex="d_resi" Header="Tgl. Resi" Format ="dd/MM/yyyy" Width="65"/> 
                                            <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Nama. Ekspedisi" Width="130" /> 
                                        </Columns>
                                    </ColumnModel>
                                    <View>
                                        <ext:GridView ID="GridView2" runat="server" StandardHeaderRow="true">
                                            <HeaderRows>
                                                <ext:HeaderRow>
                                                    <Columns>                                                                                                            
                                                        <ext:HeaderColumn />    
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                              <ext:TextField ID="txEPFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                <Listeners>
                                                                  <KeyUp Handler="reloadFilterGrid(#{GridPanel1} )" Buffer="700" Delay="700" />
                                                                </Listeners>
                                                              </ext:TextField>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                                <ext:ComboBox ID="cbCustomerEksFltr" runat="server" DisplayField="c_cab_dcore" ValueField="c_cab_dcore"
                                                                    Width="60" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                                                                    AllowBlank="true" ForceSelection="false">
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
                                                                          <ext:Parameter Name="model" Value="2011-b" />
                                                                          <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab_dcore.Contains(@0)', paramTextGetter(#{cbCustomerEksFltr}), '']]"
                                                                            Mode="Raw" />
                                                                          <ext:Parameter Name="sort" Value="v_cunam" />
                                                                          <ext:Parameter Name="dir" Value="ASC" />
                                                                        </BaseParams>
                                                                        <Reader>
                                                                          <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                                            TotalProperty="d.totalRows">
                                                                            <Fields>
                                                                              <ext:RecordField Name="c_cusno" />
                                                                              <ext:RecordField Name="c_cab_dcore" />
                                                                              <ext:RecordField Name="v_cunam" />
                                                                            </Fields>
                                                                          </ext:JsonReader>
                                                                        </Reader>
                                                                      </ext:Store>
                                                                    </Store>
                                                                    <Template ID="Template2" runat="server">
                                                                      <Html>
                                                                      <table cellpading="0" cellspacing="1" style="width: 400px">
                                                                      <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                                                                      <tpl for="."><tr class="search-item">
                                                                          <td>{c_cusno}</td><td>{c_cab_dcore}</td><td>{v_cunam}</td>
                                                                      </tr></tpl>
                                                                      </table>
                                                                      </Html>
                                                                    </Template>
                                                                    <Listeners>
                                                                      <Change Handler="reloadFilterGrid(#{GridPanel1} )" Buffer="300" Delay="300" />
                                                                    </Listeners>
                                                                  </ext:ComboBox>
                                                            </Component>
                                                        </ext:HeaderColumn>                                     
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                              <ext:TextField ID="txResiFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                <Listeners>
                                                                  <KeyUp Handler="reloadFilterGrid(#{GridPanel1} )" Buffer="700" Delay="700" />
                                                                </Listeners>
                                                              </ext:TextField>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn />                                       
                                                        <ext:HeaderColumn />  
                                                    </Columns>
                                                </ext:HeaderRow>
                                            </HeaderRows>                                                
                                        </ext:GridView>
                                    </View>
                                    <BottomBar>
                                    </BottomBar>
                                </ext:GridPanel>
                            </Content>
                            </ext:Container>
                            <ext:Container ID="Container4" runat="server" Layout="FitLayout" Visible="false" > 
                            <Content>
                                <ext:GridPanel ID="GridPanel4" runat="server" Cls="mygrid" Height="145" EnableColumnHide="false" MaskDisabled="true">
                                    <LoadMask ShowMask="false" />
                                    <DirectEvents>
                                      <Command OnEvent="gridMainCommand" Before="if(command != 'Select') { return false; }">
                                        <EventMask ShowMask="true" />
                                        <ExtraParams>
                                          <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                                          <ext:Parameter Name="Parameter" Value="c_expno" />
                                          <ext:Parameter Name="PrimaryID" Value="record.data.c_expno" Mode="Raw" />
                                          <ext:Parameter Name="PrimaryID2" Value="record.data.c_expno" Mode="Raw" />
                                          <ext:Parameter Name="PrimaryID3" Value="record.data.c_expno" Mode="Raw" />
                                        </ExtraParams>
                                      </Command>
                                    </DirectEvents>
                                    <SelectionModel>
                                        <ext:RowSelectionModel SingleSelect="true" />
                                    </SelectionModel>
                                    <Store>
                                        <ext:Store ID="store8" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                                                <ext:Parameter Name="limit" Value="parseInt(100)" Mode="Raw" />
                                                <ext:Parameter Name="model" Value="0380-a" />
                                                <ext:Parameter Name="parameters" Value="[['c_gdg = @0', #{cbPosisiStok}.getValue(), 'System.Char'],
                                                                                         ['c_expno', paramValueGetter(#{txEPFltr}) + '%', ''],
                                                                                         ['c_cab_dcore', paramValueGetter(#{cbCustomerEksFltr}) + '%', ''],
                                                                                         ['c_resi', paramValueGetter(#{txResiFltr}) + '%', '']]" Mode="Raw" />
                                            </BaseParams>                                           
                                            <Reader>
                                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                    <Fields>
                                                        <ext:RecordField Name="c_expno" />
                                                        <ext:RecordField Name="c_cab_dcore" />                                                        
                                                        <ext:RecordField Name="c_resi" />
                                                        <ext:RecordField Name="d_resi" Type="Date" DateFormat="M$"/>     
                                                        <ext:RecordField Name="v_ket" />                                                   
                                                    </Fields>
                                                </ext:JsonReader>
                                            </Reader>
                                            <SortInfo Field="c_expno" Direction="DESC" />
                                        </ext:Store>
                                    </Store>
                                    <ColumnModel>
                                        <Columns>  
                                            <ext:CommandColumn Width="30" Resizable="false">
                                                <Commands>
                                                    <ext:GridCommand CommandName="Select" Icon="BookOpen" ToolTip-Title="" ToolTip-Text="Lihat detil Ekspedisi" />
                                                </Commands>
                                            </ext:CommandColumn>
                                            <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="No. Pallet" Width="90" />
                                            <ext:Column ColumnID="c_cab_dcore" DataIndex="c_cab_dcore" Header="Cabang" Width="60" />
                                            <ext:Column ColumnID="c_resi" DataIndex="c_resi" Header="No. Resi" Width="100" />        
                                            <ext:DateColumn ColumnID="d_resi" DataIndex="d_resi" Header="Tgl. Resi" Format ="dd/MM/yyyy" Width="65"/> 
                                            <ext:Column ColumnID="v_ket" DataIndex="v_ket" Header="Nama. Ekspedisi" Width="130" /> 
                                        </Columns>
                                    </ColumnModel>
                                    <View>
                                        <ext:GridView ID="GridView4" runat="server" StandardHeaderRow="true">
                                            <HeaderRows>
                                                <ext:HeaderRow>
                                                    <Columns>                                                                                                            
                                                        <ext:HeaderColumn />    
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                              <ext:TextField ID="TextField1" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                <Listeners>
                                                                  <KeyUp Handler="reloadFilterGrid(#{GridPanel1} )" Buffer="700" Delay="700" />
                                                                </Listeners>
                                                              </ext:TextField>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                                <ext:ComboBox ID="ComboBox1" runat="server" DisplayField="c_cab_dcore" ValueField="c_cab_dcore"
                                                                    Width="60" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                                                                    AllowBlank="true" ForceSelection="false">
                                                                    <CustomConfig>
                                                                      <ext:ConfigItem Name="allowBlank" Value="true" />
                                                                    </CustomConfig>
                                                                    <Store>
                                                                      <ext:Store ID="Store9" runat="server">
                                                                        <Proxy>
                                                                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                                            CallbackParam="soaScmsCallback" />
                                                                        </Proxy>
                                                                        <BaseParams>
                                                                          <ext:Parameter Name="start" Value="={0}" />
                                                                          <ext:Parameter Name="limit" Value="={10}" />
                                                                          <ext:Parameter Name="model" Value="2011-b" />
                                                                          <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab_dcore.Contains(@0)', paramTextGetter(#{cbCustomerEksFltr}), '']]"
                                                                            Mode="Raw" />
                                                                          <ext:Parameter Name="sort" Value="v_cunam" />
                                                                          <ext:Parameter Name="dir" Value="ASC" />
                                                                        </BaseParams>
                                                                        <Reader>
                                                                          <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                                            TotalProperty="d.totalRows">
                                                                            <Fields>
                                                                              <ext:RecordField Name="c_cusno" />
                                                                              <ext:RecordField Name="c_cab_dcore" />
                                                                              <ext:RecordField Name="v_cunam" />
                                                                            </Fields>
                                                                          </ext:JsonReader>
                                                                        </Reader>
                                                                      </ext:Store>
                                                                    </Store>
                                                                    <Template ID="Template4" runat="server">
                                                                      <Html>
                                                                      <table cellpading="0" cellspacing="1" style="width: 400px">
                                                                      <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                                                                      <tpl for="."><tr class="search-item">
                                                                          <td>{c_cusno}</td><td>{c_cab_dcore}</td><td>{v_cunam}</td>
                                                                      </tr></tpl>
                                                                      </table>
                                                                      </Html>
                                                                    </Template>
                                                                    <Listeners>
                                                                      <Change Handler="reloadFilterGrid(#{GridPanel1} )" Buffer="300" Delay="300" />
                                                                    </Listeners>
                                                                  </ext:ComboBox>
                                                            </Component>
                                                        </ext:HeaderColumn>                                     
                                                        <ext:HeaderColumn>
                                                            <Component>
                                                              <ext:TextField ID="TextField2" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                                <Listeners>
                                                                  <KeyUp Handler="reloadFilterGrid(#{GridPanel1} )" Buffer="700" Delay="700" />
                                                                </Listeners>
                                                              </ext:TextField>
                                                            </Component>
                                                        </ext:HeaderColumn>
                                                        <ext:HeaderColumn />                                       
                                                        <ext:HeaderColumn />  
                                                    </Columns>
                                                </ext:HeaderRow>
                                            </HeaderRows>                                                
                                        </ext:GridView>
                                    </View>
                                    <BottomBar>
                                    </BottomBar>
                                </ext:GridPanel>                                
                            </Content>
                            </ext:Container>
                            <ext:Container ID="Container3" runat="server" > 
                            <Content>
                                <ext:GridPanel ID="GridPanel3" runat="server" Cls="mygrid" Height="132" EnableColumnHide="false" MaskDisabled="true">
                                    <LoadMask ShowMask="false" />
                                    <SelectionModel>
                                        <ext:RowSelectionModel SingleSelect="true" />
                                    </SelectionModel>
                                    <Store>
                                        <ext:Store ID="store2" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                                                <ext:Parameter Name="limit" Value="parseInt(10)" Mode="Raw" />
                                                <ext:Parameter Name="model" Value="0380-d" />
                                                <ext:Parameter Name="parameters" Value="[['Gudang = @0', #{cbPosisiStok}.getValue(), 'System.Char'],
                                                                                         ['DivAMS', paramValueGetter(#{cbDivAms}), ''],
                                                                                         ['Cab', paramValueGetter(#{cbCustomerFltr}), '']]" Mode="Raw" />
                                            </BaseParams>                                           
                                            <Reader>
                                                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                    <Fields>
                                                        <ext:RecordField Name="Gudang" />                                                        
                                                        <ext:RecordField Name="Cab" />
                                                        <ext:RecordField Name="DivAMS" />
                                                        <ext:RecordField Name="Urut" />
                                                        <ext:RecordField Name="Satu" />
                                                        <ext:RecordField Name="Dua" />
                                                        <ext:RecordField Name="Tiga" />
                                                        <ext:RecordField Name="Tipe" />                                         
                                                    </Fields>
                                                </ext:JsonReader>
                                            </Reader>
                                            <SortInfo Field="Urut" Direction="ASC" />
                                        </ext:Store>
                                    </Store>
                                    <ColumnModel>
                                        <Columns>  
                                            <ext:Column ColumnID="Satu" DataIndex="Satu" Header="SUMMARY" Width="158" Sortable="false" /> 
                                            <ext:Column ColumnID="Dua" DataIndex="Dua" Header="SUMMARY" Width="159" Sortable="false" /> 
                                            <ext:Column ColumnID="Tiga" DataIndex="Tiga" Header="SUMMARY" Width="159" Sortable="false" /> 
                                        </Columns>
                                    </ColumnModel>
                                    <View>
                                        <ext:GridView ID="GridView3" runat="server" StandardHeaderRow="true">                                       
                                        </ext:GridView>
                                    </View>
                                </ext:GridPanel>                                                                
                            </Content>                            
                            </ext:Container>
                        </Items>
                        <BottomBar>                        
                          <ext:Toolbar ID="Toolbar2" runat = "server" StyleSpec="text-align:right;">
                            <Items>
                              <ext:Container ID="Container5" Layout="TableLayout" runat="server" StyleSpec="text-align:right;"> 
                                <Content>            
                                    <ext:SelectBox ID="SelectTodo" runat="server" Width="135">
                                      <Items>
                                        <ext:ListItem Text="&nbsp;" Value="" />
                                        <ext:ListItem Text="SHOW LEGEND" Value="LEGEND" />
                                        <ext:ListItem Text="CREATE PL" Value="PL" />
                                        <ext:ListItem Text="LP. PROSES MONITORING" Value="PRINTGRIDMAIN" />
                                        <ext:ListItem Text="CHART" Value="CHART" />
                                      </Items>
                                    </ext:SelectBox>
                                    <ext:Button ID="ButtonTodo" runat="server" Text="Execute" Icon="BulletGo">
                                      <DirectEvents>
                                        <Click OnEvent="ButtonTodo_Click">
                                            <EventMask ShowMask="true" />                                       
                                        </Click>
                                      </DirectEvents>
                                    </ext:Button>               
                                </Content>        
                              </ext:Container>
                            </Items>
                          </ext:Toolbar>
                        </BottomBar>
                    </ext:Panel>
               </Items>                
            </ext:Panel>
        </Items>
    </ext:FormPanel>
    
    <ext:TaskManager runat="server" ID="taskMgr">
    <Tasks>
      <ext:Task TaskID="servertime"
          Interval="150000">
          <DirectEvents>
              <Update OnEvent="RefreshTime">
                  <EventMask 
                      ShowMask="false" 
                      Target="CustomTarget"                       
                      CustomTarget="={Ext.getCmp('#{gridMain}').getBody()}" 
                      MinDelay="350"
                      />
              </Update>
          </DirectEvents>                    
      </ext:Task>
      
      <ext:Task>                 
        <Listeners>
            <Update Handler="#{LocalTimeLabel}.setText(new Date().dateFormat('d/m/Y H:i:s'));" />
        </Listeners>    
      </ext:Task>
    </Tasks>
    </ext:TaskManager>   
  
    <ext:Window ID="winSummary" runat="server" Width="400" Height="300" Hidden="true"
    MinWidth="480" MinHeight="320" Layout="FitLayout" Maximizable="true">
    <Content>
      <ext:Hidden ID="Hidden1" runat="server" />
    </Content>
     <Items>
      <ext:GridPanel ID="GridPanel2" runat="server" Layout="Fit">
        <LoadMask ShowMask="true" />
        <Store>
            <ext:Store ID="store7" runat="server" RemoteSort="true">
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
            <ext:Parameter Name="model" Value="0380-b" />           
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
              IDProperty="TIPE">
              <Fields>
                <ext:RecordField Name="GUDANG" />
                <ext:RecordField Name="TIPE" />
                <ext:RecordField Name="DATA" />
              </Fields>
            </ext:JsonReader>
          </Reader>
        </ext:Store>
        </Store>
        <SelectionModel>
            <ext:RowSelectionModel SingleSelect="true" />
        </SelectionModel>
        <ColumnModel>
        <Columns>
          <ext:Column DataIndex="GUDANG" Header="Gudang" />
          <ext:Column DataIndex="TIPE" Header="Transaksi" />
          <ext:Column DataIndex="DATA" Header="Jumlah" />
        </Columns>
      </ColumnModel>
     </ext:GridPanel>
    </Items>
    </ext:Window>
      
    <%--<uc:MonitoringPLSummary ID="MonitoringPLSummary1" runat="server" />--%>
    <uc:MonitoringPLEkspedisi ID="MonitoringPLEkspedisi1" runat="server" />
    <uc:MonitoringPLGridDtlPL ID="MonitoringPLGridDtlPL1" runat="server" />
    <uc:PackingListCtrl ID="PackingListCtrl1" runat="server" />
    <uc:Legend ID="Legend1" runat="server" />
    <ext:Window ID="winPrintData" runat="server" Width="360" Height="440" Hidden="true" Title="Cetak Laporan Process Monitoring"
      MinWidth="360" MinHeight="440" Layout="FitLayout" Maximizable="false" Resizable="false">
      <Content>
        <ext:Hidden ID="hidItemSp" runat="server" />
      </Content>
      <Items>
        <ext:FormPanel ID="frmReportKriteria" runat="server" Padding="5" Frame="True" Layout="Form">
          <Items>
            <ext:RadioGroup ID="rdgTipe" runat="server" ColumnsNumber="1" FieldLabel="Tipe Laporan">
              <Items>
                <%--Indra GET DATA by ETA
                <ext:Radio ID="rd01" runat="server" BoxLabel="Tgl. SP Received dan Cabang tujuan" Checked="true" InputValue="01" />--%>
                <ext:Radio ID="rd01" runat="server" BoxLabel="Tgl. ETA SP dan Cabang tujuan" Checked="true" InputValue="01" />
                <ext:Radio ID="rd02" runat="server" BoxLabel="Grid Monitoring Process" InputValue="02" />
              </Items>
              <Listeners>
                <Change Handler="validateRadio(#{rd01}.getValue(), #{rd02}.getValue(), #{cbPosisiStokRpt}, #{TglSpReceived}, #{chkCabang}, #{GridPanel5});" />
              </Listeners>
            </ext:RadioGroup>
            <ext:ComboBox ID="cbPosisiStokRpt" runat="server" FieldLabel="Posisi Gudang" ValueField="c_gdg" 
            DisplayField="v_gdgdesc" Width="200" AllowBlank="true" ForceSelection="false" EmptyText="Pilihan..." >
            <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="true" />
            </CustomConfig>
            <Store>
            <ext:Store ID="Store11" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="allQuery" Value="true" />
                <ext:Parameter Name="model" Value="2031" />
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
                <Change Handler="reloadFilterGrid(#{gridMain});reloadFilterGrid(#{GridPanel1});reloadFilterGrid(#{GridPanel3});" />
            </Listeners>
            </ext:ComboBox>         
            <%--Indra GET DATA by ETA
            <ext:CompositeField ID="TglSpReceived" runat="server" FieldLabel="Tgl. SP Received">--%>
            <ext:CompositeField ID="TglSpReceived" runat="server" FieldLabel="Tgl. ETA SP">
              <Items>
                <ext:DateField ID="dtSPReceivedAwal" runat="server" Vtype="daterange" Format="dd-MM-yyyy" AllowBlank="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="endDateField" Value="#{dtSPReceivedAkhir}" Mode="Value" />
                  </CustomConfig>
                </ext:DateField>
                <ext:Label ID="Label2" runat="server" Text="-" />
                <ext:DateField ID="dtSPReceivedAkhir" runat="server" Vtype="daterange" Format="dd-MM-yyyy" AllowBlank="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="startDateField" Value="#{dtSPReceivedAwal}" Mode="Value" />
                  </CustomConfig>
                </ext:DateField>                
              </Items>
            </ext:CompositeField>
            <ext:Checkbox ID="chkCabang" runat="server" FieldLabel="Check/Uncheck" >
                <ToolTips>
                 <ext:ToolTip ID="TTPrint" runat="server" Html="Check/Uncheck Cabang">
                 </ext:ToolTip>
                </ToolTips>
                <Listeners>
                 <Check Handler="setPrint(this, #{GridPanel5}.getStore(),#{chkCabang});" />
                </Listeners>
            </ext:Checkbox>
            <ext:GridPanel ID="GridPanel5" runat="server" Cls="mygrid" Height="300" EnableColumnHide="false" MaskDisabled="true" >
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
                        <ext:Parameter Name="limit" Value="parseInt(10)" Mode="Raw" />
                        <ext:Parameter Name="model" Value="0380-g" />
                        <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
                    </BaseParams>                                           
                    <Reader>
                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                            <Fields>
                                <ext:RecordField Name="A_F" />
                                <ext:RecordField Name="AF" />                                                                                                                
                                <ext:RecordField Name="G_L" />
                                <ext:RecordField Name="GL" />                                                        
                                <ext:RecordField Name="M_R" />
                                <ext:RecordField Name="MR" />                                                        
                                <ext:RecordField Name="S_Z" />                                        
                                <ext:RecordField Name="SZ" />                                                        
                            </Fields>
                        </ext:JsonReader>
                    </Reader>
                    <%--<SortInfo Field="Urut" Direction="ASC" />--%>
                </ext:Store>
            </Store>
            <ColumnModel>
                <Columns>  
                    <ext:CheckColumn DataIndex="A_F" Width="30" Editable="true" Sortable="false"/>
                    <ext:Column DataIndex="AF" Header="A - F" Width="40" Editable="false" Sortable="false"/>
                    <ext:CheckColumn DataIndex="G_L" Width="30" Editable="true" Sortable="false"/> 
                    <ext:Column DataIndex="GL" Header="G - L" Width="40" Editable="false" Sortable="false"/>
                    <ext:CheckColumn DataIndex="M_R" Width="30" Editable="true" Sortable="false"/> 
                    <ext:Column DataIndex="MR" Header="M - R" Width="40" Editable="false" Sortable="false"/>
                    <ext:CheckColumn DataIndex="S_Z" Width="30" Editable="true" Sortable="false"/> 
                    <ext:Column DataIndex="SZ" Header="S - Z" Width="40" Editable="false" Sortable="false"/>
                </Columns>
            </ColumnModel>
            <View>
                <ext:GridView ID="GridView5" runat="server" StandardHeaderRow="true">                                       
                </ext:GridView>
            </View>
            </ext:GridPanel>                                                                
            
          </Items>   
          <Buttons>
            <ext:Button ID="btnReport" runat="server" Icon="Printer" Text="Cetak Laporan">
              <DirectEvents>
                <Click OnEvent="Report_OnGenerate">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{GridPanel5}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>     
        </ext:FormPanel>
      </Items>
    </ext:Window>
    <ext:Window ID="winGridPart" runat="server" Width="360" Height="440" Hidden="true" Title="Process Monitoring"
      MinWidth="360" MinHeight="440" Layout="FitLayout" Maximizable="true" Resizable="false">
      
      <Content>
        <ext:TextField ID="IDlabel" runat="server" Text="adalah"></ext:TextField>
      </Content>
      <Items>
        <ext:FormPanel ID="FormPanel1" runat="server" Padding="5" Frame="True" Layout="Form">
          <Items>
            <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Layout="Form" Width="841" MaskDisabled="true">                             
            <TopBar>
                <ext:Toolbar ID="Toolbar3" runat="server">                   
                    <Items>                     
                    </Items>
                </ext:Toolbar>
            </TopBar>
            <Items>
                <ext:Container ID="Container6" runat="server" Layout="FitLayout"> 
                <Content>
                    <ext:GridPanel ID="gridMain2" runat="server" Cls="mygrid" Height="540" EnableColumnHide="false" MaskDisabled="true" >
                        <LoadMask ShowMask="false" />
                        <DirectEvents>
                          <Command OnEvent="gridMainCommand" Before="if(command != 'GridDtlPL') { return false; }">
                            <EventMask ShowMask="false" />
                            <ExtraParams>
                              <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                              <ext:Parameter Name="Parameter" Value="NoPL" />
                              <ext:Parameter Name="PrimaryID" Value="record.data.NoPL" Mode="Raw" />
                              <ext:Parameter Name="PrimaryID2" Value="record.data.NoSP" Mode="Raw" />
                              <ext:Parameter Name="PrimaryID3" Value="record.data.NoDO" Mode="Raw" />
                            </ExtraParams>
                          </Command>
                        </DirectEvents>
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <Store>
                            <ext:Store ID="store10" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
                                <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                        CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="={0}" />
                                    <ext:Parameter Name="limit" Value="={2000}" />
                                </AutoLoadParams>
                                <BaseParams>
                                    <ext:Parameter Name="start" Value="0" />
                                    <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                                    <ext:Parameter Name="model" Value="0380" />
                                    <ext:Parameter Name="parameters" Value="[['Gudang = @0', #{cbPosisiStok}.getValue(), 'System.Char'],
                                                                             ['DivAms', paramValueGetter(#{cbDivAms}) + '%', 'System.String'],
                                                                             ['Cabang', paramValueGetter(#{cbCustomerFltr}) + '%', ''],
                                                                             ['CabNoSP', paramValueGetter(#{txSPCABFltr}) + '%', 'System.String'],
                                                                             ['NoSP', paramValueGetter(#{txSPHOFltr}) + '%', ''],
                                                                             ['Status', paramValueGetter(#{sbStatusFltr}) + '%', 'System.String'],
                                                                             ['ETAFILTER', paramValueGetter(#{sbETAFltr}), 'System.String']]" Mode="Raw" />
                                </BaseParams>
                               
                                
                                <Reader>
                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                        <Fields>
                                            <ext:RecordField Name="DivAms" />
                                            <ext:RecordField Name="Cabang" />
                                            <ext:RecordField Name="CabNoSP" />
                                            <ext:RecordField Name="NoSP" />
                                            <ext:RecordField Name="NoPL" />
                                            <ext:RecordField Name="NoDO" />
                                            <ext:RecordField Name="NoEP" />
                                            <ext:RecordField Name="Status" />
                                            <ext:RecordField Name="LastTimeAct" Type="Date" DateFormat="M$"/>                                            
                                            <ext:RecordField Name="ETA" Type="Date" DateFormat="M$" />
                                            <ext:RecordField Name="SPTIME" Type="Date" DateFormat="M$" />
                                            <ext:RecordField Name="Tipe" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                                <SortInfo Field="LastTimeAct" Direction="DESC" />
                            </ext:Store>
                        </Store>
                        <ColumnModel>
                            <Columns>  
                                <ext:Column ColumnID="DivAms" DataIndex="DivAms" Header="Div. AMS" Width="80" />
                                <ext:Column ColumnID="Cabang" DataIndex="Cabang" Header="Cabang" Width="60" />
                                <ext:Column ColumnID="CabNoSP" DataIndex="CabNoSP" Header="No. SP Cabang" Width="120" />
                                <ext:Column ColumnID="NoSP" DataIndex="NoSP" Header="No. SP HO / SG" Width="90" />
                                <ext:CommandColumn Width="30" DataIndex="" Resizable="false">
                                    <Commands>
                                        <ext:GridCommand Icon="BookOpen" CommandName="GridDtlPL" ToolTip-Text="Lihat detil Packing List" ToolTip-Title="Command" />
                                    </Commands>
                                </ext:CommandColumn>
                                <ext:Column ColumnID="NoPL" DataIndex="NoPL" Header="No. Picking List" Width="90" />                                                                                                                   
                                <ext:Column ColumnID="NoDO" DataIndex="NoDO" Header="No. DO / SJ" Width="90" />                           
                                <ext:Column ColumnID="Status" DataIndex="Status" Header="Status" Width="110" />  
                                <ext:DateColumn ColumnID="LastTimeAct" DataIndex="LastTimeAct" Header="Last Time Act." Format ="dd/MM/yyyy H:i" Width="110"/>
                                <ext:DateColumn ColumnID="SPTIME" DataIndex="SPTIME" Header="SP Received Time" Format ="dd/MM/yyyy H:i" Width="110"/>
                                <ext:DateColumn ColumnID="ETA" DataIndex="ETA" Header="ETA" Format ="dd/MM/yyyy H:i" Width="110"/>
                                <ext:Column ColumnID="Tipe" DataIndex="Tipe" Header="Tipe" Width="150" Hidden="true" />  
                            </Columns>
                        </ColumnModel>
                        <View>
                            <ext:GridView ID="GridView6" runat="server" StandardHeaderRow="true">
                                <HeaderRows>
                                    <ext:HeaderRow>
                                        <Columns>                                                    
                                            <ext:HeaderColumn />
                                            <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                                                <Component>
                                                    <ext:ComboBox ID="cbCustomerFltr2" runat="server" DisplayField="c_cab_dcore" ValueField="c_cab_dcore"
                                                        Width="60" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                                                        AllowBlank="true" ForceSelection="false">
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
                                                              <ext:Parameter Name="model" Value="2011-b" />
                                                              <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab_dcore.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
                                                                Mode="Raw" />
                                                              <ext:Parameter Name="sort" Value="v_cunam" />
                                                              <ext:Parameter Name="dir" Value="ASC" />
                                                            </BaseParams>
                                                            <Reader>
                                                              <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                                TotalProperty="d.totalRows">
                                                                <Fields>
                                                                  <ext:RecordField Name="c_cusno" />
                                                                  <ext:RecordField Name="c_cab_dcore" />
                                                                  <ext:RecordField Name="v_cunam" />
                                                                </Fields>
                                                              </ext:JsonReader>
                                                            </Reader>
                                                          </ext:Store>
                                                        </Store>
                                                        <Template ID="Template6" runat="server">
                                                          <Html>
                                                          <table cellpading="0" cellspacing="1" style="width: 400px">
                                                          <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                                                          <tpl for="."><tr class="search-item">
                                                              <td>{c_cusno}</td><td>{c_cab_dcore}</td><td>{v_cunam}</td>
                                                          </tr></tpl>
                                                          </table>
                                                          </Html>
                                                        </Template>
                                                        <Listeners>
                                                          <Change Handler="reloadFilterGrid(#{gridMain2});" Buffer="300" Delay="300" />
                                                        </Listeners>
                                                      </ext:ComboBox>
                                                </Component>
                                            </ext:HeaderColumn>                    
                                            <ext:HeaderColumn>
                                                <Component>
                                                  <ext:TextField ID="txSPCABFltr2" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                      <KeyUp Handler="reloadFilterGrid(#{gridMain2} )" Buffer="700" Delay="700" />
                                                    </Listeners>
                                                  </ext:TextField>
                                                </Component>
                                            </ext:HeaderColumn>
                                            <ext:HeaderColumn>
                                                <Component>
                                                  <ext:TextField ID="txSPHOFltr2" runat="server" EnableKeyEvents="true" AllowBlank="true">
                                                    <Listeners>
                                                      <KeyUp Handler="reloadFilterGrid(#{gridMain2} )" Buffer="700" Delay="700" />
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
                                        </Columns>
                                    </ext:HeaderRow>
                                </HeaderRows>
                                <GetRowClass Fn="getRowClass" />                                                 
                            </ext:GridView>
                        </View>
                        <BottomBar>                                       
                        </BottomBar>
                    </ext:GridPanel>
                </Content>
                </ext:Container>
            </Items>
            </ext:Panel>         
          </Items>   
          <Buttons>
            <ext:Button ID="Button1" runat="server" Icon="Printer" Text="Cetak Laporan">
              <DirectEvents>
                <Click OnEvent="Report_OnGenerate">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{GridPanel5}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>     
        </ext:FormPanel>
      </Items>
    </ext:Window>
    <ext:Window ID="winChart" runat="server" Width="990px" Height="420px" Hidden="true" Title="Chart Process Monitoring"
      MinWidth="990px" MinHeight="420px" Layout="FitLayout" Maximizable="false" Resizable="false">
      <Content>
        <ext:Hidden ID="Hidden2" runat="server" />
      </Content>
      <Items>
        <ext:FormPanel ID="FormPanel2" runat="server" Padding="5" Frame="True" Layout="Form">     
          <Items>
            <ext:Container ID="Container7" runat="server" > 
                <Content>
                    <iframe style="background-color:White" id="iframe1" runat="server" src="chart.html" width="950px" height="360px">                               
                    
                   </iframe>            
                </Content>   
            </ext:Container>
          </Items>                       
        </ext:FormPanel>        
      </Items>
    </ext:Window>    
</asp:Content>