<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FakturEkspedisiCtrl.ascx.cs"
    Inherits="transaksi_FakturEkspedisiCtrl" %>

<script type="text/javascript">
    var prepareCommandsDetilBerat = function(rec, toolbar, valX) {
        var del = toolbar.items.get(0); // delete button
        //var vd = toolbar.items.get(1); // void button

        var isNew = false;

        if (!Ext.isEmpty(rec)) {
            isNew = rec.get('l_new');
        }

        if (Ext.isEmpty(valX) || isNew) {
            del.setVisible(false);
            //        vd.setVisible(false);
        }
        else {
            del.setVisible(true);
            //vd.setVisible(true);
        }
    }

    var selectedEksp = function(rec, npwp) {
        var isNpwp = rec.get('l_npwp');

        try {
            npwp.setValue(isNpwp);        
        }
        catch (e) {
            ShowError(e.toString());
        }
    }
    
    var selectedInvoice = function(combo, rec, nilai, sisa, nilaiFisik, faFisik, maxValue, claim, nilaiShipment) {
        if (Ext.isEmpty(nilai)) {
            ShowWarning("Objek target tidak terdefinisi.");
            return;
        }

        var nilaiSisa = rec.get('n_netsisa');
        var nFisik = rec.get('n_bilva_faktur');
        var sFaFisik = rec.get('c_ie');
        var nClaim = rec.get('n_disc');
        var nShipment = rec.get('n_totalbiaya');

        try {
            nilai.setMinValue(0);

            if (Ext.isNumber(nilaiSisa)) {
                nilai.setMaxValue(nilaiSisa);
            }
            else {
                nilai.setMaxValue(Number.MAX_VALUE);
            }

            if (Ext.isNumber(nilaiSisa)) {
                nilai.setValue(nilaiSisa);
//                nilai.setValue(nFisik);
                sisa.setValue(0);
                nilaiFisik.setValue(nFisik);
                faFisik.setValue(sFaFisik);
                maxValue.setValue(nilaiSisa);
                claim.setValue(nClaim);
                nilaiShipment.setValue(nShipment);
            }
        }
        catch (e) {
            ShowError(e.toString());
        }
    }

    var storeToDetailGrid = function(frm, grid, faktur, fakturFisik, nilai, nilaiFisik, totInv, totAms, totSelisih, totNilai, sisa, claim, totClaim, nilaiShipment, nilaiLain, pinalty, isNpwp, totPph) {
        if (!frm.getForm().isValid()) {
            ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
            return;
        }

        if (Ext.isEmpty(grid) ||
          Ext.isEmpty(nilai) ||
          Ext.isEmpty(nilaiFisik) ||
          Ext.isEmpty(faktur) ||
          Ext.isEmpty(fakturFisik)) {
            ShowWarning("Objek tidak terdefinisi.");
            return;
        }

        if (nilai.getValue() == "") {
            if (nilai.getValue() == "0") { }
            else {
                ShowWarning("Inputan nilai kosong");
                return;
            }
        }

        var store = grid.getStore();

        if (Ext.isEmpty(store)) {
            ShowWarning("Objek store tidak terdefinisi.");
            return;
        }

        var i = 0;
        var isCheck = false;
        var nNilaiInv = 0;
        var nNilaiAms = 0;
        var nSelisih = 0;
        var nClaim = 0;
        var nPinalty = 0;
        var nLain = 0;
        var nTotalInv = 0;
        var nTotalAms = 0;
        var nTotalSelisih = 0;
        var nTotalBayar = 0;
        var nTotalClaim = 0;
        var nPph = 0;
        var nTotalPph = 0;

        var valX = [faktur.getValue()];
        var fieldX = ['c_ieno'];

        var isDup = findDuplicateEntryGrid(store, fieldX, valX);
        if (!isDup) {
            var c_ieno = faktur.getValue();
            if (c_ieno.length != 10) {
                ShowWarning("No tidak terdefinisi.");
                return false;
            }

            var sFaktur = fakturFisik;
            //            nNilaiInv = parseFloat(nilaiFisik);
            nNilaiInv = nilai.getValue();
            //            nNilaiAms = nilai.getValue();
            nNilaiAms = parseFloat(nilaiShipment);
            nSelisih = nNilaiInv - nNilaiAms;
            nClaim = parseFloat(claim.getValue());
            nPinalty = parseFloat(pinalty.getValue());
            nLain = parseFloat(nilaiLain.getValue());

            if (isNpwp == "true") {
                nPph = 0.02 * nNilaiInv
            }
            else {
                nPph = 0.04 * nNilaiInv
            }
            
            nTotalInv = totInv.getValue() + nNilaiInv;
            nTotalAms = totAms.getValue() + nNilaiAms;
            nTotalSelisih = nTotalInv - nTotalAms;
            nTotalClaim = totClaim.getValue() + nClaim;
            nTotalPph = totPph.getValue() + nPph;
            //            nTotalBayar = nTotalInv - (nTotalClaim + nPinalty) + nLain;
            //            nTotalBayar = (totNilai.getValue() + nilai.getValue()) - (nClaim + nPinalty) + nLain;
            nTotalBayar = (totNilai.getValue() + nilai.getValue()) - (nClaim + nPph);

            totInv.setValue(nTotalInv);
            totAms.setValue(nTotalAms);
            totSelisih.setValue(nTotalSelisih);
            totNilai.setValue(nTotalBayar);
            totClaim.setValue(nTotalClaim);
            totPph.setValue(nTotalPph);

            store.insert(0, new Ext.data.Record({
                'c_ieno': faktur.getValue(),
                'c_ie': sFaktur,
                'n_bilvafaktur': nNilaiInv,
                'n_bed': nNilaiAms,
                'n_bedselisih': nSelisih,
                'n_disc': nClaim,
                'l_new': true
            }));

            faktur.reset();
            nilai.reset();
            sisa.reset();
            claim.reset();
            faktur.focus();
        }
        else {
            ShowError("Data Telah Ada");
            return;
        }
    }

    var deleteOnGrid = function(grid, rec, totInv, totAms, totSelisih, totNilai, claim, pinalty, nilaiLain, isNpwp, totPph) {
        //        var store = grid.getStore();

        //        var n_InvFisik = rec.get('n_bilvafaktur');
        //        var n_InvAms = rec.get('n_bed');

        //        var totalInv = totInv.getValue() - n_InvFisik;
        //        var totalAms = totAms.getValue() - n_InvAms;
        //        var totalSelisih = totalInv - totalAms;
        //        var totalNilai = (totalInv) - (claim.getValue() + pinalty.getValue());

        //        totInv.setValue(totalInv);
        //        totAms.setValue(totalAms);
        //        totSelisih.setValue(totalSelisih);
        //        totNilai.setValue(totalNilai);

        //        store.remove(rec);
        if (Ext.isEmpty(rec)) {
            return;
        }

        var isVoid = rec.get('l_void');
        var isNew = rec.get('l_new');

        if (isVoid) {
            ShowWarning('Item ini telah di batalkan.');
        }
        else {
            if (isNew) {
                var store = grid.getStore();

                var n_InvFisik = rec.get('n_bilvafaktur');
                var n_InvAms = rec.get('n_bed');
                var n_claim = rec.get('n_disc');

                var totalInv = totInv.getValue() - n_InvFisik;
                var totalAms = totAms.getValue() - n_InvAms;
                var totalSelisih = totalInv - totalAms;
                var totalClaim = claim.getValue() - n_claim;
                var totalNilai = totalInv - (pinalty.getValue() + totalClaim) + nilaiLain.getValue();

                if (isNpwp == "true") {
                    nPph = 0.02 * totalInv
                }
                else {
                    nPph = 0.04 * totalInv
                }
                totalNilai -= nPph;
                
                totInv.setValue(totalInv);
                totAms.setValue(totalAms);
                totSelisih.setValue(totalSelisih);
                claim.setValue(totalClaim);
                totNilai.setValue(totalNilai);
                totPph.setValue(nPph);

                store.remove(rec);
            }
            else {
                ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?', function(btn) {
                    if (btn == 'yes') {
                        ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.', function(btnP, txt) {
                            if (btnP == 'ok') {
                                if (txt.trim().length < 1) {
                                    txt = 'Kesalahan pemakai.';
                                }
                                rec.set('l_void', true);
                                rec.set('v_ket', txt);

                                var store = grid.getStore();

                                var n_InvFisik = rec.get('n_bilvafaktur');
                                var n_InvAms = rec.get('n_bed');
                                var n_claim = rec.get('n_disc');

                                var totalInv = totInv.getValue() - n_InvFisik;
                                var totalAms = totAms.getValue() - n_InvAms;
                                var totalSelisih = totalInv - totalAms;
                                var totalClaim = claim.getValue() - n_claim;
                                var totalNilai = totalInv - (pinalty.getValue() + totalClaim) + nilaiLain.getValue();

                                if (isNpwp == "true") {
                                    nPph = 0.02 * totalInv
                                }
                                else {
                                    nPph = 0.04 * totalInv
                                }
                                totalNilai -= nPph;
                                
                                totInv.setValue(totalInv);
                                totAms.setValue(totalAms);
                                totSelisih.setValue(totalSelisih);
                                claim.setValue(totalClaim);
                                totNilai.setValue(totalNilai);
                                totPph.setValue(nPph);                                
                            }
                        });
                    }
                });
            }
        }
    }

    var voidInsertedDataFromStoreFE = function(rec, grid, totInv, totAms, totSelisih, totNilai, claim, pinalty) {
        if (Ext.isEmpty(rec)) {
            return;
        }

        var isVoid = rec.get('l_void');
        var isNew = rec.get('l_new');

        if (isVoid) {
            ShowWarning('Item ini telah di batalkan.');
        }
        else {
            if (isNew) {
                var store = grid.getStore();

                var n_InvFisik = rec.get('n_bilvafaktur');
                var n_InvAms = rec.get('n_bed');

                var totalInv = totInv.getValue() - n_InvFisik;
                var totalAms = totAms.getValue() - n_InvAms;
                var totalSelisih = totalInv - totalAms;
                var totalNilai = (totalInv) - (claim.getValue() + pinalty.getValue());

                totInv.setValue(totalInv);
                totAms.setValue(totalAms);
                totSelisih.setValue(totalSelisih);
                totNilai.setValue(totalNilai);

                store.remove(rec);
            }
            else {
                ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?', function(btn) {
                    if (btn == 'yes') {
                        ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.', function(btnP, txt) {
                            if (btnP == 'ok') {
                                if (txt.trim().length < 1) {
                                    txt = 'Kesalahan pemakai.';
                                }
                                rec.set('l_void', true);
                                rec.set('v_ket', txt);

                                var store = grid.getStore();

                                var n_InvFisik = rec.get('n_bilvafaktur');
                                var n_InvAms = rec.get('n_bed');

                                var totalInv = totInv.getValue() - n_InvFisik;
                                var totalAms = totAms.getValue() - n_InvAms;
                                var totalSelisih = totalInv - totalAms;
                                var totalNilai = (totalInv) - (claim.getValue() + pinalty.getValue());

                                totInv.setValue(totalInv);
                                totAms.setValue(totalAms);
                                totSelisih.setValue(totalSelisih);
                                totNilai.setValue(totalNilai);
                            }
                        });
                    }
                });
            }
        }
    }
</script>

<ext:Window ID="winDetail" runat="server" Height="600" Width="800" Hidden="true"
    Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
    <content>
        <ext:Hidden ID="hfStoreID" runat="server" />
        <ext:Hidden ID="hfGdg" runat="server" />
        <ext:Hidden ID="hfGdgDesc" runat="server" />
        <ext:Hidden ID="hfBilva" runat="server" AllowDecimals="true" />
        <ext:Hidden ID="hfFaktur" runat="server" />
        <ext:Hidden ID="hfFakturFisik" runat="server" />
        <ext:Hidden ID="hfNilaiShipment" runat="server" />
        <ext:Hidden ID="hfClaim" runat="server" />
        <ext:Hidden ID="hfMaxValue" runat="server" />
        <ext:Hidden ID="hfIsNpwp" runat="server" />        
    </content>
    <items>
        <ext:BorderLayout ID="blLayout" runat="server">
            <North MinHeight="230" Collapsible="false">
                <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="235" AutoScroll="true"
                    Layout="Fit">
                    <Items>
                        <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" Layout="Form">
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
                                                        <ext:RecordField Name="l_npwp" />
                                                    </Fields>
                                                </ext:JsonReader>
                                            </Reader>
                                        </ext:Store>
                                    </Store>
                                    <Template ID="Template1" runat="server">
                                        <Html>
                                        <table cellpading="0" cellspacing="2" style="width: 100">
                                    <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td><td class="body-panel">Npwp</td></tr>
                                    <tpl for="."><tr class="search-item">
                                    <td>{c_exp}</td><td>{v_ket}</td><td>{l_npwp}</td>
                                    </tr></tpl>
                                    </table>
                                        </Html>
                                    </Template>
                                    <Triggers>
                                        <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                    </Triggers>
                                    <Listeners>
                                        <%--<Select Handler="this.triggers[0].show();" />--%>
                                        <Select Handler="selectedEksp(record, #{hfIsNpwp})" />
                                        <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                        <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:CompositeField ID="CompositeField3" runat="server" FieldLabel="Total Invoice">
                                    <Items>
                                        <ext:NumberField ID="txInvFisik" runat="server" FieldLabel="Sisa" AllowNegative="false"
                                            AllowDecimals="true" DecimalPrecision="2" Width="100" AllowBlank="false" />
                                        <ext:Label ID="Label2" runat="server" Text="   Total Shipment:" />
                                        <ext:NumberField ID="txInvAMS" runat="server" FieldLabel="Sisa" AllowNegative="false"
                                            AllowDecimals="true" DecimalPrecision="2" Width="100" AllowBlank="false" />
                                        <ext:Label ID="Label3" runat="server" Text="   Selisih:" />
                                        <ext:NumberField ID="txSelisih" runat="server" FieldLabel="Sisa" AllowNegative="true"
                                            AllowDecimals="true" DecimalPrecision="2" Width="100" AllowBlank="false" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:NumberField ID="txClaim" runat="server" FieldLabel="Pot.Claim" AllowBlank="false"
                                    AllowDecimals="true" AllowNegative="false" Width="100" >
                                    <DirectEvents>
                                        <Change OnEvent="BtnCalcBayar"></Change>
                                    </DirectEvents>
                                </ext:NumberField>
                                <ext:NumberField ID="txPinalty" runat="server" FieldLabel="Pot.Pinalty" AllowBlank="false"
                                    AllowDecimals="true" AllowNegative="false" Width="100" >
                                    <DirectEvents>
                                        <Change OnEvent="BtnCalcBayar"></Change>
                                    </DirectEvents>
                                </ext:NumberField>
                                <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Lain-lain">
                                    <Items>
                                        <ext:NumberField ID="txLain" runat="server" AllowBlank="false" AllowDecimals="true"
                                            AllowNegative="false" Width="100">
                                             <DirectEvents>
                                                <Change OnEvent="BtnCalcBayar"></Change>
                                             </DirectEvents>
                                        </ext:NumberField>
                                        <ext:Label ID="Label4" runat="server" Text="   Alasan:" />                                        
                                        <ext:TextField ID="txAlasan" runat="server" Width="150" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:NumberField ID="txPph" runat="server" FieldLabel="PPH" AllowBlank="false"
                                    AllowDecimals="true" AllowNegative="false" Width="100" >
                                    <DirectEvents>
                                        <Change OnEvent="BtnCalcBayar"></Change>
                                    </DirectEvents>
                                </ext:NumberField>                                
                                <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Total Bayar">
                                    <Items>
                                        <ext:NumberField ID="txTotBE" runat="server" AllowBlank="false" AllowDecimals="true"
                                            AllowNegative="false" Width="100">
                                        </ext:NumberField>
                                        <ext:Label ID="Label1" runat="server" Text="   Tgl Bayar:" />
                                        <ext:DateField ID="txDateBE" runat="server" AllowBlank="false" Width="100" Format="dd-MM-yyyy" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:TextField ID="txKet" runat="server" FieldLabel="Keterangan" Width="250" />
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
                                                <ext:ComboBox ID="cbResi" runat="server" DisplayField="c_ieno" ValueField="c_ieno"
                                                    Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="350" MinChars="3"
                                                    FieldLabel="Invoice" AllowBlank="false">
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
                                                                <ext:Parameter Name="model" Value="5010" />
                                                                <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                                                                       ['exp', #{cbEksHdr}.getValue(), 'System.String'],
                                                                       ['@contains.c_ieno.Contains(@0)', paramTextGetter(#{cbResi}), '']]" Mode="Raw" />
                                                                <ext:Parameter Name="sort" Value="c_ieno" />
                                                                <ext:Parameter Name="dir" Value="DESC" />
                                                            </BaseParams>
                                                            <Reader>
                                                                <ext:JsonReader IDProperty="c_ieno" Root="d.records" SuccessProperty="d.success"
                                                                    TotalProperty="d.totalRows">
                                                                    <Fields>
                                                                        <ext:RecordField Name="c_ieno" />
                                                                        <ext:RecordField Name="n_netsisa" />
                                                                        <ext:RecordField Name="n_bilva_faktur" />
                                                                        <ext:RecordField Name="c_ie" />
                                                                        <ext:RecordField Name="n_disc" />
                                                                        <ext:RecordField Name="n_totalbiaya" />
                                                                    </Fields>
                                                                </ext:JsonReader>
                                                            </Reader>
                                                        </ext:Store>
                                                    </Store>
                                                    <Template ID="Template3" runat="server">
                                                        <Html>
                                                        <table cellpading="0" cellspacing="5" >
                                                            <tr><td class="body-panel">No.IE</td><td class="body-panel">Nilai Invoice</td>
                                                            <td class="body-panel">Nilai Shipment</td>
                                                            <td class="body-panel">Faktur Fisik</td>><td class="body-panel">Nilai Fisik</td>
                                                            <td class="body-panel">Pot.Claim</td></tr>
                                                            <tpl for="."><tr class="search-item">
                                                            <td>{c_ieno}</td>
                                                            <td>{n_netsisa}</td>
                                                            <td>{n_totalbiaya}</td>
                                                            <td>{c_ie}</td>
                                                            <td>{n_bilva_faktur}</td>
                                                            <td>{n_disc}</td>
                                                            </tr></tpl>
                                                            </table>
                                                        </Html>
                                                    </Template>
                                                    <Triggers>
                                                        <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                                                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                                    </Triggers>
                                                    <Listeners>
                                                        <%--<Select Handler="this.triggers[0].show();" />--%>
                                                        <Select Handler="selectedInvoice(this, record, #{txNilai}, #{txSisa}, #{hfBilva}, #{hfFakturFisik}, #{hfMaxValue}, #{hfClaim}, #{hfNilaiShipment})" />
                                                        <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                                        <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
                                                    </Listeners>
                                                </ext:ComboBox>
                                                <ext:NumberField ID="txSisa" runat="server" FieldLabel="Sisa" AllowNegative="false"
                                                    AllowDecimals="true" DecimalPrecision="2" Width="100" />
                                                <ext:NumberField ID="txNilai" runat="server" FieldLabel="Nilai" AllowNegative="false"
                                                    AllowDecimals="true" DecimalPrecision="2" Width="100" AllowBlank="false" >
                                                    <DirectEvents>
                                                        <Change OnEvent="BtnCalcSisa"></Change>
                                                    </DirectEvents>
                                                </ext:NumberField>
                                                <ext:Button ID="btnAddResi" runat="server" FieldLabel="&nbsp;" LabelSeparator=" "
                                                    ToolTip="Add" Icon="Add">
                                                    <%--<DirectEvents>
                                                        <Click OnEvent="AddBtnResi_Click">
                                                            <ExtraParams>
                                                                <ext:Parameter Name="exp" Value="#{cbEksHdr}.getValue()" Mode="Raw" />
                                                                <ext:Parameter Name="resi" Value="#{cbResi}.getValue()" Mode="Raw" />
                                                                <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                            </ExtraParams>
                                                        </Click>
                                                    </DirectEvents>--%>
                                                    <Listeners>
                                                        <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbResi}, #{hfFakturFisik}.getValue(), #{txNilai}, #{hfBilva}.getValue(), #{txInvFisik}, #{txInvAMS}, #{txSelisih},#{txTotBE},#{txSisa},#{hfClaim},#{txClaim},#{hfNilaiShipment}.getValue(),#{txLain},#{txPinalty},#{hfIsNpwp}.getValue(),#{txPph});" />
                                                    </Listeners>
                                                </ext:Button>
                                                <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
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
                                                        <ext:Parameter Name="model" Value="0314" />
                                                        <ext:Parameter Name="sort" Value="" />
                                                        <ext:Parameter Name="dir" Value="" />
                                                        <ext:Parameter Name="parameters" Value="[['c_beno = @0', #{hfFaktur}.getValue(), 'System.String']]"
                                                            Mode="Raw" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                            <Fields>
                                                                <ext:RecordField Name="c_ieno" />
                                                                <ext:RecordField Name="c_ie" />
                                                                <ext:RecordField Name="n_bilvafaktur" />
                                                                <ext:RecordField Name="n_bed" />
                                                                <ext:RecordField Name="n_bedselisih" />
                                                                <ext:RecordField Name="n_disc" />
                                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                                <ext:RecordField Name="v_ket" />                        
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
                                                            <%--<ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />--%>
                                                        </Commands>
                                                        <%--<PrepareToolbar Handler="prepareCommandsDetilBerat(record, toolbar, #{hfFakturFisik}.getValue());" />--%>
                                                    </ext:CommandColumn>
                                                    <ext:Column DataIndex="c_ieno" Header="No.FE" Width="100" />
                                                    <ext:Column DataIndex="c_ie" Header="No.Faktur" Width="100" />
                                                    <ext:Column DataIndex="n_bilvafaktur" Header="Nilai Invoice" Width="100" />
                                                    <ext:Column DataIndex="n_bed" Header="Nilai Shipment" Width="100" />
                                                    <ext:Column DataIndex="n_bedselisih" Header="Selisih" Width="100" />
                                                    <ext:Column DataIndex="n_disc" Header="Pot.Claim" Width="100"  />
                                                    <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                                </Columns>
                                            </ColumnModel>
                                            <Listeners>
                                                <Command Handler="if(command == 'Delete') { deleteOnGrid(this, record, #{txInvFisik}, #{txInvAMS}, #{txSelisih}, #{txTotBE}, #{txClaim}, #{txPinalty}, #{txLain} ,#{hfIsNpwp}.getValue(), #{txPph}); } " />
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
                            </Items>
                        </ext:Toolbar>
                    </BottomBar>
                </ext:Panel>
            </Center>
        </ext:BorderLayout>
    </items>
    <buttons>
        <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
          <DirectEvents>
            <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
              <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="NumberID" Value="#{hfFaktur}.getValue()" Mode="Raw" />
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
                        <ext:Parameter Name="NumberID" Value="#{hfFaktur}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                            Mode="Raw" />
                        <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="Ekspedisi" Value="#{cbEksHdr}.getValue()" Mode="Raw" />
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
    </buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
