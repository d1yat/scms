<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InvoiceEkspedisiEksternalCtrl.ascx.cs"
  Inherits="transaksi_InvoiceEkspedisiEksternalCtrl" %>

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
        var lbLain = Ext.getCmp('<%= lbLainBtm.ClientID %>');
        var lbNet = Ext.getCmp('<%= lbNetBtm.ClientID %>');
        var txTax = Ext.getCmp('<%= txPajak.ClientID %>');
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
      Materai = txMaterai.getValue(),
      sumGrossVol = 0,
      harga = 0,
      totalHargaVol = 0,
      sumTaxVol = 0,
      sumNetVol = 0,
      biayaLain = 0,
      divider = 0.5,
      beratAktual = 0,
      volAktual = 0,
      via = null,
      expmin = 0,
      tonase = 0,
      loop = 0,
      totaltonase = 0;

        //D. Indra 20170216
        if (Materai == "") {
            Materai = 0;
        }

        var arrResi = new Array();
        var arrVia = new Array();


        store.each(function(r) {
            if ((!r.get('l_void'))) {
                berat = r.get('n_berat');
                vol = r.get('n_vol');
                harga = r.get('n_biaya');
                biayaLain = r.get('n_biayalain');
                via = r.get('c_via');
                resi = r.get('c_resi');
                expmin = r.get('n_expmin');
                var isNew = r.get('l_new');
                tonase = r.get('n_tonase');
                var storeaaa = store;

                if (via == "04" || via == "05" || via == "06" || via == "07" || via == "08" || via == "09") {

                    sumGross += harga;
                    sumGrossVol += harga;
                    //alert('test');

                    //                    if (arrResi.indexOf(resi) >= 0 && arrVia.indexOf(via) >= 0) {
                    //                        arrResi[loop] = '';
                    //                        arrVia[loop] = '';
                    //                        loop += 1;
                    //                    }
                    //                    else {
                    //                        sumGross += harga;
                    //                        sumGrossVol += harga;
                    //                        arrResi[loop] = resi;
                    //                        arrVia[loop] = via;
                    //                        loop += 1;
                    //                    }
                }
                else {
                    //calc berat
                    //                    if (expmin > 0 && berat < expmin) {
                    //                        beratAktual = expmin;
                    //                    }
                    //                    else {
                    //                        beratAktual = berat;
                    //                    }
                    //alert(beratAktual);
                    if (tonase > 0) {
                        sumGrossVol += Math.round(tonase * harga);
                        r.set('n_totalbiaya', Math.round(tonase * harga));
                        //alert('test 1');
                    }
                    else {
                        if (vol > 0) {
                            sumGrossVol += Math.round(vol * harga);
                            r.set('n_totalbiaya', Math.round(vol * harga));
                            //alert('test 2');
                        }
                        else {
                            if (beratAktual > 0) {
                                sumGrossVol += Math.round(beratAktual * harga);
                                r.set('n_totalbiaya', Math.round(beratAktual * harga));
                                //alert('test 3');
                            }
                        }
                    }

                    //calc vol
                    //                    if (vol > 0) {
                    //                        //                        tonase = vol / 6000 * 1000000;
                    //                                            if (expmin > 0 && vol < expmin) {
                    //                                                volAktual = expmin;
                    //                                            }
                    //                                            else {
                    //                                                volAktual = vol;
                    //                                            }
                    //                                            if (tonase != 0) {
                    //                                                sumGrossVol += Math.round(tonase * harga);
                    //                                                r.set('n_totalbiaya', Math.round(tonase * harga));
                    //                                            }
                    //                                            else {
                    //                                                sumGrossVol += Math.round(volAktual * harga);
                    //                                                //                        r.set('n_vol', volAktual);
                    //                                                r.set('n_totalbiaya', Math.round(volAktual * harga));
                    //                                            }
                    //                                        }
                    //                                        else {
                    //                                            sumGrossVol += Math.round(beratAktual * harga);
                    //                                            r.set('n_vol', 0);
                    //                                            r.set('n_totalbiaya', Math.round(beratAktual * harga));
                    //                                        }


                    arrResi[loop] = '';
                    arrVia[loop] = '';
                    loop += 1;
                }
                //                loop++;
                sumBiayaLain += biayaLain;
                totaltonase += tonase;
                //                r.set('n_totalbiaya', sumGross);
            }
        });

        //berat
        totalHarga = (sumGross);
        sumTax = ((totalHarga + sumBiayaLain) * (tax / 100));
        sumNet = (totalHarga + sumTax + Materai + sumBiayaLain);

        //vol
        totalHargaVol = (sumGrossVol);

        //tonase

        //Indra D. 20170214
        //sumTaxVol = ((totalHargaVol + sumBiayaLain) * (tax / 100));
        sumTaxVol = ((totalHargaVol + sumBiayaLain) * (tax / 100));
        sumNetVol = (totalHargaVol + sumTaxVol + Materai + sumBiayaLain);

        if (!Ext.isEmpty(lbGross)) {
            lbGross.setText(myFormatNumber(sumGrossVol));
        }
        if (!Ext.isEmpty(lbTax)) {
            lbTax.setText(myFormatNumber(sumTaxVol));
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

    var recalculateClaim = function(store) {
        var lbDisc = Ext.getCmp('<%= lbDiscBtm.ClientID %>');
        var claimno,
      potongan = 0,
      sumPotongan = 0;

        store.each(function(r) {
            if ((!r.get('l_void'))) {
                potongan = r.get('n_disc');

                sumPotongan += potongan;
                }
        });
        
        if (!Ext.isEmpty(lbDisc)) {
            lbDisc.setText(myFormatNumber(sumPotongan));
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

    var voidFakturInsertedDataFromStore2 = function(store, rec) {
        if (rec.get('l_void')) {
            return false;
        }

        if (rec.get('l_new')) {
            deleteRecordOnStore(store, rec, function(stor) {
            recalculateClaim(store);
            });
        }
        else {
            voidInsertedDataFromStore(rec, function(txt) {
                rec.set('l_modified', false);
                rec.set('l_void', true);
                rec.set('v_ket', txt);

                recalculateClaim(store);
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
        var tonase = e.record.get('n_tonase');
        var biaya = e.record.get('n_biaya');
        var totalbiaya = 0;

        if (e.record.get('l_new')) {
            recalculateFaktur(store);
            return false;
        }

        if (e.record.get('l_void')) {
            return false;
        }
        else {
            e.record.set('l_modified', true);
        }

        recalculateFaktur(store);
//        if (tonase != 0) {
//            totalbiaya = tonase * biaya;
//            e.record.set('n_totalbiaya', totalbiaya);
//        }
    }

    var afterEditDataConfirm2 = function(e, store) {

        if (e.record.get('l_new')) {
            recalculateFaktur(store);
            return false;
        }

        if (e.record.get('l_void')) {
            return false;
        }
        else {
            e.record.set('l_modified', true);
        }

        recalculateClaim(store);
    }

    var storeToDetailGrid = function(frm, grid, claimno, potongan) {
        if (!frm.getForm().isValid()) {
            ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
            return;
        }

        if (Ext.isEmpty(grid) ||
          Ext.isEmpty(claimno.getValue()) ||
          Ext.isEmpty(potongan.getValue())) {
            ShowWarning("Objek tidak terdefinisi.");
            return;
        }

        var store = grid.getStore();
        if (Ext.isEmpty(store)) {
            ShowWarning("Objek store tidak terdefinisi.");
            return;
        }

        var claimNo = claimno.getValue();

        var Potongan = potongan.getValue();

        var valX = [claimNo];
        var fieldX = ['c_claimno'];

        // Find Duplicate entry
        var isDup = findDuplicateEntryGrid(store, fieldX, valX);

        if (!isDup) {
            //            var recItem = claimno.findRecord(claimno.valueField, claimNo);
            //            var suplName = (Ext.isEmpty(recItem) ? '' : recItem.get('v_nama'));
            //            var NamaVia = tipe.getText();


            store.insert(0, new Ext.data.Record({

                'c_claimno': claimNo,
                'n_disc': Potongan,
                'l_new': true
            }));

            claimno.reset();
            potongan.reset();
            claimno.focus();
        }
        else {
            ShowError('Data telah ada.');

            return false;
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
                    <ext:ComboBox ID="cbEksHdr" runat="server" DisplayField="v_ket" ValueField="c_exp"
                      Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                      FieldLabel="Nama Expedisi" AllowBlank="true">
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
                    <ext:TextField ID="txFakturHdr" runat="server" AllowBlank="false" FieldLabel="No. Invoice"
                        Width="150" />
                    <ext:NumberField ID="txFisikFaktur" runat="server" AllowBlank="false" FieldLabel="Nilai Invoice"
                        Width="150" AllowDecimals="true" AllowNegative="false" />
                    <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Tanggal">
                      <Items>
                        <ext:DateField ID="txTglFaktur" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                      Width="100" Format="dd-MM-yyyy" />
                      <ext:Label ID="Label1" runat="server" Text="   T O P:" />
                        <ext:TextField ID="txTOP" runat="server" AllowBlank="false" Width="50" />
                        <ext:Label ID="lblhari" runat="server" Text=" Hari" />
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
                    <%--<ext:TextField ID="txKM" runat="server" AllowBlank="false" FieldLabel="KM" Width="75" />--%>
                    <ext:NumberField ID="txMaterai" runat="server" AllowBlank="false" FieldLabel="Materai" AllowNegative="false" Width="120">
                      <Listeners>
                        <Change Handler="recalculateFaktur(#{gridDetail}.getStore());" />
                      </Listeners>
                    </ext:NumberField>
                    <ext:TextField ID="txKet" runat="server" AllowBlank="true" FieldLabel="Keterangan"
                    Width="280"  />
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
        <ext:Panel runat="server" Layout="FitLayout">
          <Items>
            <ext:TabPanel runat="server">
              <Items>
                <ext:Panel ID="pnlGridDtl" runat="server" Title="Detail" Layout="FitLayout">
                  <TopBar>
                    <ext:Toolbar ID="Toolbar3" runat="server">
                      <Items>
                        <ext:FormPanel ID="FormPanel1" runat="server" Frame="True" Layout="Table"
                          LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                          <Items>
                      <ext:ComboBox ID="cbResi" runat="server" DisplayField="c_resi" ValueField="c_resi"
                      Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                      FieldLabel="Shipment/Resi" AllowBlank="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
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
                      <Template ID="Template4" runat="server">
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
                    <%--<ext:SelectBox ID="cbTipeBiaya" runat="server" FieldLabel="Tipe Biaya" SelectedIndex="0"
                      AllowBlank="false">
                      <Items>
                        <ext:ListItem Value="01" Text="Berat" />
                        <ext:ListItem Value="02" Text="Volume" />
                      </Items>
                    </ext:SelectBox>--%>
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
                    </ext:Button>
                    <ext:Button ID="btnResetResi" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                        Icon="Cancel">
                        <Listeners>
                          <Click Handler="#{frmpnlDetailEntry}.getForm().reset()" />
                        </Listeners>
                    </ext:Button>
                    <ext:ComboBox ID="cbEP" runat="server" DisplayField="c_expno" ValueField="c_expno"
                      Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
                      FieldLabel="Ekspedisi" AllowBlank="false">
                      <CustomConfig>
                        <ext:ConfigItem Name="allowBlank" Value="false" />
                      </CustomConfig>
                      <Store>
                        <ext:Store ID="Store8" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="5011" />
                            <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                                                                       ['exp', #{cbEksHdr}.getValue(), 'System.String'],
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
                      <Template ID="Template5" runat="server">
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
                            <Click OnEvent="AddBtnEP_Click">
                                <ExtraParams>
                                  <ext:Parameter Name="exp" Value="#{cbEksHdr}.getValue()" Mode="Raw"/>
                                  <ext:Parameter Name="expno" Value="#{cbEP}.getValue()" Mode="Raw"/>
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
                                <ext:RecordField Name="n_tonase" />
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
                          <ext:Column DataIndex="n_vol" Header="Berat Volume" Width="70" />
                          <ext:Column DataIndex="n_tonase" Header="Berat di tagih" Width="50">
                            <Editor>
                              <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="3" MinValue="0" />
                            </Editor>
                          </ext:Column>
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
                <ext:Panel ID="pnlGridDtlClaim" runat="server" Title="Claim" Layout="FitLayout">
                  <TopBar>
                    <ext:Toolbar ID="Toolbar1" runat="server">
                      <Items>
                        <ext:FormPanel ID="frmpnlDetailEntryGridDtlClaim" runat="server" Frame="True" Layout="Table"
                          LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                          <Items>
                            <ext:TextField ID="txClaimDtl" runat="server" FieldLabel="No. Claim" AllowBlank="true" Width="75" />
                            <ext:NumberField ID="txPotonganDtl" runat="server" AllowBlank="true" FieldLabel="Potongan" AllowNegative="false" Width="120" />
                            <ext:Button ID="btnAddClaim" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                              Icon="Add">
                              <Listeners>
                                <Click Handler="storeToDetailGrid(#{frmpnlDetailEntryGridDtlClaim}, #{gridDetail2}, #{txClaimDtl}, #{txPotonganDtl});recalculateClaim(#{gridDetail2}.getStore())" />
                                <%--<Click Handler="recalculateClaim(#{gridDetail2}.getStore());" />--%>
                              </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                              Icon="Cancel">
                              <Listeners>
                                <Click Handler="#{frmpnlDetailEntryGridDtlClaim}.getForm().reset();#{cbExpedisiDtl}.enable();" />
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
                            <ext:Parameter Name="model" Value="04004" />
                            <ext:Parameter Name="sort" Value="" />
                            <ext:Parameter Name="dir" Value="" />
                            <ext:Parameter Name="parameters" Value="[['ieno', paramValueGetter(#{hfIeno}), 'System.String']]"
                              Mode="Raw" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                              <Fields>
                                <ext:RecordField Name="c_claimno" />
                                <ext:RecordField Name="n_disc" />
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
                          <ext:Column DataIndex="c_claimno" Header="Claim" Width="100" />
                          <ext:Column DataIndex="n_disc" Header="Potongan" Width="75" />
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
            <ext:Toolbar runat="server" Layout="FitLayout">
              <Items>
                <ext:FormPanel runat="server" AutoScroll="true" Height="80" Layout="FitLayout">
                  <Items>
                    <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
                      <Items>
                        <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".33" Layout="Form">
                          <Items>
                            <ext:Label ID="lbGrossBtm" runat="server" Text="" FieldLabel="Gross" Hidden="true" />
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
                            <ext:Label ID="lbNetBtm" runat="server" Text="" FieldLabel="Net Berat" Hidden="true" />
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
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="gridValues2" Value="saveStoreToServer(#{gridDetail2}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Gross" Value="#{lbGrossBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="Pajak" Value="#{lbTaxBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="TotalBiayaLain" Value="#{lbLainBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="NetBerat" Value="#{lbNetBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="NetVol" Value="#{lbNetVolBtm}.getText()" Mode="Raw" />
            <ext:Parameter Name="totalPotongan" Value="#{lbDiscBtm}.getText()" Mode="Raw" />
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
