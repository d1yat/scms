<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Proses_PharmanetCtrl.ascx.cs"
  Inherits="transaksi_pembelian_ProsesPharmanetCtrl" %>

<script type="text/javascript">

    var setDefaultAcc = function(chkDefault, store, chkDeclined) {
        var chk = chkDefault.getValue();

        if (chk) {
            store.each(function(r) {

                r.set('n_qtyterima', r.get('n_qty'));
                r.set('nv_batchterima', r.get('c_batch'));
                r.set('d_expiredterima', r.get('d_expired'));
                //                r.set('c_type', '02');
                //                r.set('v_jenisSP', 'Accepted');
                //                r.set('l_modified', true);
            });
            chkDeclined.setValue(false);
        }
        else {
            store.each(function(r) {
                r.set('n_qtyterima', 0);
                r.set('nv_batchterima', 0);
                r.set('d_expiredterima', r.get('d_expired'));
                //                r.set('c_type', '01');
                //                r.set('v_jenisSP', 'Pending');
                //                r.set('l_modified', false);
            });
        }
    }




    var setDefaultDecl = function(chkDefault, store, chkAccepted) {
        var chk = chkDefault.getValue();

        if (chk) {
            store.each(function(r) {
            r.set('n_qtyterima', 0);
//                r.set('c_type', '03');
//                r.set('v_jenisSP', 'Rejected');
//                r.set('l_modified', true);
            });
            chkAccepted.setValue(false);
        }
        else {
            store.each(function(r) {
            r.set('n_qtyterima', 0);
//                r.set('c_type', '01');
//                r.set('v_jenisSP', 'Pending');
//                r.set('l_modified', false);
            });
        }
    }
   
    
    

//    var onGridBeforeEdit = function(e) {
//    if (e.field == 'n_qty') {
//            e.cancel = false;
//        }
//        else if (e.field == 'n_qty') {
//        if (!e.record.get('n_qty')) {
//                e.cancel = true;
//            }
//        }
//        else {
//            e.cancel = true;
//        }
//    }




    var onGridAfterEdit = function(e, btnProses) 
    {


        var prevValue = 0;
        ////
        if (e.field == 'n_qtyterima') 
        
        {
            //awal

            //ShowWarning('Jumlah');

           

            if (e.record.get('n_qtyterima')) {
                prevValue = (e.record.get('n_qty') || 0);
                if ((prevValue != 0) && (e.value > prevValue)) {
                    e.record.set('n_qty', prevValue);
                    ShowWarning('Jumlah qty terima tidak boleh lebih besar dari ketersedian data.');

                    btnProses.setDisabled(true);
                }


                ////////
                else if ((prevValue == 0) && (e.value > e.originalValue)) {
                    e.record.reject();

                    ShowWarning('Jumlah quantity tidak boleh lebih besar dari ketersedian data.');
                }
                /////////////

                else {
                    e.record.set('n_qtyterima', e.value);
                    
                }

            }
            //akhir

        }
        ////


        //if 2 awal
        if (e.field == 'n_qtyterima') {
            //awal

            //ShowWarning('Jumlah');



            if (e.record.get('n_qtyterima')) {
                prevValue = (e.record.get('n_qty') || 0);
                if ((prevValue != 0) && (e.value > prevValue)) {
                    e.record.set('n_qty', prevValue);
                    ShowWarning('Jumlah qty terima tidak boleh lebih besar dari ketersedian data.');

                    btnProses.setDisabled(true);
                }


                ////////
                else if ((prevValue == 0) && (e.value > e.originalValue)) {
                    e.record.reject();

                    ShowWarning('Jumlah quantity tidak boleh lebih besar dari ketersedian data.');
                }
                /////////////

                else {
                    e.record.set('n_qtyterima', e.value);

                }

            }
            //akhir

        }
        //if 2 akhir
    }


  
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="980" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="725" Layout="Fit">
  <Content>
    <%--<ext:Hidden ID="hfSpNo" runat="server" />--%>
    <ext:Hidden ID="hfPoOutlet" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeNameCtrl" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="175" MaxHeight="175" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="195" Padding="10">
          <Items>
             <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                      ValueField="c_cusno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3" AllowBlank="false" ForceSelection="false">
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
                            <ext:Parameter Name="model" Value="3001" />
                            <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
                              Mode="Raw" />
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
                                <ext:RecordField Name="c_gdg_cab" />
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
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbPrincipalHdr});" />
                      </Listeners>
                    </ext:ComboBox>
                   <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
                      ValueField="c_nosup" Width="250" ItemSelector="tr.search-item" ListWidth="350"
                      MinChars="3" AllowBlank="false" ForceSelection="false">
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
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="model" Value="0024" />
                            <ext:Parameter Name="parameters" Value="[['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                       
                        ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]"
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
                      <Template ID="Template2" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="0" style="width: 350px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_nosup}</td><td>{v_nama}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <BeforeSelect Handler="return checkForExistingDataInGridDetail(this, record, #{gridDetail});" />
                        <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    
                  
                                       
            <ext:CompositeField runat="server" FieldLabel="Tanggal">
              <Items>
                <ext:DateField ID="txTanggal" runat="server" AllowBlank="false" Format="dd-MM-yyyy"
                  EnableKeyEvents="true" />
              </Items>
            </ext:CompositeField>
            
           <ext:TextField ID="txSpCabang" runat="server" AllowBlank="false" FieldLabel="Nomor Do"
              MaxLength="10" Width="200" />
              
                       
           <ext:TextField ID="txNoPL" runat="server" AllowBlank="false" FieldLabel="Nomor PL"
              MaxLength="10" Width="200" />
              
           <ext:TextField ID="txKeterangan" runat="server" FieldLabel="Keterangan" MaxLength="100"
              Width="250" />
              
         
            <%--<ext:Checkbox ID="chkCheck" runat="server" FieldLabel="Periksa" />--%>
            
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
        
        
        <TopBar>
              <ext:Toolbar ID="Toolbar1" runat="server">
                  <Items>
                       <ext:Checkbox runat="server" ID="chkAllAcc" FieldLabel="Set All Accepted ">
                             <Listeners>
                                 <Check Handler="setDefaultAcc(this, #{gridDetail}.getStore(),#{chkAllDec});" />
                             </Listeners>
                        </ext:Checkbox>
                 <%--<ext:ToolbarSeparator />
                        <ext:Checkbox runat="server" ID="chkAllDec" FieldLabel="Set All Rejected ">
                               <Listeners>
                                   <Check Handler="setDefaultDecl(this, #{gridDetail}.getStore(),#{chkAllAcc});" />
                               </Listeners>
                        </ext:Checkbox>
                 <ext:ToolbarSeparator />--%>
                                                                                      
                                            
                 </Items>
            </ext:Toolbar>
      </TopBar>
         
         
         <%--hafizh awal--%>
         
         
         <%--hafizh akhir--%>
         
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="2500" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_po_outlet', #{hfPoOutlet}.getValue(), 'System.String'],
                    ['c_nosup = @0', paramValueGetter(#{cbSuplierFltr}) , 'System.String']
                    
                    ]"
                      Mode="Raw" />
                  </BaseParams>
                  <Reader>
                     <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itnam" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />       
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="n_dprion" Type="Float" />
                        <ext:RecordField Name="n_dprioff" Type="Float" />
                        <ext:RecordField Name="Proses" />
                        <ext:RecordField Name="v_ket" />
                        <ext:RecordField Name="n_qtyterima" Type="Float" /> 
                        <ext:RecordField Name="nv_batchterima" />
                        <ext:RecordField Name="d_expiredterima" Type="Date" DateFormat="M$" />
                                            
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                
                
                  
                  <ext:Column ColumnID="c_iteno" DataIndex="c_iteno" Header="Kode barang" Width="80"  />
                  <ext:Column ColumnID="v_itnam" DataIndex="v_itnam" Header="Nama barang" Width="80" />
                  <ext:Column ColumnID="v_undes" DataIndex="v_undes" Header="Kemasan" Width="80" />
                  <ext:Column ColumnID="c_batch" DataIndex="c_batch" Header="Batch" Width="80" />
                  <ext:DateColumn ColumnID="d_expired" DataIndex="d_expired" Header="Tgl Exp" Format="dd-MM-yyyy" />
                  <ext:NumberColumn ColumnID="n_qty" DataIndex="n_qty" Header="Qty" Format="0.000,00/i" Width="100" />
                  <ext:NumberColumn ColumnID="n_dprion" DataIndex="n_dprion" Header="Discount On" Format="0.000,00/i" Width="100" />
                  <ext:NumberColumn ColumnID="n_dprioff" DataIndex="n_dprioff" Header="Discount Off" Format="0.000,00/i" Width="100" />
                  <ext:Column ColumnID="Proses" DataIndex="Proses" Header="Proses" Width="80" />
                  
                              
                  <ext:NumberColumn ColumnID="n_qtyterima" DataIndex="n_qtyterima" Header="Qty DiTerima" Format="0.000,00/i"   Width="75">
                                  <Editor>
                                      <ext:NumberField ID="NumberField2" runat="server" AllowDecimals="true" AllowNegative="false"
                                       MinValue="0" DecimalPrecision="2" />
                                  </Editor>
                                  
                                  
                                  
                  </ext:NumberColumn>
                  
                  
                  
                   <ext:Column DataIndex="nv_batchterima" Header="Batch Diterima" Width="150">
                      <Editor>
                        <ext:TextField ID="TextField3" runat="server" AllowBlank="true" />
                      </Editor>
                  </ext:Column>
                  <ext:DateColumn DataIndex="d_expiredterima" Header="Batch Expired Diterima" Width="150" Format="dd-MM-yyyy">
                      <Editor>
                        <ext:DateField ID="dfTglterima" runat="server" AllowBlank="false" Format="dd-MM-yyyy" />
                      </Editor>
                  </ext:DateColumn>
                  
                                     
                  <ext:Column DataIndex="v_ket" Header="Keterangan" Width="150">
                      <Editor>
                        <ext:TextField ID="txtField1" runat="server" AllowBlank="true" />
                      </Editor>
                  </ext:Column>
                  
                  
                                  
                </Columns>
              </ColumnModel>
              
                             
              <Listeners>
                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); } else if (command == 'Reset') { resetSisaSPQtyCmd(record); }" />
                <%--<Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } " />--%>
               <%--<BeforeEdit Fn="onGridBeforeEdit" />--%> 
               <AfterEdit Fn="onGridAfterEdit" />
               <%-- <AfterEdit Handler="afterEditDataConfirm(e, #{gridDetail}.getStore());" />--%>
                
              </Listeners>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
      <%--<South MinHeight="80" MaxHeight="80">
      </South>--%>
    </ext:BorderLayout>
  </Items>
  <Buttons>
  
  
   <ext:Button ID="btnVerifikasi" runat="server" Icon="Disk" Text="Kirim Email ke Pharmanet">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Kirim Email" Message="Anda yakin ingin kirim email atas PL ini?" />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:parameter name="CustomerID" value="#{cbCustomerHdr}.getValue()" mode="raw"/>
            <ext:parameter name="NomorPL" value="#{txNoPL}.getValue()" mode="raw" />
            <ext:Parameter Name="POoutlet" Value="#{hfPoOutlet}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
   
    <ext:Button ID="btnProses"  runat="server" Icon="Disk" Text="Proses">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini?" />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="CustomerID" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="POoutlet" Value="#{hfPoOutlet}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Keterangan" Value="#{txKeterangan}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Cancel">
      <DirectEvents>
        <Click OnEvent="ReloadBtn_Click">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Cancel ?" Message="Anda yakin ingin cancel data ini?" />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="POoutlet" Value="#{hfPoOutlet}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
