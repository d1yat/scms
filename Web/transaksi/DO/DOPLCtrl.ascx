<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DOPLCtrl.ascx.cs" 
Inherits="transaksi_DO_DOPLCtrl" %>

<%@ Register Src="DOPLCtrlPrint.ascx" TagName="DOPLCtrlPrint" TagPrefix="uc1" %>

<script type="text/javascript">

  var prepareCommands = function(toolbar, valX) {
    var del = toolbar.items.get(0); // delete button
    var vd = toolbar.items.get(1); // void button

    if (Ext.isEmpty(valX)) {
      del.setVisible(true);
      vd.setVisible(false);
    }
    else {
      del.setVisible(false);
      vd.setVisible(true);
    }
  };

  var voidPLDataFromStore = function(rec, dm) {
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
                        txt = 'Kesalahan pemakai.';
                      }
                      dm.DeleteMethod(rec.get('c_dono'), txt);
                    }
                  });
              }
            });
          }



    var voidInsertedDataFromStore = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      var isVoid = rec.get('l_void');

      if (isVoid) {
        ShowWarning('Item ini telah di batalkan.');
      }
      else {
        ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
              function(btnP, txt) {
                if (btnP == 'ok') {
                  if (txt.trim().length < 1) {
                    txt = 'Kesalahan pemakai.';
                  }
                  rec.set('l_void', true);
                  rec.set('v_ket', txt);
                }
              });
          }
        });
      }
    }
</script>

<ext:Window ID="winDetil" runat="server" Hidden="true" Layout="Fit" MinHeight="500" MinWidth="730">
  <Content>
    <ext:Hidden ID="hfDONo" runat="server" />
    <ext:Hidden ID="hfDOType" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout runat="server" ID="bdrLay1">
      <North MinHeight="175" MaxHeight="175" Collapsible="true">
        <ext:FormPanel ID="pnlHeaders" runat="server" Title="Header" Height="175" Padding="10">
          <Items>
            <ext:Panel runat="server" Header="false" Border="false">
              <Items>
                <ext:ComboBox FieldLabel="Customer" runat="server" ID="txCustomerHeader" DisplayField="v_cunam" ValueField="c_cusno"
                Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                ForceSelection="false" AllowBlank="false">
                  <Store>
                    <ext:Store runat="server" ID="storeID" >
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Proxy>
                        <ext:ScriptTagProxy CallbackParam="soaScmsCallback" 
                        Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" 
                        Timeout="10000000" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="={10}" />
                        <ext:Parameter Name="model" Value="2011" />
                        <ext:Parameter Name="parameters" 
                                       Value="[['l_hide = @0', false, 'System.Boolean'],
                                       ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{Customer}), '']]"
                                       Mode="Raw" />
                        <ext:Parameter Name="sort" Value="v_cunam" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_cusno" />
                            <ext:RecordField Name="c_cab" />
                            <ext:RecordField Name="v_cunam" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                          </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
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
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Select Handler="clearRelatedComboRecursive(this, #{txPlHeader})" />
                      </Listeners>
                </ext:ComboBox>
                
                <ext:ComboBox FieldLabel="No PL" runat="server" ID="txPlHeader" DisplayField="c_plno" ValueField="c_plno"
                Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                AllowBlank="false" ForceSelection="false">
                  <Store>
                    <ext:Store runat="server">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Proxy>
                        <ext:ScriptTagProxy CallbackParam="soaScmsCallback" 
                        Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" 
                        Timeout="10000000" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="={10}" />
                        <ext:Parameter Name="model" Value="6001" />
                        <ext:Parameter Name="parameters" 
                                       Value="[['cusno', #{txCustomerHeader}.getValue(), 'System.String'],
                                       ['@contains.c_plno.Contains(@0)', paramTextGetter(#{txPlHeader}), '']]"
                                       Mode="Raw" />
                        <ext:Parameter Name="sort" Value="c_plno" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_plno" Root="d.records" SuccessProperty="d.success"
                          TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_plno" />
                            <ext:RecordField Name="v_ket" />
                            <ext:RecordField Name="ket" Type="Int" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                          </ext:Store>
                      </Store>
                      <Template ID="tmpPL" runat="server" >
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 200px" >
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Ket</td>
                        <td class="body-panel">Flag</td>
                        <tpl for="."><tr class="search-item">
                            <td>{c_plno}</td> <td>{v_ket}</td><td>{ket}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                       <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                      </Listeners>
                      <DirectEvents>
                        <Change OnEvent="OnEvenAddGrid">
                          <ExtraParams>
                            <%--<ext:Parameter Name="Command" Value="Command" Mode="Raw" />--%>
                            <ext:Parameter Name="Parameter" Value="c_plno" />
                            <ext:Parameter Name="PrimaryID" Value="#{txPlHeader}.getValue()" Mode="Raw" />
                          </ExtraParams>
                        </Change>
                      </DirectEvents>
                </ext:ComboBox>
                
                <ext:ComboBox ID="txVia" runat="server" FieldLabel="Via" DisplayField="v_ket"
                ValueField="c_type" Width="250" TypeAhead="false" AllowBlank="false" ForceSelection="false">
                <Store>
                  <ext:Store runat="server" RemotePaging="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="false" />
                    </CustomConfig>
                    <Proxy>
                      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                        CallbackParam="soaScmsCallback" />
                    </Proxy>
                    <BaseParams>
                      <ext:Parameter Name="allQuery" Value="true" />
                      <ext:Parameter Name="model" Value="2001" />
                      <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                ['c_notrans = @0', '02', ''],
                                                ['@contains.c_type.Contains(@0) || @contains.v_ket.Contains(@0)', paramTextGetter(#{txVia}), '']]" Mode="Raw" />
                      <ext:Parameter Name="sort" Value="c_type" />
                      <ext:Parameter Name="dir" Value="ASC" />
                    </BaseParams>
                    <Reader>
                      <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                        TotalProperty="d.totalRows">
                        <Fields>
                          <ext:RecordField Name="c_type" />
                          <ext:RecordField Name="v_ket" />
                        </Fields>
                      </ext:JsonReader>
                    </Reader>
                  </ext:Store>
                </Store>
                <Triggers>
                  <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                </Triggers>
                <Listeners>
                  <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                </Listeners>
              </ext:ComboBox>
              
              <ext:TextField ID="txKet" runat="server" AllowBlank="true" FieldLabel="Keterangan" />  
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center Collapsible="true" MinHeight="175" MaxHeight="175">
        <ext:FormPanel ID="pnlDetil"  runat="server" Title="Detail" Height="200" Padding="10">
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server" Height="200">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store runat="server" RemotePaging="false" RemoteSort="false" >
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <%--<ext:Parameter Name="model" Value="" />--%>
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <%--<ext:Parameter Name="parameters" Value="[[]]"
                      Mode="Raw" />--%>
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="n_sisa" Type="Float" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Delete" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(toolbar, #{hfDONo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="250" />
                  <ext:NumberColumn DataIndex="n_sisa" Header="Quantity" Format="0.000,00/i" Width="75" />
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
              </Listeners>
            </ext:GridPanel>
          </Items>
          <Buttons>
            <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Simpan">
              <DirectEvents>
                <Click OnEvent="SaveBtn_Click">
                  <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfDONo}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
        </ext:FormPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
</ext:Window>

<uc1:DOPLCtrlPrint ID="DOPLCtrlPrint" runat="server" />
