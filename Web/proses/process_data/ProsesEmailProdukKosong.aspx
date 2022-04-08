<%--
 Indra 20190411FM Penambahan modul produk kosong
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProsesEmailProdukKosong.aspx.cs"
  Inherits="ProsesEmailProdukKosong" MasterPageFile="~/Master.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidPKDataFromStore = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
              function(btnP, txt) {
                if (btnP == 'ok') {
                  if (txt.trim().length < 1) {
                    txt = '';
                  }

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_pkno'), txt);
                }
              });
          }
        });
    }

    var validateRadio = function(rdTipeinput, dtABEtx, dtExpiredtx) {
        if (rdTipeinput) {
            dtABEtx.setVisible(true);
            dtExpiredtx.setVisible(false);
        } else {
            dtABEtx.setVisible(false);
            dtExpiredtx.setVisible(true);
        };
    };
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport runat="server" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfPKNo" runat="server" />
  </Content>
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
              <ext:Hidden ID="hidWndDown" runat="server" />              
              <ext:ToolbarSeparator />
              <ext:Button ID="Button1" runat="server" Text="Segarkan" Icon="ArrowRefresh">
                <Listeners>
                  <Click Handler="refreshGrid(#{gridMain});" />
                </Listeners>
              </ext:Button>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel ID="gridMain" runat="server">
            <LoadMask ShowMask="true" />
            <Listeners>
              <Command Handler="if(command == 'DeleteDataProduk') { voidPKDataFromStore(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="gridMainCommand" >
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_pkno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_pkno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="storeGridEPK" runat="server" RemoteSort="true" SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0231" />
                  <ext:Parameter Name="parameters" Value="[['c_pkno', paramValueGetter(#{txPkno}) + '%', '']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_pkno">
                    <Fields>            
                      <ext:RecordField Name="c_pkno" Type="String" />
                      <ext:RecordField Name="d_pkdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_nama" Type="String" /> 
                      <ext:RecordField Name="v_nmdivpri" Type="String" /> 
                      <ext:RecordField Name="c_iteno" Type="String" /> 
                      <ext:RecordField Name="v_itnam" Type="String" /> 
                      <ext:RecordField Name="c_tipe" Type="String" />
                      <ext:RecordField Name="pkdt" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="nedt" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="l_sent" Type="Boolean" />
                      <ext:RecordField Name="d_sent" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_username" Type="String" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_pkno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="EditDataProduk" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Ubah data produk kosong" />
                    <ext:GridCommand CommandName="DeleteDataProduk" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus data produk kosong" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_pkno" DataIndex="c_pkno" Header="No. Dokumen" Width="90"/>
                <ext:DateColumn DataIndex="d_pkdate" Header="Tanggal Input" Format="dd-MM-yyyy" Width="75" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama Principal" Width="200"/>
                <ext:Column ColumnID="v_nmdivpri" DataIndex="v_nmdivpri" Header="Nama Div. Principal" Width="200" />
                <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kd. Item" Width="50"/>
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Item" Width="200"/>
                <ext:Column ColumnID="c_tipe" DataIndex="c_tipe" Header="Tipe" Width="50" Tooltip="PK=Produk Kosong|NE=Nearly Expired"/>
                <ext:DateColumn DataIndex="pkdt" Header="ABE" Format="dd-MM-yyyy" Width="75" />
                <ext:DateColumn DataIndex="nedt" Header="Expired" Format="dd-MM-yyyy" Width="80" />                
                <ext:CheckColumn ColumnID="l_sent" DataIndex="l_sent" Header="Send" Width="50" />
                <ext:DateColumn DataIndex="d_sent" Header="Last Sent" Format="dd-MM-yyyy H:i" Width="110" />
                <ext:Column ColumnID="v_username" DataIndex="v_username" Header="Nm. Penginput" Width="120"/>
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txPkno});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txPkno" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
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
                    </Columns>
                  </ext:HeaderRow>
                </HeaderRows>
              </ext:GridView>
            </View>
            <BottomBar>
              <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
                <Items>
                  <ext:Label runat="server" Text="Page size:" />
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
          </ext:GridPanel>
        </Items>
        <Buttons>
          <ext:Button ID="BtnAdd" runat="server" Text="Tambah Produk" Icon="Add">
              <DirectEvents>
                <Click OnEvent="BtnAdd_Click">
                    <EventMask ShowMask="true" />                                       
                </Click>
              </DirectEvents>
          </ext:Button> 
          <ext:Button ID="BtnSend" runat="server" Text="Send Email" Icon="EmailGo">
              <DirectEvents>
                <Click OnEvent="BtnSend_Click">
                    <EventMask ShowMask="true" />                                       
                </Click>
              </DirectEvents>
          </ext:Button> 
          <ext:Button ID="BtnHistory" runat="server" Text="History Produk Kosong" Icon="DatabaseTable">
              <DirectEvents>
                <Click OnEvent="BtnHistory_Click">
                    <EventMask ShowMask="true" />                                       
                </Click>
              </DirectEvents>
          </ext:Button>
          <ext:Button ID="btnReport" runat="server" Text="Print to Excel" Icon="Printer">
              <DirectEvents>
                <Click OnEvent="Report_OnGenerate">
                    <EventMask ShowMask="true" />                                       
                </Click>
              </DirectEvents>
          </ext:Button> 
        </Buttons>
      </ext:Panel>
    </Items>
  </ext:Viewport> 
  
  <ext:Window ID="winTambahProduk" runat="server" Width="400px" Height="310px" Hidden="true" Title="Tambah Produk Kosong"
      MinWidth="400px" MinHeight="310px" Layout="FitLayout" Maximizable="false" Resizable="false">
      <Content>
        <ext:Hidden ID="Hidden2" runat="server" />
      </Content>
      <Items>
        <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" Layout="Form" LabelAlign="Left">
          <Items>
          <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Padding="10">
            <Items>
              <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
              DisplayField="v_nama" Width="250" ListWidth="300" PageSize="10"
              ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
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
              <Template ID="Template2" runat="server">
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
              <Triggers>
                <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
              </Triggers>
              <Listeners>
                <Select Handler="this.triggers[0].show();" />
                <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                <Change Handler="clearRelatedComboRecursive(true, #{cbDivPrinsipal});" />
              </Listeners>
              </ext:ComboBox>
              <ext:ComboBox ID="cbDivPrinsipal" runat="server" FieldLabel="Divisi Pemasok" ValueField="c_kddivpri"
              DisplayField="v_nmdivpri" Width="250" ListWidth="300" 
              PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="true" />
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
                <Select Handler="this.triggers[0].show();" />
                <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                <Change Handler="clearRelatedComboRecursive(true, #{cbItems});" />
              </Listeners>
            </ext:ComboBox>
              <ext:ComboBox ID="cbItems" runat="server" FieldLabel="Produk" ValueField="c_iteno"
              DisplayField="v_itnam" Width="250" ListWidth="300" 
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
                        <ext:RecordField Name="v_itnam" />
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
                    <td>{c_iteno}</td><td>{v_itnam}</td>
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
              <ext:RadioGroup ID="rdgJenisSP" runat="server" ColumnsNumber="1" FieldLabel="Tipe Produk">
                <Items>
                    <ext:Radio ID="rdProdukKosong" runat="server" Checked="true" BoxLabel="Produk Kosong" />
                    <ext:Radio ID="rdProdukExpired" runat="server" BoxLabel="Nearly Expired" />
                </Items>
                    <Listeners>
                        <Change Handler="validateRadio(#{rdProdukKosong}.getValue(), #{CompositeField1}, #{CompositeField2});" />
                    </Listeners>
              </ext:RadioGroup>
              <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="ABE">
              <Items>
                <ext:DateField ID="dtABE" runat="server" Format="dd-MM-yyyy" EnableKeyEvents="true" Editable="false" />
              </Items>
              <ToolTips>
                <ext:ToolTip ID="ttABE" runat="server" Html="Available Best Estimate">
                </ext:ToolTip>
              </ToolTips>
              </ext:CompositeField>
              <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Expired" Hidden="true">              
              <Items>
                <ext:DateField ID="dtExpired" runat="server" Format="dd-MM-yyyy" EnableKeyEvents="true" Editable="false" />
              </Items>
              <ToolTips>
                <ext:ToolTip ID="ttExpired" runat="server" Html="Tanggal expired produk">
                </ext:ToolTip>
              </ToolTips>
              </ext:CompositeField>
              <ext:Checkbox ID="chkSendEmail" runat="server" FieldLabel="Send to email" Checked="true">
              <ToolTips>
                <ext:ToolTip ID="ttSendEmail" runat="server" Html="Check list jika ingin dikirim email">
                </ext:ToolTip>
              </ToolTips>
              </ext:Checkbox>
              <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Produk aktif" Checked="true">
              <ToolTips>
                <ext:ToolTip ID="ttAktif" runat="server" Html="Check list jika produk ingin tetap aktif">
                </ext:ToolTip>
              </ToolTips>
              </ext:Checkbox>
            </Items>
            <Buttons>
                <ext:Button ID="btnSimpan" runat="server" Text="Simpan data" Icon="PageAdd" ToolTip="Tekan untuk menyimpan data">
                  <DirectEvents>
                    <Click OnEvent="btnbtnSimpan_OnClick">                      
                      <ExtraParams>
                        <ext:Parameter Name="NumberID" Value="#{hfPKNo}.getValue()" Mode="Raw" />
                      </ExtraParams>
                    </Click>
                  </DirectEvents>
                </ext:Button>                
            </Buttons>
          </ext:FormPanel>            
          </Items>
        </ext:Panel>  
      </Items>
  </ext:Window>  
  <ext:Window ID="winHistoryProduk" runat="server" Height="500" Width="1350" Hidden="true" Resizable="false"
  Maximizable="true" MinHeight="500" MinWidth="1350" Layout="Fit">
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="History Produk Kosong" Height="150" Layout="Fit">
           <TopBar>
              <ext:Toolbar ID="Toolbar1" runat="server">
                <Items>        
                <ext:ToolbarSeparator />
                <ext:Button ID="Button3" runat="server" Text="Segarkan" Icon="ArrowRefresh">
                  <Listeners>
                    <Click Handler="refreshGrid(#{gridHistory});" />
                  </Listeners>
                </ext:Button>
              </Items>
            </ext:Toolbar>
          </TopBar>
          <Items>
            <ext:GridPanel ID="gridHistory" runat="server">
            <LoadMask ShowMask="true" />
            <DirectEvents>
              <Command OnEvent="gridMainCommand" >
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_pkno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_pkno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
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
                  <ext:Parameter Name="limit" Value="={20}" />
                </AutoLoadParams>
                <BaseParams>
                  <ext:Parameter Name="start" Value="0" />
                  <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                  <ext:Parameter Name="model" Value="0231-b" />
                  <ext:Parameter Name="parameters" Value="[[['c_pkno', paramValueGetter(#{txPkno2}) + '%', '']]]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_pkno">
                    <Fields>            
                      <ext:RecordField Name="c_pkno" Type="String" />
                      <ext:RecordField Name="d_pkdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_nama" Type="String" /> 
                      <ext:RecordField Name="v_nmdivpri" Type="String" /> 
                      <ext:RecordField Name="c_iteno" Type="String" /> 
                      <ext:RecordField Name="v_itnam" Type="String" /> 
                      <ext:RecordField Name="pkdt" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="nedt" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="l_sent" Type="Boolean" />
                      <ext:RecordField Name="d_sent" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_username" Type="String" />
                      <ext:RecordField Name="l_delete" Type="Boolean" />
                      <ext:RecordField Name="c_keterangan" Type="String" />
                      <ext:RecordField Name="c_tipe" Type="String" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_pkno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="25" Resizable="false">
                </ext:CommandColumn>
                <ext:Column ColumnID="c_pkno" DataIndex="c_pkno" Header="No. Dokumen" Width="100"/>
                <ext:DateColumn DataIndex="d_pkdate" Header="Tanggal Input" Format="dd-MM-yyyy" Width="75" />
                <ext:Column ColumnID="v_nama" DataIndex="v_nama" Header="Nama Principal" Width="200"/>
                <ext:Column ColumnID="v_nmdivpri" DataIndex="v_nmdivpri" Header="Nama Div. Principal" Width="200" />
                <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kd. Item" Width="50"/>
                <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama Item" Width="200"/>
                <ext:Column ColumnID="c_tipe" DataIndex="c_tipe" Header="Tipe" Width="50" Tooltip="PK=Produk Kosong|NE=Nearly Expired"/>
                <ext:DateColumn DataIndex="pkdt" Header="ABE" Format="dd-MM-yyyy" Width="75" />
                <ext:DateColumn DataIndex="nedt" Header="Expired" Format="dd-MM-yyyy" Width="80" />
                <ext:CheckColumn ColumnID="l_sent" DataIndex="l_sent" Header="Send" Width="50" />
                <ext:DateColumn DataIndex="d_sent" Header="Last Send" Format="dd-MM-yyyy" Width="75" />
                <ext:Column ColumnID="v_username" DataIndex="v_username" Header="Nm. Penginput" Width="150"/>
                <ext:CheckColumn ColumnID="l_delete" DataIndex="l_delete" Header="Delete" Width="50" />
                <ext:Column ColumnID="c_keterangan" DataIndex="c_keterangan" Header="Ket. Hapus" Width="250"/>
              </Columns>
            </ColumnModel>
            <View>
              <ext:GridView ID="GridView2" runat="server" StandardHeaderRow="true">
                <HeaderRows>
                  <ext:HeaderRow>
                    <Columns>
                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                        <Component>
                          <ext:Button ID="Button2" runat="server" Icon="Cancel" ToolTip="Clear filter">
                            <Listeners>
                              <Click Handler="clearFilterGridHeader(#{gridHistory}, #{txPkno2});reloadFilterGrid(#{gridHistory});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txPkno2" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridHistory})" Buffer="300" Delay="300" />
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
                    </Columns>
                  </ext:HeaderRow>
                </HeaderRows>
              </ext:GridView>
            </View>
            <BottomBar>
              <ext:PagingToolbar runat="server" ID="PagingToolbar1" PageSize="20">
                <Items>
                  <ext:Label ID="Label1" runat="server" Text="Page size:" />
                  <ext:ToolbarSpacer ID="ToolbarSpacer2" runat="server" Width="10" />
                  <ext:ComboBox ID="ComboBox1" runat="server" Width="80">
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
          </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  
</ext:Window>
  <ext:Window ID="wndDown" runat="server" Hidden="true" />
</asp:Content>

