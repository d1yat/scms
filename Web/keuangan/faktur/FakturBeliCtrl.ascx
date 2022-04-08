<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FakturBeliCtrl.ascx.cs"
  Inherits="keuangan_pembayaran_FakturBeliCtrl" %>

<script type="text/javascript">
  var storeToDetailGrid = function(frm, gridDetailStore, grid, typ, expe, dat, valu) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(typ) ||
          Ext.isEmpty(expe) ||
          Ext.isEmpty(dat) ||
          Ext.isEmpty(valu)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

    var store = grid.getStore();
    if (Ext.isEmpty(store)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }

    var typVal = typ.getValue();
    var expVal = expe.getValue();

    var valX = [typVal, expVal];
    var fieldX = ['c_type', 'c_exp'];

    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

    if (!isDup) {
      var vale = valu.getValue();

      store.insert(0, new Ext.data.Record({
        'c_type': typVal,
        'v_type_desc': typ.getText(),
        'c_exp': (typVal == '02' ? '' : expVal),
        'v_exp_desc': (typVal == '02' ? '' : expe.getText()),
        'd_top': dat.getValue(),
        'n_sisa': vale,
        'n_value': vale,
        'l_new': true
      }));

      typ.reset();
      expe.reset();
      expe.enable();
      dat.reset();
      valu.reset();

      recalculateFaktur(gridDetailStore);
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  //  var changeKursAct = function(c, val, t) {
  //    var store = c.getStore();
  //    if (!Ext.isEmpty(store)) {
  //      var idx = store.findExact('c_kurs', val);
  //      if (idx != -1) {
  //        var r = store.getAt(idx);
  //        t.setValue(r.get('n_currency'));
  //      }
  //    }
  //  }

  //  var autoChangeDateTop = function(oValu, nValu, lb, tx, fakt) {
  //    if (nValu < 0) {
  //      ShowError('Minimum jumlah tidak boleh lebih kecil dari 0');

  //      return;
  //    }

  //    var dat = new Date(),
  //      dt = new Date();
  //    var iVal = 0;

  //    if (!Ext.isEmpty(lb)) {
  //      if ((!Ext.isEmpty(fakt)) || (fakt.trim().length > 0)) {
  //        dat = Date.parseDate(lb.getText(), 'd-m-Y');
  //        iVal = (nValu - oValu);
  //      }
  //      else if (!Ext.isEmpty(tx)) {
  //        dat = tx.getValue();
  //        iVal = nValu;
  //      }
  //      else {
  //        dat = new Date();
  //        iVal = nValu;
  //      }

  //      if (Ext.isEmpty(dat)) {
  //        lb.setText('parseFailed');

  //        return;
  //      }

  //      //nValu
  //      if (iVal < 0) {
  //        dt = dat.add(Date.DAY, iVal);
  //      }
  //      else if (iVal > 0) {
  //        dt = dat.add(Date.DAY, iVal);
  //      }
  //      else {
  //        dt = dat;
  //      }

  //      lb.setText(dt.format('d-m-Y'));
  //    }
  //    //    else {
  //    //      lb.setText('parseFailed');
  //    //    }
  //  }

  var autoChangeDateTop1 = function(oValu, nValu, lb, tx, fakt) {
      if (nValu < 0) {
          ShowError('Minimum jumlah tidak boleh lebih kecil dari 0');

          return;
      }

      var dat = new Date(),
      dt = new Date();
      var iVal = 0;

      if (!Ext.isEmpty(lb)) {
          if ((!Ext.isEmpty(fakt)) || (fakt.trim().length > 0)) {
              dat = Date.parseDate(lb.getText(), 'd-m-Y');
              iVal = (nValu - oValu);
          }
          if (!Ext.isEmpty(tx)) {
              dat = tx.getValue();
              iVal = nValu;
          }
          else {
              dat = new Date();
              iVal = nValu;
          }

          if (Ext.isEmpty(dat)) {
              lb.setText('parseFailed');

              return;
          }

          //nValu
          if (iVal < 0) {
              dt = dat.add(Date.DAY, iVal);
          }
          else if (iVal > 0) {
              dt = dat.add(Date.DAY, iVal);
          }
          else {
              dt = dat;
          }

          lb.setText(dt.format('d-m-Y'));
      }
      //    else {
      //      lb.setText('parseFailed');
      //    }
  }
  var autoChangeDate = function(value1, value2, lb1, lb2, dateOri) {
      if (!Ext.isEmpty(value1) || !Ext.isEmpty(value2)) {
          var dt1 = new Date(), dt2 = new Date(), dat = new Date();
          var iVal1 = 0, iVal2 = 0;

          iVal1 = value1.getValue();
          iVal2 = value2.getValue();
          dat = dateOri.getValue();
          
          if (iVal1 < 0 || iVal2 < 0) {
              ShowError('Minimum T O P dan T O P P J G tidak boleh lebih kecil dari 0');
              return;
          }
          dt1 = dat.add(Date.DAY, iVal1);
          dt2 = dat.add(Date.DAY, iVal2);
          lb1.setText(dt1.format('d-m-Y'));
          lb2.setText(dt1.format('d-m-Y'));    
      }

  }
  
  var onBeforeEditGrid = function(e) {
    if (e.record.get('l_void')) {
      e.cancel = true;
    }
    else if (e.field == 'n_qty') {
      var isNew = e.record.get('l_new');
      if (!isNew) {
        //e.cancel = true;
      }
    }
  }

  var onAfterEditGrid = function(e) {
    if (!e.record.get('l_void')) {
      if (!e.record.get('l_new')) {
        e.record.set('l_modified', true);
      }

      recalculateFaktur(e.grid.getStore());
    }
  }

  var onBeforeEditGridBea = function(e) {
    if (e.record.get('l_void')) {
      e.cancel = true;
    }
  }

  var onAfterEditGridBea = function(e, storeGridDetail) {
    if (!e.record.get('l_void')) {
      if (!e.record.get('l_new')) {
        e.record.set('l_modified', true);
      }
      //e.record.set('l_modified', true);

      recalculateFaktur(e.grid.getStore());
    }
  }

  var calcPph22 = function(gridDetailStore, pph22) {

    var nLoop = 0;
    var nTotal = 0;
    var nTotFinal = 0;

    for (nLoop = 0; nLoop < gridDetailStore.getCount(); nLoop++) {
      nTotal = gridDetailStore.data.items[nLoop].data['n_ppph'];
      nTotFinal = nTotFinal + nTotal;
    }

    nTotFinal = nTotFinal / nLoop;

    pph22.setValue(nTotFinal);
  }



  var ChangPph = function(grid, NewValue) {


      var ppph = 0;
      var NewPph = NewValue.getValue();

      grid.each(function(r) {
          if ((!r.get('l_void')) && (r.get('c_type') == '01')) {
              r.beginEdit();
              r.set('n_ppph', NewPph);
              r.commit();
              r.endEdit();
          }
      });
      grid.each(function(r) {
          if (!r.get('l_void')) {
              if (!r.get('l_new')) {
                  r.set('l_modified', true);
              }
          }
      });
  }

  var recalculateFaktur = function(store) {
      var lbGross = Ext.getCmp('<%= lbGrossBtm.ClientID %>');
      var lbTax = Ext.getCmp('<%= lbTaxBtm.ClientID %>');
      var lbDisc = Ext.getCmp('<%= lbDiscBtm.ClientID %>');
      var lbNet = Ext.getCmp('<%= lbNetBtm.ClientID %>');
      var fExtDisc = Ext.getCmp('<%= txExtDiscHdr.ClientID %>');
      var fNPph22 = Ext.getCmp('<%= txNPph22.ClientID %>');
      var gDetailBea = Ext.getCmp('<%= gridDetailBea.ClientID %>');
      var lbExtDisc = Ext.getCmp('<%= lbExtDiscHdr.ClientID %>');
      var txNPph = Ext.getCmp('<%= txNPph22.ClientID %>');
      var txXdiscText = 0;
      //    var lbDiscExtra = Ext.getCmp('<%= lbDiscExtraBtm.ClientID %>');

      var sumGross = 0,
      sumDisc = 0,
      sumTax = 0,
      sumNet = 0,
      qty = 0, hna = 0, disc = 0, n_bea = 0, nxDisc = 0, totalHarga = 0,
      extDisc = fExtDisc.getValue(),
      NPph22 = fNPph22.getValue(),
      sumBea = 0, callNetto = 0,
      ppph = 0, sumPpph = 0,
      //nDiscValPerc = txXdiscText.getValue(),
    XdiscHasil = 0;
      //    var sumDiscExtra = 0;


      store.each(function(r) {
          if ((!r.get('l_void')) && (r.get('c_type') == '01')) {
              qty = r.get('n_qty');
              hna = r.get('n_salpri');
              disc = r.get('n_disc');
              bea = r.get('n_bea');
              ppph = r.get('n_ppph');
              //        discextra = r.get('n_discextra');

              sumPpph += ppph;
              total = (qty * hna);

              sumGross += (total + n_bea);
              sumDisc += (total * (disc / 100));
              //        sumDiscExtra += (total * (discextra / 100));
          }
      });

      if (!Ext.isEmpty(gDetailBea)) {
          var storeBea = gDetailBea.getStore();

          if (!Ext.isEmpty(storeBea)) {
              sumBea = storeBea.sum('n_value');

              if (sumBea > 0) {
                  sumNet = (sumGross - sumDisc);

                  store.each(function(r) {
                      if ((!r.get('l_void')) && (r.get('c_type') == '01')) {
                          qty = r.get('n_qty');
                          hna = r.get('n_salpri');
                          disc = r.get('n_disc');
                          //ppph = r.get('n_ppph');
                          //              discextra = r.get('n_discextra');

                          total = (qty * hna);
                          callNetto = (total * (disc / 100));

                          if (!r.get('l_new')) {
                              r.set('l_modified', true);
                          }

                          r.set('n_bea',
                (((total - callNetto) / sumNet) * sumBea));
                      }
                  });
              }
              else {
                  store.each(function(r) {
                      if (!r.get('l_void')) {
                          if (!r.get('l_new')) {
                              r.set('l_modified', true);
                          }

                          r.set('n_bea', 0);
                      }
                  });
              }
          }
      }

      sumPpph = sumPpph / store.getCount();

      txNPph.setValue(sumPpph);

      if (extDisc == 0) {
          XdiscHasil = (lbExtDisc.getValue() / sumGross) * 100;
          if (!Ext.isEmpty(lbExtDisc)) {
              fExtDisc.setValue(XdiscHasil);
          }
      }
      else {
          nxDisc = (sumGross * (extDisc / 100));
          if (!Ext.isEmpty(lbExtDisc)) {
              lbExtDisc.setValue(nxDisc);
          }
      }


      totalHarga = (sumGross - sumDisc - nxDisc);

      sumTax = (totalHarga * 0.11);

      //sumNet = (totalHarga + (sumTax + sumBea));
      sumNet = (totalHarga + sumTax);

      if (!Ext.isEmpty(lbGross)) {
          lbGross.setText(myFormatNumber(sumGross));
      }

      if (!Ext.isEmpty(lbDisc)) {
          lbDisc.setText(myFormatNumber(sumDisc));
      }
      if (!Ext.isEmpty(lbTax)) {
          lbTax.setText(myFormatNumber(sumTax));
      }
      if (!Ext.isEmpty(lbNet)) {
          lbNet.setText(myFormatNumber(sumNet));
      }
      //    if (!Ext.isEmpty(lbDiscExtra)) {
      //        lbDiscExtra.setText(myFormatNumber(sumDiscExtra));
      //    }
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

  //  var validateBeaType = function(val, cb) {
  //    if (val == '01') {
  //      cb.enable();
  //    }
  //    else {
  //      cb.reset();
  //      cb.disable();
  //    }
  //  }

  var onRnSelect = function(rec, hf) {
    if (Ext.isEmpty(hf)) {
      return;
    }
    if (Ext.isEmpty(rec)) {
      hf.setValue('');
    }
    else {
      hf.setValue(rec.get('Gudang'));
    }
  }

  var deleteRecordBeaOnGrid = function(g, rec, gData) {
    deleteRecordOnStore(g.getStore(), rec, function() {
      recalculateFaktur(gData.getStore());
    });
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="825" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfFaktur" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGdg" runat="server" />
    <ext:Hidden ID="hfDate" runat="server" />    
  </Content>
  <Items>
    <ext:BorderLayout ID="blLayout" runat="server">
      <North MinHeight="215" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="215" AutoScroll="true"
          Layout="Fit">
          <Items>                              
            <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">                              
                  <Items>
                    <ext:TextField ID="txFakturHdr" runat="server" AllowBlank="false" FieldLabel="Faktur"
                      Width="250" />
                    <ext:DateField ID="txTanggalHdr" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                      Width="100" Format="dd-MM-yyyy" >
                      <Listeners>
                         <Change Handler="autoChangeDate(#{txTopHdr}, #{txTopPjgHdr}, #{lbDateTopHdr}, #{lbDateTopPjgHdr}, #{txTanggalHdr});" />
                      </Listeners>
                    </ext:DateField>
                    <ext:ComboBox ID="cbPemasokHdr" runat="server" DisplayField="v_nama" ValueField="c_nosup"
                      Width="200" ItemSelector="tr.search-item" ListWidth="275" MinChars="3" FieldLabel="Pemasok"
                      PageSize="10">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <%--<ext:Parameter Name="model" Value="2021" />--%>
                            <ext:Parameter Name="model" Value="14013" />
                            <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                    ['l_aktif = @0', true, 'System.Boolean'],
                                    ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPemasokHdr}), '']]"
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
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_nosup}</td><td>{v_nama}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Listeners>
                        <Change Handler="clearRelatedComboRecursive(true, #{cbNoDOHdr});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbNoDOHdr" runat="server" DisplayField="DeliveryNo" ValueField="ReceiveNote"
                      Width="200" ItemSelector="tr.search-item" ListWidth="350" MinChars="3" FieldLabel="No. Delivery"
                      PageSize="10">
                      <Store>
                        <ext:Store ID="Store1" runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="14001" />
                            <ext:Parameter Name="parameters" Value="[['suplierId', #{cbPemasokHdr}.getValue(), 'System.String'],
                              ['Gudang = @0', '1', 'System.Char'],
                              ['@contains.DeliveryNo.Contains(@0) || @contains.ReceiveNote.Contains(@0)', paramTextGetter(#{cbNoDOHdr}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="TanggalDO" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_kurs" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="Gudang" />
                                <ext:RecordField Name="GudangDesc" />
                                <ext:RecordField Name="DeliveryNo" />
                                <ext:RecordField Name="ReceiveNote" />
                                <ext:RecordField Name="TanggalRN" Type="Date" DateFormat="M$" />
                                <ext:RecordField Name="TanggalDO" Type="Date" DateFormat="M$" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template ID="Template1" runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 350px">
                        <tr><td class="body-panel">Tgl RN</td><td class="body-panel">No. Receive</td>
                        <td class="body-panel">No. Delivery</td><td class="body-panel">Tanggal Delivery</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{TanggalRN:this.formatDate}</td><td>{ReceiveNote}</td><td>{DeliveryNo}</td><td>{TanggalDO:this.formatDate}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                        <Functions>
                          <ext:JFunction Name="formatDate" Fn="myFormatDate" />
                        </Functions>
                      </Template>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                        <Select Handler="onRnSelect(record, #{hfGdg})" />
                        <%--<Change --%>
                      </Listeners>
                      <DirectEvents>
                        <Change OnEvent="NoDOHdr_Change" Before="showMaskLoad(#{gridDetail}, 'Sedang membaca data...', false);"
                          Success="showMaskLoad(#{gridDetail}, '', true);" Failure="showMaskLoad(#{gridDetail}, '', true);">
                          <%--<EventMask ShowMask="true" />--%>
                          <ExtraParams>
                            <ext:Parameter Name="NoSupl" Value="#{cbPemasokHdr}.getValue()" Mode="Raw" />
                            <ext:Parameter Name="NoDO" Value="#{cbNoDOHdr}.getValue()" Mode="Raw" />
                            <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
                          </ExtraParams>
                        </Change>
                      </DirectEvents>
                    </ext:ComboBox>
                    <ext:CompositeField runat="server" FieldLabel="Pajak">
                      <Items>
                        <ext:TextField ID="txTaxNoHdr" runat="server" AllowBlank="true" FieldLabel="No. Pajak"
                          Width="150" />
                        <ext:DateField ID="txTaxDateHdr" runat="server" AllowBlank="true" FieldLabel="Tanggal Pajak"
                          Width="100" Format="dd-MM-yyyy">
                        </ext:DateField>
                        <%--
                        <ext:Button ID="btnProcessPpn" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Proses" Text="Hitung Pajak">
                            <DirectEvents>
                                <Click OnEvent="TaxDateHdr_Change"></Click>
                            </DirectEvents>
                        </ext:Button>
                        --%>
                        <ext:Hidden ID="hfTax" runat="server" Text=""></ext:Hidden>
                      </Items>
                    </ext:CompositeField>
                    <ext:CompositeField runat="server" FieldLabel="Kurs">
                      <Items>
                        <ext:ComboBox ID="cbKursHdr" runat="server" DisplayField="v_desc" ValueField="c_kurs"
                          Width="100" ItemSelector="tr.search-item" ListWidth="275" MinChars="3">
                          <Store>
                            <ext:Store runat="server">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="start" Value="={0}" />
                                <ext:Parameter Name="limit" Value="-1" />
                                <ext:Parameter Name="allQuery" Value="true" />
                                <ext:Parameter Name="model" Value="2071" />
                                <ext:Parameter Name="parameters" Value="[['@contains.c_kurs.Contains(@0) || @contains.v_desc.Contains(@0)', paramTextGetter(#{cbKursHdr}), '']]"
                                  Mode="Raw" />
                                <ext:Parameter Name="sort" Value="v_desc" />
                                <ext:Parameter Name="dir" Value="ASC" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader IDProperty="c_kurs" Root="d.records" SuccessProperty="d.success"
                                  TotalProperty="d.totalRows">
                                  <Fields>
                                    <ext:RecordField Name="c_kurs" />
                                    <ext:RecordField Name="v_desc" />
                                    <ext:RecordField Name="c_symbol" />
                                    <ext:RecordField Name="n_currency" Type="Float" />
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                            </ext:Store>
                          </Store>
                          <Template runat="server">
                            <Html>
                            <table cellpading="0" cellspacing="1" style="width: 275px">
                            <tr><td class="body-panel">Simbol</td>
                            <td class="body-panel">Nama</td><td class="body-panel">Nilai</td></tr>
                            <tpl for="."><tr class="search-item">
                            <td>{c_symbol}</td><td>{v_desc}</td><td>{n_currency:this.formatNumber}</td>
                            </tr></tpl>
                            </table>
                            </Html>
                            <Functions>
                              <ext:JFunction Name="formatNumber" Fn="myFormatNumber" />
                            </Functions>
                          </Template>
                          <Triggers>
                            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                            
                          </Triggers>
                          <Listeners>
                            <Change Handler="changeKursAct(this, newValue, #{txKursValueHdr});" />
                            <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                          </Listeners>
                        </ext:ComboBox>
                        <ext:Label runat="server" Text="&nbsp;" />
                        <ext:NumberField ID="txKursValueHdr" runat="server" AllowBlank="false" AllowDecimals="true"
                          DecimalPrecision="2" AllowNegative="false" Width="75" />
                      </Items>
                    </ext:CompositeField>
                  </Items>
                </ext:Panel>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:CompositeField runat="server" FieldLabel="Extra Discount">
                      <Items>
                        <ext:NumberField ID="txExtDiscHdr" runat="server" AllowBlank="false" AllowDecimals="true"
                          DecimalPrecision="2" AllowNegative="false" Width="50" MaxValue="100">
                          <Listeners>
                            <Change Handler="recalculateFaktur(#{gridDetail}.getStore());" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Label runat="server" Text="&nbsp;" />
                        <%--<ext:Label ID="lbExtDiscHdr" runat="server" Text="0" />--%>
                        <ext:TextField ID="lbExtDiscHdr" runat="server" Text="0">
                          <Listeners>
                            <Change Handler="recalculateFaktur(#{gridDetail}.getStore());" />
                          </Listeners>
                        </ext:TextField>
                      </Items>
                    </ext:CompositeField>
                    <ext:CompositeField runat="server" FieldLabel="T O P">
                      <Items>
                        <ext:NumberField ID="txTopHdr" runat="server" AllowBlank="false" FieldLabel="T O P"
                          Width="50" AllowDecimals="false" AllowNegative="false">
                          <Listeners>
                            <Change Handler="autoChangeDateTop1(oldValue, newValue, #{lbDateTopHdr}, #{txTanggalHdr}, #{hfFaktur}.getValue());" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Label runat="server" Text="&nbsp;" />
                        <ext:Label ID="lbDateTopHdr" runat="server" Text="" />
                      </Items>
                    </ext:CompositeField>
                    <ext:CompositeField runat="server" FieldLabel="T O P P J G">
                      <Items>
                        <ext:NumberField ID="txTopPjgHdr" runat="server" AllowBlank="false" FieldLabel="T O P"
                          Width="50" AllowDecimals="false" AllowNegative="false">
                          <Listeners>
                            <Change Handler="autoChangeDateTop1(oldValue, newValue, #{lbDateTopPjgHdr}, #{txTanggalHdr}, #{hfFaktur}.getValue());" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Label runat="server" Text="&nbsp;" />
                        <ext:Label ID="lbDateTopPjgHdr" runat="server" Text="" />
                      </Items>
                    </ext:CompositeField>
                    <ext:CompositeField runat="server" FieldLabel="PPH 22">
                      <Items>
                        <ext:NumberField ID="txNPph22" runat="server" AllowBlank="false" FieldLabel="PPh 22 (%)"
                          Width="50" AllowDecimals="true" AllowNegative="false" MaxValue="100">
                          <Listeners>
                            <%--<Change Handler="autoChangeGridPpph(oldValue, newValue, #{gridDetail});" />--%>
                            <%-- <Change Handler="autoChangeGridPpph(record, #{gridDetail});" />--%>
                            <Change Handler="ChangPph(#{gridDetail}.getStore(), this)" />
                          </Listeners>
                          <%--<DirectEvents>
                            <Change OnEvent="onchange">
                              <ExtraParams>
                                <ext:Parameter Name="oldValue" Value="oldValue" Mode="Raw" />
                                <ext:Parameter Name="newValue" Value="newValue" Mode="Raw" />
                              </ExtraParams>
                            </Change>
                          </DirectEvents>--%>
                        </ext:NumberField>
                        <ext:Label ID="Label1" runat="server" Text="&nbsp;" />
                        <ext:Label ID="lbNPph22" runat="server" Text="0" />
                      </Items>
                    </ext:CompositeField>
                    <ext:NumberField ID="txTotalFaktur" runat="server" AllowBlank="false" AllowDecimals="true"
                      DecimalPrecision="2" AllowNegative="false" FieldLabel="Fisik Faktur" />
                    <ext:TextField ID="txKeteranganHdr" runat="server" FieldLabel="Keterangan" MaxLength="100"
                      Width="255" />
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
                
                  <Items>
                    <ext:GridPanel ID="gridDetail" runat="server">
                      <LoadMask ShowMask="true" />
                      <Listeners>
                        <BeforeEdit Handler="onBeforeEditGrid(e);" />
                        <AfterEdit Handler="onAfterEditGrid(e);" />
                        <Command Handler="if ((command == 'Void') || (command == 'Delete')) { voidFakturInsertedDataFromStore(this.getStore(), record); }" />
                        <%--<Command Handler="if (command == 'Void') { voidFakturInsertedDataFromStore(this.getStore(), record); }" />--%>
                        <%--<Command Handler="if(command == 'Delete') { voidFakturInsertedDataFromStore(this, record); } else if (command == 'Void') { voidFakturInsertedDataFromStore(record); }" />--%>
                      </Listeners>
                      <SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                      </SelectionModel>
                      <Store>
                        <ext:Store runat="server" RemoteSort="true">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <AutoLoadParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                          </AutoLoadParams>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                            <ext:Parameter Name="model" Value="0103" />
                            <ext:Parameter Name="sort" Value="v_itnam" />
                            <ext:Parameter Name="dir" Value="ASC" />
                            <ext:Parameter Name="parameters" Value="[['fakturNo', paramValueGetter(#{hfFaktur}), 'System.String']]"
                              Mode="Raw" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records"
                              SuccessProperty="d.success">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="v_itnam" />
                                <ext:RecordField Name="c_type" />
                                <ext:RecordField Name="v_type_desc" />
                                <ext:RecordField Name="n_bea" Type="Float" />
                                <ext:RecordField Name="n_disc" Type="Float" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                                <ext:RecordField Name="n_salpri" Type="Float" />
                                <ext:RecordField Name="n_ppph" Type="Float" />
                                <ext:RecordField Name="l_new" Type="Boolean" />
                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                <ext:RecordField Name="l_void" Type="Boolean" />
                                <ext:RecordField Name="l_pph" Type="Boolean" />
                                <ext:RecordField Name="v_ket" />
                                <%--<ext:RecordField Name="n_discextra" Type="Float" />--%>
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                          <SortInfo Field="v_itnam" Direction="ASC" />
                        </ext:Store>
                      </Store>
                      <ColumnModel>
                        <Columns>                              
                          <ext:CommandColumn Width="25">
                            <Commands>
                              <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                              <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                            </Commands>
                            <PrepareToolbar Handler="prepareGridButtonCommands(record, toolbar, #{hfFaktur}.getValue());" />
                          </ext:CommandColumn>
                          <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                          <ext:Column DataIndex="v_itnam" Header="Nama" Width="225" />
                          <ext:Column DataIndex="v_type_desc" Header="Tipe" Width="50" />
                          <ext:NumberColumn DataIndex="n_qty" Header="Jumlah" Format="0.000,00/i">
                            <Editor>
                              <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                DecimalPrecision="2" MinValue="0">
                                <Listeners>
                                  <Focus Handler="this.selectText();" />
                                </Listeners>
                              </ext:NumberField>
                            </Editor>
                          </ext:NumberColumn>
                          <%--<ext:NumberColumn DataIndex="n_disc" Header="Bonus" Format="0.000,00/i" />--%>
                          <ext:NumberColumn DataIndex="n_disc" Header="Potongan" Format="0.000,00/i" Editable="true">
                            <Editor>
                              <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                DecimalPrecision="2" MinValue="0" MaxValue="100">
                                <Listeners>
                                  <Focus Handler="this.selectText();" />
                                </Listeners>
                              </ext:NumberField>
                            </Editor>
                          </ext:NumberColumn>
                          <%--<ext:NumberColumn DataIndex="n_discextra" Header="Diskon Ekstra" Format="0.000,00/i" Editable="true">
                            <Editor>
                              <ext:NumberField ID="NumberField2" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                DecimalPrecision="2" MinValue="0" MaxValue="100">
                                <Listeners>
                                  <Focus Handler="this.selectText();" />
                                </Listeners>
                              </ext:NumberField>
                            </Editor>
                          </ext:NumberColumn>--%>
                          <ext:NumberColumn DataIndex="n_salpri" Header="Harga" Format="0.000,00/i" Editable="true">
                            <Editor>
                              <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                DecimalPrecision="2" MinValue="0">
                                <Listeners>
                                  <Focus Handler="this.selectText();" />
                                </Listeners>
                              </ext:NumberField>
                            </Editor>
                          </ext:NumberColumn>
                          <ext:NumberColumn DataIndex="n_bea" Header="Bea" Format="0.000,00/i" />
                          <ext:NumberColumn DataIndex="n_ppph" Header="PPH22" Format="0.000,00/i">
                            <Editor>
                              <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                DecimalPrecision="2" MinValue="0" >
                                <%--<Listeners>
                                  <Change Handler="calcPph22(#{gridDetail}.getStore(), #{txNPph22});" />
                                  <KeyDown Handler="calcPph22(#{gridDetail}.getStore(), #{txNPph22});" />
                                </Listeners>--%>
                              </ext:NumberField>
                            </Editor>
                          </ext:NumberColumn>
                          <ext:CheckColumn DataIndex="l_void" Header="Hapus" Width="50" />
                        </Columns>
                      </ColumnModel>
                    </ext:GridPanel>
                  </Items>
                </ext:Panel>
                <ext:Panel ID="pnlGridDtlBea" runat="server" Title="Detail Bea" Layout="FitLayout">
                  <TopBar>
                    <ext:Toolbar runat="server">
                      <Items>
                        <ext:FormPanel ID="frmpnlDetailEntryGridDtlBea" runat="server" Frame="True" Layout="Table"
                          LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                          <Items>
                            <ext:ComboBox ID="cbTipeDtl" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                              ValueField="c_type" Width="250" TypeAhead="false">
                              <Store>
                                <ext:Store runat="server" RemotePaging="false">
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="model" Value="2001" />
                                    <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                              ['c_notrans = @0', '53', ''],
                                              ['c_type != @0', '00', '']]" Mode="Raw" />
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
                              <Listeners>
                                <Change Handler="validateBeaType(newValue, #{cbExpedisiDtl});" />
                              </Listeners>
                            </ext:ComboBox>
                            <ext:ComboBox ID="cbExpedisiDtl" runat="server" FieldLabel="Expeditur" DisplayField="v_ket"
                              ValueField="c_exp" Width="250" TypeAhead="false">
                              <Store>
                                <ext:Store runat="server" RemotePaging="false">
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="allQuery" Value="true" />
                                    <ext:Parameter Name="model" Value="2081" />
                                    <ext:Parameter Name="parameters" Value="[['l_aktif = @0', 'true', 'System.Boolean'],
                                      ['l_import = @0', 'true', 'System.Boolean'],
                                      ['@contains.c_exp.Contains(@0) || @contains.v_ket.Contains(@0)', paramTextGetter(#{cbExpedisiDtl}), '']]"
                                      Mode="Raw" />
                                    <ext:Parameter Name="sort" Value="v_ket" />
                                    <ext:Parameter Name="dir" Value="ASC" />
                                  </BaseParams>
                                  <Reader>
                                    <ext:JsonReader IDProperty="c_exp" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                                      <Fields>
                                        <ext:RecordField Name="c_exp" />
                                        <ext:RecordField Name="v_ket" />
                                      </Fields>
                                    </ext:JsonReader>
                                  </Reader>
                                </ext:Store>
                              </Store>
                            </ext:ComboBox>
                            <ext:DateField ID="txDateTopBeaDtl" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                              Width="100" Format="dd-MM-yyyy" />
                            <ext:NumberField ID="txValueBeaDtl" runat="server" AllowBlank="false" AllowDecimals="true"
                              DecimalPrecision="2" AllowNegative="false" FieldLabel="Jumlah" />
                            <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                              Icon="Add">
                              <Listeners>
                                <Click Handler="storeToDetailGrid(#{frmpnlDetailEntryGridDtlBea}, #{gridDetail}.getStore(), #{gridDetailBea}, #{cbTipeDtl}, #{cbExpedisiDtl}, #{txDateTopBeaDtl}, #{txValueBeaDtl});" />
                              </Listeners>
                            </ext:Button>
                            <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                              Icon="Cancel">
                              <Listeners>
                                <Click Handler="#{frmpnlDetailEntryGridDtlBea}.getForm().reset();#{cbExpedisiDtl}.enable();" />
                              </Listeners>
                            </ext:Button>
                          </Items>
                        </ext:FormPanel>
                      </Items>
                    </ext:Toolbar>
                  </TopBar>
                  <Items>
                    <ext:GridPanel ID="gridDetailBea" runat="server">
                      <LoadMask ShowMask="true" />
                      <SelectionModel>
                        <ext:RowSelectionModel SingleSelect="true" />
                      </SelectionModel>
                      <Store>
                        <ext:Store runat="server" RemoteSort="true">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <AutoLoadParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                          </AutoLoadParams>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="-1" Mode="Raw" />
                            <ext:Parameter Name="model" Value="0104" />
                            <ext:Parameter Name="sort" Value="v_type_desc" />
                            <ext:Parameter Name="dir" Value="ASC" />
                            <ext:Parameter Name="parameters" Value="[['fakturNo', paramValueGetter(#{hfFaktur}), 'System.String']]"
                              Mode="Raw" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                              <Fields>
                                <ext:RecordField Name="c_type" />
                                <ext:RecordField Name="v_type_desc" />
                                <ext:RecordField Name="c_exp" />
                                <ext:RecordField Name="v_exp_desc" />
                                <ext:RecordField Name="d_top" Type="Date" DateFormat="M$" />
                                <ext:RecordField Name="n_sisa" Type="Float" />
                                <ext:RecordField Name="n_value" Type="Float" />
                                <ext:RecordField Name="l_new" Type="Boolean" />
                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                <ext:RecordField Name="l_void" Type="Boolean" />
                                <ext:RecordField Name="v_ket" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                          <SortInfo Field="v_type_desc" Direction="ASC" />
                        </ext:Store>
                      </Store>
                      <ColumnModel>
                        <Columns>
                          <ext:CommandColumn Width="25">
                            <Commands>
                              <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                              <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                            </Commands>
                            <PrepareToolbar Handler="prepareGridButtonNRCommands(record, toolbar);" />
                          </ext:CommandColumn>
                          <ext:Column DataIndex="v_type_desc" Header="Tipe" Width="150" />
                          <ext:Column DataIndex="v_exp_desc" Header="Expeditur" Width="250" />
                          <ext:DateColumn DataIndex="d_top" Header="T O P" Format="dd-MM-yyyy" Editable="true">
                            <Editor>
                              <ext:DateField runat="server" Format="dd-MM-yyyy" />
                            </Editor>
                          </ext:DateColumn>
                          <ext:NumberColumn DataIndex="n_value" Header="Jumlah" Format="0.000,00/i" Editable="true">
                            <Editor>
                              <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                DecimalPrecision="2">
                                <Listeners>
                                  <Focus Handler="this.selectText();" />
                                </Listeners>
                              </ext:NumberField>
                            </Editor>
                          </ext:NumberColumn>
                          <ext:CheckColumn DataIndex="l_void" Header="Hapus" Width="50" />
                        </Columns>
                      </ColumnModel>
                      
                      <Listeners>
                        <%--<Added Handler="recalculateFaktur(#{gridDetail}.getStore());" />--%>
                        <BeforeEdit Handler="onBeforeEditGridBea(e);" />
                        <AfterEdit Handler="onAfterEditGridBea(e, #{gridDetail}.getStore());" />
                        <Command Handler="if(command == 'Delete') { deleteRecordBeaOnGrid(this, record, #{gridDetail}); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
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
                <ext:FormPanel runat="server" AutoScroll="true" Height="60" Layout="FitLayout">
                  <Items>
                    <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
                      <Items>
                        <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                          <Items>
                            <ext:Label ID="lbGrossBtm" runat="server" Text="" FieldLabel="Gross" />
                            <ext:Label ID="lbTaxBtm" runat="server" Text="" FieldLabel="Pajak" />
                          </Items>
                        </ext:Panel>
                        <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                          <Items>
                            <ext:Label ID="lbDiscBtm" runat="server" Text="" FieldLabel="Potongan" />
                            <ext:Label ID="lbNetBtm" runat="server" Text="" FieldLabel="Net" />
                          </Items>
                        </ext:Panel>
                        <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".4" Layout="Form" Visible="false">
                          <Items>
                            <ext:Label ID="lbDiscExtraBtm" runat="server" Text="" FieldLabel="Diskon Ekstra" />
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
            <ext:Parameter Name="NumberID" Value="#{hfFaktur}.getValue()" Mode="Raw" />
            <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="gridValuesBea" Value="saveStoreToServer(#{gridDetailBea}.getStore())"
              Mode="Raw" />
            <ext:Parameter Name="Gudang" Value="#{hfGdg}.getValue()" Mode="Raw" />
            <%--<ext:Parameter Name="storeValues" Value="saveStoreToServer(#{bufferStore})" Mode="Raw" />
            <ext:Parameter Name="TanggalNote" Value="#{txTanggalHdr}.getRawValue()" Mode="Raw" />
            <ext:Parameter Name="JenisBayar" Value="#{cbTipeHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="CustomerID" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="CustomerName" Value="#{cbCustomerHdr}.getText()" Mode="Raw" />
            <ext:Parameter Name="TipeBayar" Value="#{cbTipeBayarHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="BankID" Value="#{cbBankHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="RekNo" Value="#{cbRekeningHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GiroID" Value="#{txGiroHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="GiroDate" Value="#{txTempoGiroHdr}.getRawValue()" Mode="Raw" />
            <ext:Parameter Name="KursID" Value="#{cbKursHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="KursValue" Value="#{txKursValue}.getValue()" Mode="Raw" />
            <ext:Parameter Name="JumlahTransaksi" Value="#{txJumlahHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SisaTransaksi" Value="0" />
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdr}.getValue()" Mode="Raw" />--%>
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
    </ext:Button>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
