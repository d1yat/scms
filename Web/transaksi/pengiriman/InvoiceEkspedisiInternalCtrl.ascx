<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InvoiceEkspedisiInternalCtrl.ascx.cs"
  Inherits="transaksi_InvoiceEkspedisiInternalCtrl" %>

<script type="text/javascript">
    var prepareCommands = function(rec, toolbar, valX) {
        var del = toolbar.items.get(0); // delete button
        var vd = toolbar.items.get(1); // void button

        var isNew = false;

        if (!Ext.isEmpty(rec)) {
            isNew = rec.get('l_new');
        }

        if (Ext.isEmpty(valX) || isNew) {
            del.setVisible(true);
            vd.setVisible(false);
        }
        else {
            del.setVisible(false);
            vd.setVisible(true);
        }
    }

    var recalculateFaktur = function(store) {
        var lbNet = Ext.getCmp('<%= lbNetBtm.ClientID %>');
        var lbTolParkir = Ext.getCmp('<%= lbTolBtm.ClientID %>');
        var lbBiayaBBM = Ext.getCmp('<%= lbBiayaBBM.ClientID %>');
        var txFisik = Ext.getCmp('<%= txFisikFaktur.ClientID %>');

        var sumGross = 0,
      sumTax = 0,
      sumNet = 0,
//    sumBiayaLain = 0,
      totalHarga = 0,
      vol = 0,
      berat = 0,
      tax = 0, //txTax.getValue(),
      jumlah = 0,
      biayaBBM = 0,
      BBM = lbBiayaBBM.getText(),
      biayaBBM = parseInt(BBM);
      disc = 0, //txDisc.getValue(),
      tmptol = lbTolParkir.getText().replace(".", ""),
      TolParkir = parseFloat(tmptol),

      sumGrossVol = 0,
      harga = 0,
      totalHargaVol = 0,
      sumTaxVol = 0,
      sumNetVol = 0,
      biayaLain = 0,
      divider = 0.5,
      beratRound = 0,
      volRound = 0;


        sumNet = (TolParkir + biayaBBM);

        if (!Ext.isEmpty(lbNet)) {
            lbNet.setText(myFormatNumber(sumNet));
        }
        if (!Ext.isEmpty(txFisik)) {
            txFisik.setValue(sumNet);
        }
    }

    var voidFakturInsertedDataFromStore = function(store, rec) {
        if (rec.get('l_void')) {
            return false;
        }

        if (rec.get('l_new')) {
            deleteRecordOnStore(store, rec, function(stor) {
                recalculateFaktur(stor);
            });
        }
        else {
            voidInsertedDataFromStore(rec, function(txt) {
                rec.set('l_modified', false);
                rec.set('l_void', true);
                rec.set('v_ket', txt);

                recalculateFaktur(store);
            });
        }
    }

    var checkForExistingDataInGridDetail = function(cb, rec, grid) {
        if (Ext.isEmpty(grid)) {
            return false;
        }

        var store = grid.getStore();
        if (Ext.isEmpty(store)) {
            return false;
        }

        if (Ext.isEmpty(rec)) {
            return false;
        }

        var newExp = (rec.get('c_exp')).toString().trim();
        var oldExp = cb.getValue().trim();

        if (newExp == oldExp) {
            return;
        }

        var len = store.getModifiedRecords().length;

        if (len > 0) {
            ShowWarning('Maaf, anda tidak dapat mengganti expedisi, jika telah ada data didalam grid detail.');
            return false;
        }
    }

    var afterEditDataConfirm = function(e, store) {

    if (e.record.get('l_new')) {
        recalculateFaktur(store);
        return false;
        }

        if (e.record.get('l_void')) {
            recalculateFaktur(store);
        return false;
        }
    else {
        e.record.set('l_modified', true);
        recalculateFaktur(store);
        }
    }

//Indra D. 20170426
//    var recalculateBBM = function(store, checked) {
    var recalculateBBM = function(store) {

        var lbBiayaBBM = Ext.getCmp('<%= lbBiayaBBM.ClientID %>');
        var txBBM = Ext.getCmp('<%= txBBM.ClientID %>');
        var liter = txBBM.getValue();

        var biayabbm = 0;
        
//Indra D. 20170426
        
        //        if (checked) {
        //            biayabbm = liter * 6700;
        //        }
        //        else {
        //            biayabbm = liter * 6400
        //        }

        var TpBBM = Ext.getCmp('<%= rdgBBMType.ClientID %>');
        var TipeBBM = TpBBM.getValue(); 
        
        if (TipeBBM == "01") {
            biayabbm = liter * 6700;
        }
        else if (TipeBBM == "02") {
            biayabbm = liter * 5150;
        } else {
            biayabbm = liter * 7200;
        }
//Indra D.

        lbBiayaBBM.setText((biayabbm));
        recalculateFaktur(store);
    }

    var recalculateKM = function() {

        var lbJarak = Ext.getCmp('<%= lbJarak.ClientID %>');
        var awalKM = Ext.getCmp('<%= txawalKM.ClientID %>');
        var akhirKM = Ext.getCmp('<%= txakhirKM.ClientID %>');

        var awal = awalKM.getValue(),
        akhir = akhirKM.getValue();

        var jarak = akhir - awal;

        lbJarak.setText((jarak));
    }

    var recalculateTol = function(store) {
        var lbTol = Ext.getCmp('<%= lbTolBtm.ClientID %>');
        var tol = 0,
        sumTol = 0;

        store.each(function(r) {
            if ((!r.get('l_void'))) {
                tol = r.get('n_detailtol');

                sumTol += tol;
            }
        });

        if (!Ext.isEmpty(lbTol)) {
            lbTol.setText(myFormatNumber(sumTol));
            recalculateFaktur(store);
        }
    }
    
    var storeToDetailGrid = function(frm, grid, tol) {
        if (!frm.getForm().isValid()) {
            ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
            return;
        }

        if (Ext.isEmpty(grid) ||
          Ext.isEmpty(tol.getValue())) {
            ShowWarning("Objek tidak terdefinisi.");
            return;
        }

        var store = grid.getStore();
        if (Ext.isEmpty(store)) {
            ShowWarning("Objek store tidak terdefinisi.");
            return;
        }

        var tolvar = tol.getValue();

        store.insert(0, new Ext.data.Record({
            'n_detailtol': tolvar,
            'l_new': true
        }));

        tol.reset();
        tol.focus();
    }

    var voidFakturInsertedDataFromStore2 = function(store, rec) {
        if (rec.get('l_void')) {
            return false;
        }

        if (rec.get('l_new')) {
            deleteRecordOnStore(store, rec, function(stor) {
                recalculateTol(store);
            });
        }
        else {
            voidInsertedDataFromStore(rec, function(txt) {
                rec.set('l_modified', false);
                rec.set('l_void', true);
                rec.set('v_ket', txt);

                recalculateTol(store);
            });
        }
    }

</script>

<ext:Window ID="winDetail" runat="server" Height="600" Width="825" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGdg" runat="server" />
    <ext:Hidden ID="hfIeno" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="blLayout" runat="server">
      <North MinHeight="215" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="170" AutoScroll="true"
          Layout="Fit">
          <Items>                              
            <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">                              
                  <Items>
                    <ext:DateField ID="txTglFaktur" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                      Width="100" Format="dd-MM-yyyy" />
                    <ext:TextField ID="txFakturHdr" runat="server" AllowBlank="false" FieldLabel="Invoice"
                        Width="150" />
                    <ext:NumberField ID="txFisikFaktur" runat="server" AllowBlank="false" FieldLabel="Fisik Faktur"
                        Width="150" AllowDecimals="true" AllowNegative="false" />
                    <ext:CompositeField ID="CompositeField3" runat="server" FieldLabel="KM Awal-Akhir">
                      <Items>
                         <ext:NumberField ID="txawalKM" runat="server" AllowBlank="false" AllowNegative="false" Width="75">
                            <Listeners>
                                <Change Handler="recalculateKM();" />
                            </Listeners>
                         </ext:NumberField>
                         <ext:Label ID="Label2" runat="server" Text="-"/>
                         <ext:NumberField ID="txakhirKM" runat="server" AllowBlank="false" AllowNegative="false"  Width="75">
                            <Listeners>
                                <Change Handler="recalculateKM();" />
                            </Listeners>
                         </ext:NumberField>
                         <ext:Label ID="Label1" runat="server" Text="Jarak :"/>
                         <ext:Label ID="lbJarak" runat="server" Text="0"/>
                      </Items>
                    </ext:CompositeField>
                    <ext:TextField ID="txKet" runat="server" AllowBlank="true" FieldLabel="Keterangan"
                    Width="280"  />
                  </Items>
                </ext:Panel>
                <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">                              
                  <Items>
                    <ext:NumberField ID="txBBM" runat="server" AllowBlank="false" FieldLabel="BBM(Liter)" AllowNegative="false" Width="75">
                      <Listeners>
<%--Indra D. 20170426                    --%>                      
                        <%--<Change Handler="recalculateBBM(#{gridDetail}.getStore(), #{rdgBensin}.getValue());" />--%>
                        <Change Handler="recalculateBBM(#{gridDetail}.getStore());" />
                      </Listeners>
                    </ext:NumberField>
<%--Indra D. 20170426                    --%>
<%--                    <ext:RadioGroup ID="rdgBBMType" runat="server" ColumnsNumber="1" FieldLabel="Tipe BBM">
                      <Items>
                        <ext:Radio ID="rdgBensin" runat="server" BoxLabel="Bensin" InputValue="01" />
                        <ext:Radio ID="rdgSolar" runat="server" BoxLabel="Solar" InputValue="02" Checked="true"/>
                      </Items>
                      <Listeners>
                        <Change Handler="recalculateBBM(#{gridDetail}.getStore(), #{rdgBensin}.getValue());" />
                      </Listeners>
                    </ext:RadioGroup>--%>
                    <ext:ComboBox ID="rdgBBMType" runat="server" FieldLabel="Tipe BBM" DisplayField="v_ket"
                      ValueField="c_type" Width="160" AllowBlank="false" ForceSelection="false" MinChars="3">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store6" runat="server" RemotePaging="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_portal = @0', '5', 'System.Char'],
                                              ['c_notrans = @0', '71', '']]" Mode="Raw" />
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
                        <Change Handler="recalculateBBM(#{gridDetail}.getStore());" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:Label ID="lbBiayaBBM" runat="server" Text="0" FieldLabel="Biaya BBM" />
                    <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="0"
                      AllowBlank="false">
                      <Items>
                        <ext:ListItem Value="01" Text="PDF" />
                        <ext:ListItem Value="02" Text="Excel Data Only" />
                        <ext:ListItem Value="03" Text="Excel" />
                      </Items>
                    </ext:SelectBox>
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="Panel2" runat="server" Layout="FitLayout">
          <Items>
            <ext:TabPanel ID="TabPanel1" runat="server">
              <Items>
                <ext:Panel ID="pnlGridDtl" runat="server" Title="Detail" Layout="FitLayout">
                  <TopBar>
                    <ext:Toolbar ID="Toolbar3" runat="server">
                      <Items>
                        <ext:FormPanel ID="FormPanel1" runat="server" Frame="True" Layout="Table"
                          LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                          <Items>
                      <ext:ComboBox ID="cbEP" runat="server" DisplayField="c_expno" ValueField="c_expno"
                      Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                      FieldLabel="No. EP" AllowBlank="false">
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
                            <ext:Parameter Name="model" Value="5012" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                                            ['@contains.c_expno.Contains(@0)', paramTextGetter(#{cbEP}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_expno" />
                            <ext:Parameter Name="dir" Value="DESC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_expno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_expno" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template2" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 150px">
                        <tr><td class="body-panel">No. Exp</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_expno}</td>
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
                      </Listeners>
                    </ext:ComboBox>
                    <ext:Button ID="btnAddEP" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                        Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="addBtnEP_click">
                                <ExtraParams>
                                  <ext:Parameter Name="epno" Value="#{cbEP}.getValue()" Mode="Raw" />
                                  <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>
                    </ext:Button>
                    <ext:Button ID="btnClearEP" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                        Icon="Cancel">
                        <Listeners>
                          <Click Handler="#{frmpnlDetailEntry}.getForm().reset()" />
                        </Listeners>
                    </ext:Button>
                    </Items>
                        </ext:FormPanel>
                      </Items>
                    </ext:Toolbar>
                  </TopBar>
                  <Items>
                    <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit" AutoScroll="true"> 
                    <LoadMask ShowMask="true" />
                      <SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                      </SelectionModel>
                      <Store>
                        <ext:Store ID="Store9" runat="server" RemotePaging="false" RemoteSort="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="04002" />
                            <ext:Parameter Name="sort" Value="i_urut" />
                            <ext:Parameter Name="dir" Value="DESC" />
                            <ext:Parameter Name="parameters" Value="[['ieno', paramValueGetter(#{hfIeno}), 'System.String']]"
                              Mode="Raw" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                              <Fields>
                                <ext:RecordField Name="c_resi" />
                                <ext:RecordField Name="c_expno" />
                                <ext:RecordField Name="v_cunam" />
                                <ext:RecordField Name="c_cusno" />
                                <ext:RecordField Name="n_koli" />
                                <ext:RecordField Name="n_berat" />
                                <ext:RecordField Name="n_vol" />
                                <%--<ext:RecordField Name="n_tonase" />--%>
                                <ext:RecordField Name="n_biaya" />
                                <ext:RecordField Name="n_expmin" />
                                <ext:RecordField Name="c_exptype" />
                                <ext:RecordField Name="v_ket_exptype" />
                                <ext:RecordField Name="c_via" />
                                <ext:RecordField Name="v_ket_via" />
                                <ext:RecordField Name="n_biayalain" />
                                <ext:RecordField Name="n_totalbiaya" />
                                <ext:RecordField Name="l_new" Type="Boolean" />
                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                <ext:RecordField Name="l_void" Type="Boolean" />
                                <ext:RecordField Name="i_urut" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <ColumnModel>
                        <Columns>
                          <ext:CommandColumn Width="25">
                            <Commands>
                              <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                              <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                            </Commands>
                            <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfIeno}.getValue());" />
                          </ext:CommandColumn>
                          <ext:Column DataIndex="c_resi" Header="Resi" Width="75" />
                          <ext:Column DataIndex="c_expno" Header="No. EP" Width="100" />
                          <ext:Column DataIndex="v_cunam" Header="Tujuan" Width="150" />
                          <ext:Column DataIndex="n_koli" Header="Koli" Width="50" />
                          <ext:Column DataIndex="n_berat" Header="Berat Aktual" Width="70" />
                          <ext:Column DataIndex="n_vol" Header="Berat Volume" Width="70">
                            <Editor>
                              <ext:NumberField ID="NumberField3" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="3" MinValue="0" />
                            </Editor>
                          </ext:Column>
                          <%--<ext:Column DataIndex="n_tonase" Header="Tonase" Width="50" />--%>
                          <ext:Column DataIndex="n_biaya" Header="Biaya/Kg" Width="75" />
                          <ext:Column DataIndex="n_expmin" Header="Min. Kg" Width="75" />
                          <ext:Column DataIndex="v_ket_exptype" Header="Tipe Exp" Width="75" />
                          <ext:Column DataIndex="v_ket_via" Header="Via" Width="75" />
                          <ext:Column DataIndex="n_biayalain" Header="Biaya Lain" Width="100">
                            <Editor>
                              <ext:NumberField ID="NumberField5" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                            </Editor>
                          </ext:Column>
                          <ext:Column DataIndex="n_totalbiaya" Header="Jumlah" Width="100" />
                          <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                        </Columns>
                      </ColumnModel>
                      <Listeners>
                        <Command Handler="if ((command == 'Void') || (command == 'Delete')) { voidFakturInsertedDataFromStore(this.getStore(), record); }" />
                        <AfterEdit Handler="afterEditDataConfirm(e, #{gridDetail}.getStore());" />
                      </Listeners>
                    </ext:GridPanel>
                  </Items>
                </ext:Panel>
                <ext:Panel ID="pnlGridDtlTol" runat="server" Title="Detail Tol" Layout="FitLayout">
                  <TopBar>
                    <ext:Toolbar ID="Toolbar21" runat="server">
                      <Items>
                        <ext:FormPanel ID="frmpnlDetailEntryGridDtlTol" runat="server" Frame="True" Layout="Table"
                          LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                          <Items>
                            <ext:NumberField ID="txTolDtl" runat="server" AllowBlank="true" FieldLabel="Biaya Tol" AllowNegative="false" Width="120" />
                            <ext:Button ID="btnAddTol" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                              Icon="Add">
                              <Listeners>
                                <Click Handler="storeToDetailGrid(#{frmpnlDetailEntryGridDtlTol}, #{gridDetail2}, #{txTolDtl});recalculateTol(#{gridDetail2}.getStore())" />
                              </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                              Icon="Cancel">
                              <Listeners>
                                <Click Handler="#{frmpnlDetailEntryGridDtlTol}.getForm().reset();#{cbExpedisiDtl}.enable();" />
                              </Listeners>
                            </ext:Button>
                          </Items>
                        </ext:FormPanel>
                      </Items>
                    </ext:Toolbar>
                  </TopBar>
                  <Items>
                    <ext:GridPanel ID="gridDetail2" runat="server" Layout="Fit" AutoScroll="true"> 
                    <LoadMask ShowMask="true" />
                      <SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                      </SelectionModel>
                      <Store>
                        <ext:Store ID="Store7" runat="server" RemotePaging="false" RemoteSort="false">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="04005" />
                            <ext:Parameter Name="sort" Value="" />
                            <ext:Parameter Name="dir" Value="" />
                            <ext:Parameter Name="parameters" Value="[['ieno', paramValueGetter(#{hfIeno}), 'System.String']]"
                              Mode="Raw" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                              <Fields>
                                <ext:RecordField Name="IDX" />
                                <ext:RecordField Name="c_ieno" />
                                <ext:RecordField Name="n_detailtol" />
                                <ext:RecordField Name="l_new" Type="Boolean" />
                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                <ext:RecordField Name="l_void" Type="Boolean" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <ColumnModel>
                        <Columns>
                          <ext:CommandColumn Width="25">
                            <Commands>
                              <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                              <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                            </Commands>
                            <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfIeno}.getValue());" />
                          </ext:CommandColumn>
                          <ext:Column DataIndex="n_detailtol" Header="Rincian Tol" Width="75" />
                          <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                        </Columns>
                      </ColumnModel>
                      <Listeners>
                        <Command Handler="if ((command == 'Void') || (command == 'Delete')) { voidFakturInsertedDataFromStore2(this.getStore(), record); }" />
                        <AfterEdit Handler="afterEditDataConfirm2(e, #{gridDetail2}.getStore());" />
                      </Listeners>
                    </ext:GridPanel>
                  </Items>
                </ext:Panel>
              </Items>
            </ext:TabPanel>
          </Items>
          <BottomBar>
            <ext:Toolbar ID="Toolbar1" runat="server" Layout="FitLayout">
              <Items>
                <ext:FormPanel ID="FormPanel7" runat="server" AutoScroll="true" Height="35" Layout="FitLayout">
                  <Items>
                    <ext:Panel ID="Panel3" runat="server" Padding="5" AutoScroll="true" Layout="Column">
                      <Items>
                        <ext:Panel ID="Panel4" runat="server" Border="false" Header="false" ColumnWidth=".33" Layout="Form">
                          <Items>
                            <ext:Label ID="lbTolBtm" runat="server" Text="" FieldLabel="Tol" />
                          </Items>
                        </ext:Panel>
                        <ext:Panel ID="Panel5" runat="server" Border="false" Header="false" ColumnWidth=".33" Layout="Form">
                          <Items>
                          </Items>
                        </ext:Panel>
                        <ext:Panel ID="Panel6" runat="server" Border="false" Header="false" ColumnWidth=".33" Layout="Form">
                          <Items>
                            <ext:Label ID="lbNetBtm" runat="server" Text="" FieldLabel="Net" />
                          </Items>
                        </ext:Panel>
                      </Items>
                    </ext:Panel>
                  </Items>
                </ext:FormPanel>
              </Items>
            </ext:Toolbar>
          </BottomBar>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
        <DirectEvents>
            <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                    ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
                <EventMask ShowMask="true" />
                <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="#{hfIeno}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="OutputRpt" Value="#{cbRptTypeOutput}.getValue()" Mode="Raw" />
                </ExtraParams>
            </Click>
        </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders}, #{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders}, #{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfIeno}.getValue()" Mode="Raw" />
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
            <ext:Parameter Name="gridValues2" Value="saveStoreToServer(#{gridDetail2}.getStore())" Mode="Raw" />
            <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Net" Value="#{lbNetBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="BBM" Value="#{lbBiayaBBM}.getText()" Mode="Raw" />
            <ext:Parameter Name="Tol" Value="#{lbTolBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="TipeBBM" Value="#{rdgBBMType}.getText()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
      <DirectEvents>
        <Click OnEvent="ReloadBtn_Click">
          <EventMask ShowMask="true" />
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