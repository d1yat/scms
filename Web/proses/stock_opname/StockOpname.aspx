<%--
Created By Indra
20171231FM
--%>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StockOpname.aspx.cs" Inherits="proses_stock_opname_StockOpname"
 MasterPageFile="~/Master.master" %>

<%@ Register Src="StockOpnameNewBatch.ascx" TagName="StockOpnameNewBatch" TagPrefix="uc" %>
<%@ Register Src="StockOpnameMonitoring.ascx" TagName="StockOpnameMonitoring" TagPrefix="uc" %>
<%@ Register Src="StockOpnamePerProduk.ascx" TagName="StockOpnamePerProduk" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
    <script type="text/javascript">

        var afterEditDataConfirm = function(e, store, txKondisi) {

            var qtysys = e.record.get('qtysys'),
                SOQty = e.record.get('SOQty'),
                recount1 = e.record.get('Recount-1'),
                recount2 = e.record.get('Recount-2');

            if (e.field == 'expired') {

            }

            else if (e.field == 'print') {

            }

            else if (e.field == 'SOQty' && txKondisi == 'Buat Form') {
                if (SOQty >= 0) {
                    if (SOQty == 0 && txKondisi == 'Buat Form') {
                        e.record.set('SOQty', 0);
                        e.record.set('selisih', SOQty - qtysys);
                        e.record.set('saving', true);

                    } else if (SOQty != 0 && txKondisi == 'Buat Form') {
                        e.record.set('selisih', SOQty - qtysys);
                        e.record.set('saving', true);

                    } else {
                        e.record.set('Recount-1', e.originalValue);
                        e.record.set('Recount-2', e.originalValue);
                        e.record.set('saving', e.originalValue);
                    }
                }
                else {
                    alert('Tidak boleh di input kurang dari 0');
                    e.record.set('SOQty', 0);
                    SOQty = 0;
                    e.record.set('selisih', SOQty - qtysys);
                } 
            }

            else if (e.field == 'Recount-1' && txKondisi == 'SOQty') {
                if (recount1 >= 0) {
                    if (recount1 == 0 && txKondisi == 'SOQty') {
                        e.record.set('Recount-1', 0);
                        e.record.set('selisih', recount1 - qtysys);
                        e.record.set('saving', true);
                    } else if (recount1 != 0 && txKondisi == 'SOQty') {
                        e.record.set('selisih', recount1 - qtysys);
                        e.record.set('saving', true);
                    } else {
                        e.record.set('SOQty', e.originalValue);
                        e.record.set('Recount-2', e.originalValue);
                        e.record.set('saving', e.originalValue);
                    }
                }
                else {
                    alert('Tidak boleh di input kurang dari 0');
                    e.record.set('Recount-1', 0);
                    recount1 = 0;
                    e.record.set('selisih', recount1 - qtysys);
                }
            }

            else if (e.field == 'Recount-2' && txKondisi == 'Recount-1') {
                if (recount2 >= 0) {
                    if (recount2 == 0 && txKondisi == 'Recount-1') {
                        e.record.set('Recount-2', 0);
                        e.record.set('selisih', recount2 - qtysys);
                        e.record.set('saving', true);
                    } else if (recount2 != 0 && txKondisi == 'Recount-1') {
                        e.record.set('selisih', recount2 - qtysys);
                        e.record.set('saving', true);
                    } else {
                        e.record.set('SOQty', e.originalValue);
                        e.record.set('Recount-1', e.originalValue);
                        e.record.set('saving', e.originalValue);
                    }
                }
                else {
                    alert('Tidak boleh di input kurang dari 0');
                    e.record.set('Recount-2', 0);
                    recount2 = 0;
                    e.record.set('selisih', recount2 - qtysys);
                }
            }

            else {
                if (e.field == 'SOQty') {
                    e.record.set('SOQty', e.originalValue);
                    e.record.set('saving', e.originalValue);
                }
                else if (e.field == 'Recount-1') {
                    e.record.set('Recount-1', e.originalValue);
                    e.record.set('saving', e.originalValue);
                }
                else if (e.field == 'Recount-2') {
                    e.record.set('Recount-2', e.originalValue);
                    e.record.set('saving', e.originalValue);
                }

                ShowError('Tidak dapat input data / Anda salah kolom');

            }

        }
        
        var onCheckAutoValidation = function(isValid, cb) {
            if (isValid) {
                return true;
            }
        }

        var setPrint = function(chkDefault, store, chkDeclined) {
            var chk = chkDefault.getValue();

            if (chk) {
                store.each(function(e) {
                    if (e.get('selisih') != 0) {
                        e.set('print', true);
                    }
                });
            }
            else {
                store.each(function(e) {
                    e.set('print', false);
                });
            }
        }

        var getRowClass = function(record) {
            var cStatus = record.get('selisih');

            if (cStatus < 0) {
                return "orange";
            }

            if (cStatus > 0) {
                return "magenta";
            }
        }
        
    </script>
    
    <style type="text/css">    
    .magenta { 
        background: #E4E4E4; 
    }
    
    .orange {
        background: #FFCC66;  
    }       
    
</style>   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hidWndDown" runat="server" />
  <ext:Hidden ID="hfSysQty" runat="server" />
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar ID="Toolbar1" runat="server">
            <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table" LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>            
                      <ext:TextField ID="txKondisi" runat="server" MaxLength="100" Width="100" Disabled ="true" EmptyText = "Kondisi" />                                       
                      <ext:ToolbarSeparator />
                      <ext:ComboBox ID="cbDivAms" runat="server" ValueField="c_kddivams"
                      DisplayField="v_nmdivams" Width="170" ListWidth="400" 
                      PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false" EmptyText="Divisi Ams">
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
                        <Select Handler="this.triggers[0].show();" />
                        <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                        <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                      </Listeners>
                    </ext:ComboBox>
                      <ext:ComboBox ID="cbSuplier" runat="server" DisplayField="v_nama" ValueField="c_nosup"
                        Width="170" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                        AllowBlank="false" ForceSelection="false" EmptyText = "Principal">
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
                      <ext:ComboBox ID="cbDivSuplier" runat="server" DisplayField="v_nmdivpri" ValueField="c_kddivpri"
                        Width="170" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                        AllowBlank="false" ForceSelection="false" EmptyText="Div.Principal">
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
                      <ext:ToolbarSeparator />
                      <ext:SelectBox ID="cbKategori" runat="server" Width="100" ListWidth="100" SelectedIndex="0" EmptyText ="Kategori">
                        <Items>
                            <ext:ListItem Text="Regular" Value="01" />
                            <ext:ListItem Text="Psikotropika" Value="02" />
                            <ext:ListItem Text="Dingin" Value="03" />
                        </Items>
                      </ext:SelectBox>            
                      <ext:ToolbarSeparator />
                      <ext:ComboBox ID="cbItems" runat="server" ValueField="c_iteno"
                      DisplayField="v_itnam" Width="170" ListWidth="500" 
                      PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false" EmptyText = "Product">
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
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2141" />
                            <ext:Parameter Name="parameters" Value="[['@in.c_kddivams', paramValueMultiGetter(#{cbDivAms}), 'System.String[]'],
                              ['@in.c_nosup', paramValueMultiGetter(#{cbSuplier}), 'System.String[]'],
                              ['@in.c_kddivpri', paramValueMultiGetter(#{cbDivSuplier}), 'System.String[]'],
                              ['@in.c_type', paramValueMultiGetter(#{cbKategori}), 'System.String[]'],
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
                      <Template ID="Template1" runat="server">
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
                        <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                      </Listeners>
                      </ext:ComboBox>
                      <ext:ToolbarSeparator />
                      <ext:SelectBox ID="cbStatus" runat="server" Width="100" ListWidth="100" SelectedIndex="0" EmptyText="Status">
                        <Items>
                            <ext:ListItem Text="Stock Good" Value="Stock Good" />
                            <ext:ListItem Text="Stock Bad" Value="Stock Bad" />
                        </Items>
                      </ext:SelectBox>
                      <ext:ToolbarSeparator />
                      <ext:Button ID="btnGetBtn" runat="server" Text="1.Get Data" Icon="BulletRight" Disabled="false" ToolTip="Muat ulang data">
                        <DirectEvents>
                          <Click OnEvent="GetBtn_Click">
                            <EventMask ShowMask="true" />
                            <ExtraParams>
                              <ext:Parameter Name="cbDivAms" Value="#{cbDivAms}.getValue()" Mode="Raw" />
                              <ext:Parameter Name="cbSuplier" Value="#{cbSuplier}.getValue()" Mode="Raw" />
                              <ext:Parameter Name="cbDivSuplier" Value="#{cbDivSuplier}.getValue()" Mode="Raw" />
                              <ext:Parameter Name="cbKategori" Value="#{cbKategori}.getValue()" Mode="Raw" />
                              <ext:Parameter Name="cbItems" Value="#{cbItems}.getValue()" Mode="Raw" />
                              <ext:Parameter Name="cbStatus" Value="#{cbStatus}.getValue()" Mode="Raw" />
                            </ExtraParams>
                          </Click>
                        </DirectEvents>
                      </ext:Button>
                      <ext:ToolbarSeparator />
                      <ext:TextField ID="TxNoForm" runat="server" Width="100" ListWidth="100" EmptyText="No. Form">
                       <ToolTips>
                        <ext:ToolTip ID="TTNoForm" runat="server" Html="Masukan no. form">
                        </ext:ToolTip>
                       </ToolTips>
                       <DirectEvents>
                        <Change OnEvent="OnEvenAddGrid">
                          <ExtraParams>
                            <ext:Parameter Name="PrimaryID" Value="#{TxNoForm}.getValue()" Mode="Raw" />
                          </ExtraParams>
                        </Change>
                       </DirectEvents>
                      </ext:TextField>
                      <ext:ToolbarSeparator />
                      <ext:Label ID="LbSpasi12" runat="server" Text="Print" />
                      <ext:Label ID="LbSpasi13" runat="server" Text="&nbsp;" />
                      <ext:Checkbox ID="ChPrint" runat="server" Width="20" >
                       <ToolTips>
                         <ext:ToolTip ID="TTPrint" runat="server" Html="Print ALL selisih">
                         </ext:ToolTip>
                       </ToolTips>
                       <Listeners>
                         <Check Handler="setPrint(this, #{gridMain}.getStore(),#{ChPrint});" />
                       </Listeners>
                      </ext:Checkbox>
                    </Items>
                </ext:FormPanel>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel ID="gridMain" runat="server">
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
                      <ext:RecordField Name="batch" />
                      <ext:RecordField Name="qtysys" />
                      <ext:RecordField Name="SOQty" />                      
                      <ext:RecordField Name="Recount-1" />
                      <ext:RecordField Name="Recount-2" />
                      <ext:RecordField Name="selisih" />                      
                      <ext:RecordField Name="expired" Type="Date" DateFormat="M$" />                      
                      <ext:RecordField Name="box" />
                      <ext:RecordField Name="stage" />                      
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
              <ext:Column DataIndex="batch" Header="Batch" Width="100" />
              <ext:NumberColumn DataIndex="qtysys" Header="Qty. Sys" Format="0.000,00/i" Width="75" />              
              <ext:NumberColumn DataIndex="SOQty" Header="SO. Qty"  Format="0.000,00/i" Width="75" Tooltip="Ubah Qty">
                <Editor>
                  <ext:NumberField ID="NumberField1" DataIndex="SoQty" runat="server" Header="SO. Qty" Width="75" />
                </Editor>
              </ext:NumberColumn>              
              <ext:NumberColumn DataIndex="Recount-1" Header="Recount-1" Format="0.000,00/i" Width="75" Tooltip="Ubah Qty Recount-1">
                <Editor>
                  <ext:NumberField ID="NumberField3" DataIndex="Recount-1" runat="server" Header="Recount-1" Width="75" />
                </Editor>
              </ext:NumberColumn>              
              <ext:NumberColumn DataIndex="Recount-2" Header="Recount-2" Format="0.000,00/i" Width="75" Tooltip="Ubah Qty Recount-2">
                <Editor>
                  <ext:NumberField ID="NumberField4" DataIndex="Recount-2" runat="server" Header="Recount-2" Width="75" />
                </Editor>
              </ext:NumberColumn>              
              <ext:NumberColumn DataIndex="selisih" Header="Var" Format="0.000,00/i" Width="70" />                 
              <ext:DateColumn DataIndex="expired" Header="Expired" Format="dd-MM-yyyy" Width="75" Tooltip="Ubah tanggal expired">
                <Editor>
                  <ext:DateField ID="NumberField2" DataIndex="expired" runat="server" Header="Expired" Width="75" />
                </Editor>
              </ext:DateColumn>                                       
              <ext:NumberColumn DataIndex="box" Header="Box" Format="0.000,00/i" Width="73" />                
              <ext:NumberColumn DataIndex="stage" Header="Stage" Width="50" Hidden="true"/>                
              <ext:CheckColumn DataIndex="print" Header="P" width="25" Editable="true" Tooltip="Pilih print" />                   
              <ext:CheckColumn DataIndex="saving" Header="S" width="25" Editable="false" Tooltip="Simpan data"  />                   
              </Columns>                
            </ColumnModel>                  
            <Listeners>
              <AfterEdit Handler="afterEditDataConfirm(e, #{gridMain}.getStore(), #{txKondisi}.getValue());" />    
            </Listeners>
            <DirectEvents>
              <AfterEdit OnEvent = "BtnSaveData_Click">
                <ExtraParams>
                  <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw"/>
                </ExtraParams>
              </AfterEdit>
            </DirectEvents>             
            <View>
              <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                <HeaderRows>
                  <ext:HeaderRow>
                    <Columns>                
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn>                                                                                                                                                                 
                     </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNmbarang" runat="server" EnableKeyEvents="true" AllowBlank="true" >
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                      <ext:HeaderColumn/>
                    </Columns>
                  </ext:HeaderRow>
                </HeaderRows>
                <GetRowClass Fn="getRowClass" /> 
              </ext:GridView>
            </View>                              
          </ext:GridPanel>
        </Items>
        <BottomBar>
          <ext:Toolbar runat = "server">
            <Items>
             <ext:ComboBox ID="cbBentukForm" runat="server" EmptyText="Bentuk Form" Width="85" AllowBlank="true">
              <Items>
                <ext:ListItem Text="Per Principal" />
                <ext:ListItem Text="Per Div. Principal" />
              </Items>
              <ToolTips>
                <ext:ToolTip ID="ToolTip1" runat="server" Html="Pilih per Principal untuk buat form per Principal atau <br /> Pilih per Div. Principal untuk buat form per Div. Principal">
                </ext:ToolTip>
              </ToolTips>
             </ext:ComboBox>  
             <ext:Button ID="btnBuatFormSO" runat="server" Text="2.Buat Form SO" Icon="PageAdd" ToolTip="Pembuatan form SO">
              <DirectEvents>
                <Click OnEvent="btnBuatFormSO_OnClick">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail();"
                    ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin membuat Form SO." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>             
             <ext:ToolbarSeparator />   
             <ext:Button ID="btnFormSOBatal" runat="server" Text="Form SO dibatalkan" Icon="Cancel" ToolTip="Pembatalan form SO">
              <DirectEvents>
                <Click OnEvent="btnFormSOBatal_OnClick">
                  <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin Membatalkan Form SO." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>                         
             <ext:Button ID="btnAddNewBatch" runat="server" Text="New Batch" Icon="Add" ToolTip="Penambahan batch">
              <DirectEvents>
                <Click OnEvent="btnAddNew_OnClick">
                  <EventMask ShowMask="true" />
                </Click>
              </DirectEvents>
            </ext:Button>   
             <ext:ToolbarSeparator />                                   
             <ext:Button ID="btnPrintFormSO" runat="server" Text="3.Form S/O" Icon="Printer" ToolTip="Print form hitung SO">
              <DirectEvents>
                <Click OnEvent="btnPrintingSO_OnClick">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="1" Mode="Raw" />
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
             </ext:Button>
             <ext:Button ID="btnPrintBlankFormSO" runat="server" Text="Blank Form S/O" Icon="Printer" ToolTip="Print hitung blank form SO">
              <DirectEvents>
                <Click OnEvent="btnPrintingSO_OnClick">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="4" Mode="Raw" />
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
             </ext:Button>            
             <ext:Button ID="btnPrintHasilSO" runat="server" Text="4.Hasil S/O" Icon="Printer" ToolTip="Print Hasil Hitung SO">
              <DirectEvents>
                <Click OnEvent="btnPrintingSO_OnClick">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="2" Mode="Raw" />
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>              
             <ext:Button ID="btnPrintHasilAdjust" runat="server" Text="7.Hasil Adjustment" Icon="Printer" ToolTip="Print detail hasil adjusment SO">
              <DirectEvents>
                <Click OnEvent="btnPrintingSO_OnClick">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="3" Mode="Raw" />
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
             </ext:Button>  
             <ext:Button ID="btnPrintHasilAdjustRekap" runat="server" Text="8.Hasil Adjustment Rekap" Icon="Printer" ToolTip="Print rekap hasil adjusment SO">
              <DirectEvents>
                <Click OnEvent="btnPrintingSO_OnClick">
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="5" Mode="Raw" />
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
             </ext:Button> 
             <ext:ToolbarSeparator />   
             <ext:ComboBox ID="cbTipeReport" runat="server" EmptyText="Tipe report" Width="85" AllowBlank="true">
              <Items>
                <ext:ListItem Text="Excel" />
                <ext:ListItem Text="PDF" />
              </Items>
              <ToolTips>
                <ext:ToolTip ID="ToolTip2" runat="server" Html="Pilih Excel untuk cetak form dengan format .xls atau <br /> Pilih PDF untuk cetak form dengan format .pdf">
                </ext:ToolTip>
              </ToolTips>
             </ext:ComboBox>
             <ext:ComboBox ID="cbKateReport" runat="server" EmptyText="Kategori report" Width="100" AllowBlank="true">
              <Items>
                <ext:ListItem Text="Per No. Form" />
                <ext:ListItem Text="Semua Form" />
              </Items>
              <ToolTips>
                <ext:ToolTip ID="ToolTip3" runat="server" Html="Pilih Per No. Form untuk cetak form per no. form atau <br /> Pilih Semua Form untuk cetak semua no. form">
                </ext:ToolTip>
              </ToolTips>
             </ext:ComboBox>
            </Items>
          </ext:Toolbar>
        </BottomBar>
        <Buttons>
          <ext:Button ID="btnGetData" runat="server" Icon="Disk" Text="0.Get Data First" ToolTip="Lakukan Get Data First before get data">
            <DirectEvents>
              <Click OnEvent="btnGetData_OnClick">
                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin Get Data First." />
                <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="6" Mode="Raw" />
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>             
          <ext:Button ID="BtnPerProduk" runat="server" Icon="Monitor" Text="SO Per Produk" ToolTip="SO Per Produk">
            <DirectEvents>
              <Click OnEvent="btnPerProduk_OnClick">
                <EventMask ShowMask="true" />
              </Click>
            </DirectEvents>
          </ext:Button>          
          <ext:Button ID="btnRefresh" runat="server" Icon="ArrowRefresh" Text="Refresh Form" ToolTip="Muat ulang form">
            <DirectEvents>
              <Click OnEvent="btnRefresh_OnClick">
                <EventMask ShowMask="true" />
              </Click>
            </DirectEvents>
          </ext:Button>
          <ext:Button ID="btnMonitoring" runat="server" Icon="Monitor" Text="Monitoring SO" ToolTip="Monitoring stock opname">
            <DirectEvents>
              <Click OnEvent="btnMonitor_OnClick">
                <EventMask ShowMask="true" />
              </Click>
            </DirectEvents>
          </ext:Button>
          <ext:Button ID="btnConfirm" runat="server" Icon="Accept" Text="5.Confirm Hasil" ToolTip="Confirm hasil">
            <DirectEvents>
              <Click OnEvent="btnConfirm_OnClick">
                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin confirm hasil." />
                <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>
          <ext:Button ID="btnAdjust" runat="server" Icon="CartEdit" Text="6.Adjustment Stock" ToolTip="Adjusment stock">
            <DirectEvents>
              <Click OnEvent="btnAdjust_OnClick">
                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin meng-adjust stock." />
                <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>
          <ext:Button ID="btnSOUlang" runat="server" Icon="ArrowUndo" Text="9.SO Ulang" ToolTip="SO Ulang">
            <DirectEvents>
              <Click OnEvent="btnSOUlang_OnClick">
                <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin SO ulang." />
                <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridMain}.getStore())" Mode="Raw" />
                  </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>
        </Buttons>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  
  <uc:StockOpnameNewBatch ID="StockOpnameNewBatch1" runat="server" />
  <uc:StockOpnameMonitoring ID="StockOpnameMonitoring1" runat="server" />
  <uc:StockOpnamePerProduk ID="StockOpnamePerProduk1" runat="server" />  
  
  <ext:Window ID="wndDown" runat="server" Hidden="true" />
</asp:Content>



