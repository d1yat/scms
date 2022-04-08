<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InvoiceShipmentCtrl.ascx.cs"
  Inherits="transaksi_InvoiceShipmentCtrl" %>

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
        var lbGross = Ext.getCmp('<%= lbGrossBtm.ClientID %>');
        var lbTax = Ext.getCmp('<%= lbTaxBtm.ClientID %>');
        var lbDisc = Ext.getCmp('<%= lbDiscBtm.ClientID %>');
        var lbLain = Ext.getCmp('<%= lbLainBtm.ClientID %>');
        var lbNet = Ext.getCmp('<%= lbNetBtm.ClientID %>');
        var txTax = Ext.getCmp('<%= txPajak.ClientID %>');
        var txDisc = Ext.getCmp('<%= txPotongan.ClientID %>');
        var txMaterai = Ext.getCmp('<%= txMaterai.ClientID %>');
        var lbMaterai = Ext.getCmp('<%= lbMaterai.ClientID %>');
        var lbNetVol = Ext.getCmp('<%= lbNetVolBtm.ClientID %>');

        var sumGross = 0,
      sumTax = 0,
      sumNet = 0,
      sumBiayaLain = 0,
      totalHarga = 0,
      vol = 0,
      berat = 0,
      tax = txTax.getValue(),
      jumlah = 0,
      disc = txDisc.getValue(),
      Materai = txMaterai.getValue(),
      sumGrossVol = 0,
      harga = 0,
      totalHargaVol = 0,
      sumTaxVol = 0,
      sumNetVol = 0,
      biayaLain = 0,
      divider = 0.5,
      beratRound = 0,
      volRound = 0;

        store.each(function(r) {
            if ((!r.get('l_void'))) {
                jumlah = r.get('n_totalcost');
                berat = r.get('n_berat');
                vol = r.get('n_vol');
                harga = r.get('n_biaya');
                biayaLain = r.get('n_biayalain');

                //calc berat
                if (berat % 1 == divider) {
                    beratRound = berat;
                }
                else {
                    beratRound = Math.round(berat);
                }

                sumGross += beratRound * harga;


                //calc vol
                if (vol > 0) {
                    if (vol % 1 == divider) {
                        volRound = vol;
                    }
                    else {
                        volRound = Math.round(vol);
                    }
                    sumGrossVol += volRound * harga;
                }
                else {
                    sumGrossVol += beratRound * harga;
                }

                sumBiayaLain += biayaLain;
            }
        });

        //berat
        totalHarga = (sumGross - disc);
        sumTax = (totalHarga * (tax / 100));
        sumNet = (totalHarga + sumTax + Materai + sumBiayaLain);

        //vol
        totalHargaVol = (sumGrossVol - disc);
        sumTaxVol = (totalHargaVol * (tax / 100));
        sumNetVol = (totalHargaVol + sumTax + Materai + sumBiayaLain);

        if (!Ext.isEmpty(lbGross)) {
            lbGross.setText(myFormatNumber(sumGross));
        }
        if (!Ext.isEmpty(lbDisc)) {
            lbDisc.setText(myFormatNumber(disc));
        }
        if (!Ext.isEmpty(lbTax)) {
            lbTax.setText(myFormatNumber(sumTax));
        }
        if (!Ext.isEmpty(lbLain)) {
            lbLain.setText(myFormatNumber(sumBiayaLain));
        }
        if (!Ext.isEmpty(lbNet)) {
            lbNet.setText(myFormatNumber(sumNet));
        }
        if (!Ext.isEmpty(lbMaterai)) {
            lbMaterai.setText(myFormatNumber(Materai));
        }
        if (!Ext.isEmpty(lbNetVol)) {
            lbNetVol.setText(myFormatNumber(sumNetVol));
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
        e.record.set('l_modified', true);
        recalculateFaktur(store);
    }
</script>

<ext:Window ID="winDetail" runat="server" Height="600" Width="825" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGdg" runat="server" />
    <ext:Hidden ID="hfFeno" runat="server" />
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
                    <ext:ComboBox ID="cbEksHdr" runat="server" DisplayField="v_ket" ValueField="c_exp"
                      Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                      FieldLabel="Expedisi" AllowBlank="true">
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
                            <ext:Parameter Name="model" Value="2081" />
                            <ext:Parameter Name="parameters" Value="[['c_exp != @0', '00', 'System.String'],
                              ['@contains.v_ket.Contains(@0) || @contains.c_exp.Contains(@0)', paramTextGetter(#{cbEksHdr}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_ket" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_exp" />
                                <ext:RecordField Name="v_ket" />
                                <ext:RecordField Name="l_darat" Type="Boolean" />
                                <ext:RecordField Name="l_import" Type="Boolean" />
                                <ext:RecordField Name="l_laut" Type="Boolean" />
                                <ext:RecordField Name="l_udara" Type="Boolean" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template1" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 500px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td><td class="body-panel">Udara</td><td class="body-panel">Darat</td><td class="body-panel">Laut</td><td class="body-panel">Import</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_exp}</td><td>{v_ket}</td>
                        <td>{l_udara}</td>
                        <td>{l_darat}</td>
                        <td>{l_laut}</td>
                        <td>{l_import}</td>
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
                        <BeforeSelect Handler="return checkForExistingDataInGridDetail(this, record, #{gridDetail});" />
                        <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:TextField ID="txFakturHdr" runat="server" AllowBlank="false" FieldLabel="Invoice"
                        Width="150" />
                    <ext:TextField ID="txFisikFaktur" runat="server" AllowBlank="false" FieldLabel="Fisik Faktur"
                        Width="150" />
                    <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Tanggal">
                      <Items>
                        <ext:DateField ID="txTglFaktur" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                      Width="100" Format="dd-MM-yyyy" />
                      <ext:Label ID="Label1" runat="server" Text="   T O P:" />
                        <ext:TextField ID="txTOP" runat="server" AllowBlank="false" Width="50" />
                      </Items>
                    </ext:CompositeField>
                    <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Pajak">
                      <Items>
                        <ext:NumberField ID="txPajak" runat="server" AllowBlank="false" AllowDecimals="true"
                          DecimalPrecision="2" AllowNegative="false" Width="50" MaxValue="100">
                          <Listeners>
                            <Change Handler="recalculateFaktur(#{gridDetail}.getStore());" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Label ID="lbperc" runat="server" Text="%" />
                      </Items>
                    </ext:CompositeField>
                  </Items>
                </ext:Panel>
                <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">                              
                  <Items>
                    <ext:TextField ID="txKM" runat="server" AllowBlank="false" FieldLabel="KM" Width="75" />
                    <ext:NumberField ID="txMaterai" runat="server" AllowBlank="false" FieldLabel="Materai" AllowNegative="false" Width="120">
                      <Listeners>
                        <Change Handler="recalculateFaktur(#{gridDetail}.getStore());" />
                      </Listeners>
                    </ext:NumberField>
                    <ext:CompositeField ID="CompositeField3" runat="server" FieldLabel="Potongan">
                      <Items>
                        <ext:NumberField ID="txPotongan" runat="server" AllowBlank="false" AllowNegative="false" Width="120">
                          <Listeners>
                            <Change Handler="recalculateFaktur(#{gridDetail}.getStore());" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Label ID="Label3" runat="server" Text=" - No. Claim:" />
                        <ext:TextField ID="txClaimNo" runat="server" AllowBlank="false" Width="75" />
                      </Items>
                    </ext:CompositeField>
                    <ext:TextField ID="txKet" runat="server" AllowBlank="true" FieldLabel="Keterangan"
                    Width="280"  />
                    <%--<ext:RadioGroup ID="rdgType" runat="server" ColumnsNumber="1" FieldLabel="Tipe Biaya">
                      <Items>
                        <ext:Radio ID="rdBerat" runat="server" BoxLabel="Berat" Checked="true" InputValue="01" />
                        <ext:Radio ID="rdVolume" runat="server" BoxLabel="Volume" InputValue="02" />
                      </Items>
                    </ext:RadioGroup>--%>
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:Panel runat="server" Layout="FitLayout">
          <Items>
            <ext:TabPanel runat="server">
            <TopBar>
              <ext:Toolbar ID="Toolbar2" runat="server">
                <Items>
                  <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                    LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                    <Items>
                      <ext:ComboBox ID="cbResi" runat="server" DisplayField="c_resi" ValueField="c_resi"
                      Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                      FieldLabel="Shipment/Resi" AllowBlank="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
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
                            <ext:Parameter Name="model" Value="5008" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                                                                       ['exp', #{cbEksHdr}.getValue(), 'System.String'],
                                                                       ['@contains.c_resi.Contains(@0)', paramTextGetter(#{cbResi}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_resi" />
                            <ext:Parameter Name="dir" Value="DESC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_resi" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_resi" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template3" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 150px">
                        <tr><td class="body-panel">No. Resi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_resi}</td>
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
                    <ext:SelectBox ID="cbTipeBiaya" runat="server" FieldLabel="Tipe Biaya" SelectedIndex="0"
                      AllowBlank="false">
                      <Items>
                        <ext:ListItem Value="01" Text="Berat" />
                        <ext:ListItem Value="02" Text="Volume" />
                      </Items>
                    </ext:SelectBox>
                    <ext:Button ID="btnAddResi" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                        Icon="Add">
                        <DirectEvents>
                            <Click OnEvent="AddBtnResi_Click">
                                <ExtraParams>
                                  <ext:Parameter Name="exp" Value="#{cbEksHdr}.getValue()" Mode="Raw"/>
                                  <ext:Parameter Name="resi" Value="#{cbResi}.getValue()" Mode="Raw"/>
                                  <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>
                        <%--<Listeners>
                           <Click Handler="recalculateFaktur(#{gridDetail}.getStore());" />
                        </Listeners>--%>
                      </ext:Button>
                      <ext:Button ID="btnClearWP" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
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
                <ext:Panel ID="pnlGridDtl" runat="server" Title="Detail" Layout="FitLayout">          
                  <Items>
                  <%--<ext:CenterLayout>
                  <Items>
                  <ext:ComboBox runat="server" ID="cbSupDtl" FieldLabel="Supplier" DisplayField="v_nama"
                        ValueField="c_nosup" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
                        <CustomConfig>
                          <ext:ConfigItem Name="allowBlank" Value="true" />
                        </CustomConfig>
                        <Store>
                          <ext:Store ID="Store4" runat="server" AutoLoad="false">
                            <Proxy>
                              <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                CallbackParam="soaScmsCallback" />
                            </Proxy>
                            <BaseParams>
                              <ext:Parameter Name="start" Value="={0}" />
                              <ext:Parameter Name="limit" Value="10" />
                              <ext:Parameter Name="model" Value="5003" />
                              <ext:Parameter Name="parameters" Value="[['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbSupDtl}), '']]"
                                Mode="Raw" />
                              <ext:Parameter Name="sort" Value="v_nama" />
                              <ext:Parameter Name="dir" Value="ASC" />
                            </BaseParams>
                            <Reader>
                              <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                                TotalProperty="d.totalRows">
                                <Fields>
                                  <ext:RecordField Name="c_nosup" />
                                  <ext:RecordField Name="v_nama" />
                                </Fields>
                              </ext:JsonReader>
                            </Reader>
                          </ext:Store>
                        </Store>
                        <Template ID="Template5" runat="server">
                          <Html>
                          <table cellpading="0" cellspacing="0" style="width: 500px">
                          <tr>
                          <td class="body-panel">Nama Pemasok</td>
                          </tr>
                          <tpl for="."><tr class="search-item">
                              <td>{v_nama}</td>
                          </tr></tpl>
                          </table>
                          </Html>
                        </Template>
                        <Triggers>
                          <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                        </Triggers>
                        <Listeners>
                          <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                          <Change Handler="clearRelatedComboRecursive(true, #{cbDODtl});" />
                        </Listeners>
                      </ext:ComboBox>
                      </Items>
                  </ext:CenterLayout>--%>
                    <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit" AutoScroll="true"> 
                    <LoadMask ShowMask="true" />
                          <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="Store5" runat="server" RemotePaging="false" RemoteSort="false">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="start" Value="0" />
                                <ext:Parameter Name="limit" Value="-1" />
                                <ext:Parameter Name="allQuery" Value="true" />
                                <ext:Parameter Name="model" Value="04002" />
                                <ext:Parameter Name="sort" Value="" />
                                <ext:Parameter Name="dir" Value="" />
                                <ext:Parameter Name="parameters" Value="[['feno', paramValueGetter(#{hfFeno}), 'System.String']]"
                                  Mode="Raw" />
                                <%--<ext:Parameter Name="parameters" Value="[['feno', 'fe14120005', 'System.String']]"
                                  Mode="Raw" />
                                  
                                  [['feno', '{0}', 'System.String']]", fakturId--%>
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
                                    <ext:RecordField Name="n_biaya" />
                                    <ext:RecordField Name="c_exptype" />
                                    <ext:RecordField Name="v_ket_exptype" />
                                    <ext:RecordField Name="c_via" />
                                    <ext:RecordField Name="v_ket_via" />
                                    <ext:RecordField Name="n_biayalain" />
                                    <ext:RecordField Name="n_totalcost" />
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
                                <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfFeno}.getValue());" />
                              </ext:CommandColumn>
                              <ext:Column DataIndex="c_resi" Header="Resi" Width="75" />
                              <ext:Column DataIndex="c_expno" Header="No. EP" Width="100" />
                              <ext:Column DataIndex="v_cunam" Header="Tujuan" Width="150" />
                              <ext:Column DataIndex="n_koli" Header="Koli" Width="50" />
                              <ext:Column DataIndex="n_berat" Header="Berat" Width="50" />
                              <ext:Column DataIndex="n_vol" Header="Volume" Width="50">
                                <Editor>
                                  <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                                </Editor>
                              </ext:Column>
                              <ext:Column DataIndex="n_biaya" Header="Biaya/Kg" Width="75" />
                              <ext:Column DataIndex="v_ket_exptype" Header="Tipe Exp" Width="75" />
                              <ext:Column DataIndex="v_ket_via" Header="Via" Width="75" />
                              <ext:Column DataIndex="n_biayalain" Header="Biaya Lain" Width="100" />
                              <ext:Column DataIndex="n_totalcost" Header="Jumlah" Width="100" />
                              <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                            </Columns>
                          </ColumnModel>
                          <Listeners>
                            <Command Handler="if ((command == 'Void') || (command == 'Delete')) { voidFakturInsertedDataFromStore(this.getStore(), record); }" />
                            <AfterEdit Handler="afterEditDataConfirm(e, #{gridDetail}.getStore());" />
                            <%--<ContextMenu Handler="testingsdfg(#{gridDetail});" />--%>
                            <%--<RowBodyContextMenu Handler="this.getSelectionModel();" />--%>
                          </Listeners>
                          <%--<DirectEvents>
                            <Command OnEvent="GridDetailCommand" >
                              <EventMask ShowMask="true" />
                              <ExtraParams>
                                <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                                <ext:Parameter Name="PrimaryID" Value="record.data.c_dono" Mode="Raw" />
                              </ExtraParams>
                            </Command>
                          </DirectEvents>--%>
                        </ext:GridPanel>
                  </Items>
                </ext:Panel>
              </Items>
            </ext:TabPanel>
          </Items>
          <BottomBar>
            <ext:Toolbar runat="server" Layout="FitLayout">
              <Items>
                <ext:FormPanel runat="server" AutoScroll="true" Height="80" Layout="FitLayout">
                  <Items>
                    <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
                      <Items>
                        <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".33" Layout="Form">
                          <Items>
                            <ext:Label ID="lbGrossBtm" runat="server" Text="" FieldLabel="Gross" />
                            <ext:Label ID="lbLainBtm" runat="server" Text="" FieldLabel="Biaya Lain-lain" />
                          </Items>
                        </ext:Panel>
                        <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".33" Layout="Form">
                          <Items>
                            <ext:Label ID="lbDiscBtm" runat="server" Text="" FieldLabel="Potongan" />
                            <ext:Label ID="lbMaterai" runat="server" Text="" FieldLabel="Materai" />
                          </Items>
                        </ext:Panel>
                        <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" ColumnWidth=".33" Layout="Form">
                          <Items>
                            <ext:Label ID="lbTaxBtm" runat="server" Text="" FieldLabel="Pajak" />
                            <ext:Label ID="lbNetBtm" runat="server" Text="" FieldLabel="Net Berat" />
                            <ext:Label ID="lbNetVolBtm" runat="server" Text="" FieldLabel="Net" />
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
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders}, #{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders}, #{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfFeno}.getValue()" Mode="Raw" />
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
            <%--<ext:Parameter Name="Ekspedisi" Value="#{cbEksHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Faktur" Value="#{txFakturHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="FisikFaktur" Value="#{txFisikFaktur}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TglFaktur" Value="#{txTglFaktur}.getRawValue()" Mode="Raw" />
            <ext:Parameter Name="TOP" Value="#{txTOP}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Pajak" Value="#{txPajak}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Ket" Value="#{txKet}.getValue()" Mode="Raw" />
            <ext:Parameter Name="KM" Value="#{txKM}.getValue()" Mode="Raw" />--%>
            <ext:Parameter Name="Gross" Value="#{lbGrossBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="Pajak" Value="#{lbTaxBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="TotalBiayaLain" Value="#{lbLainBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="NetBerat" Value="#{lbNetBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="NetVol" Value="#{lbNetVolBtm}.getText()" Mode="Raw" />
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
