<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FakturBeliFloatingCtrl.ascx.cs"
  Inherits="keuangan_pembayaran_FakturBeliFloatingCtrl" %>

<script type="text/javascript">
    var storeToDetailGridFloat = function(frm, grid, cbItem, tSalpri, tDisc, tQty, tPPh) {
        if (!frm.getForm().isValid()) {
            ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
            return;
        }

        if (Ext.isEmpty(grid) ||
          Ext.isEmpty(tSalpri) ||
          Ext.isEmpty(tDisc) ||
          Ext.isEmpty(tQty)) {
            ShowWarning("Objek tidak terdefinisi.");
            return;
        }

        var store = grid.getStore();
        if (Ext.isEmpty(store)) {
            ShowWarning("Objek store tidak terdefinisi.");
            return;
        }

        var iteVal = cbItem.getValue();

        var valX = [iteVal];
        var fieldX = ['c_iteno'];

        var pphVal = tPPh.getValue();

        var isDup = findDuplicateEntryGrid(store, fieldX, valX);

        if (!isDup) {
            store.insert(0, new Ext.data.Record({
                'c_iteno': iteVal,
                'v_itnam': cbItem.getText(),
                'n_bea': 0,
                'n_disc': tDisc.getValue(),
                'n_qty': tQty.getValue(),
                'n_salpri': tSalpri.getValue(),
                'n_ppph': pphVal,
                'l_new': true
            }));

            cbItem.reset();
            tSalpri.reset();
            tDisc.reset();
            tQty.reset();

            recalculateFakturFloat(store);
        }
        else {
            ShowError('Data telah ada.');

            return false;
        }
    }

  var storeToDetailGridBeaFloat = function(frm, gridDetailStore, grid, typ, expe, dat, valu) {
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

      recalculateFakturFloat(gridDetailStore);
    }
    else {
      ShowError('Data telah ada.');

      return false;
    }
  }

  //  var changeKursActFloat = function(c, val, t) {
  //    var store = c.getStore();
  //    if (!Ext.isEmpty(store)) {
  //      var idx = store.findExact('c_kurs', val);
  //      if (idx != -1) {
  //        var r = store.getAt(idx);
  //        t.setValue(r.get('n_currency'));
  //      }
  //    }
  //  }

  //  var autoChangeDateTopFloat = function(oValu, nValu, lb, tx, fakt) {
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

  var onBeforeEditGridFloat = function(e) {
    if (e.record.get('l_void')) {
      e.cancel = true;
    }
  }

  var onAfterEditGridFloat = function(e) {
    if (!e.record.get('l_void')) {
      if (!e.record.get('l_new')) {
        e.record.set('l_modified', true);
      }

      recalculateFakturFloat(e.grid.getStore());
    }
  }

  var onBeforeEditGridFloatBea = function(e) {
    if (e.record.get('l_void')) {
      e.cancel = true;
    }
  }

  var onAfterEditGridFloatBea = function(e, storeGridDetail) {
    if (!e.record.get('l_void')) {
      if (!e.record.get('l_new')) {
        e.record.set('l_modified', true);
      }
      //e.record.set('l_modified', true);

      recalculateFakturFloat(e.grid.getStore());
    }
  }

  var recalculateFakturFloat = function(store) {
    var lbGross = Ext.getCmp('<%= lbGrossBtm.ClientID %>');
    var lbTax = Ext.getCmp('<%= lbTaxBtm.ClientID %>');
    var lbDisc = Ext.getCmp('<%= lbDiscBtm.ClientID %>');
    var lbNet = Ext.getCmp('<%= lbNetBtm.ClientID %>');
    var fExtDisc = Ext.getCmp('<%= txExtDiscHdr.ClientID %>');
    var gDetailBea = Ext.getCmp('<%= gridDetailBea.ClientID %>');
    var lbExtDisc = Ext.getCmp('<%= lbExtDiscHdr.ClientID %>');
    var txNPph = Ext.getCmp('<%= txNPph22.ClientID %>');    

    var sumGross = 0,
      sumDisc = 0,
      sumTax = 0,
      sumNet = 0,
      qty = 0, hna = 0, disc = 0, n_bea = 0, nxDisc = 0, totalHarga = 0,
      extDisc = fExtDisc.getValue(),
      sumBea = 0, callNetto = 0
      ppph = 0, sumPpph = 0;

    store.each(function(r) {
      if (!r.get('l_void')) {
        qty = r.get('n_qty');
        hna = r.get('n_salpri');
        disc = r.get('n_disc');
        bea = r.get('n_bea');
        ppph = r.get('n_ppph');

        sumPpph += ppph;
        total = (qty * hna);

        sumGross += (total + n_bea);
        sumDisc += (total * (disc / 100));
      }
    });

    if (!Ext.isEmpty(gDetailBea)) {
      var storeBea = gDetailBea.getStore();

      if (!Ext.isEmpty(storeBea)) {
        sumBea = storeBea.sum('n_value');

        if (sumBea > 0) {
          sumNet = (sumGross - sumDisc);

          store.each(function(r) {
            if (!r.get('l_void')) {
              qty = r.get('n_qty');
              hna = r.get('n_salpri');
              disc = r.get('n_disc');

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

    nxDisc = (sumGross * (extDisc / 100));

    totalHarga = (sumGross - sumDisc - nxDisc);

    sumTax = (totalHarga * 0.1);

    //sumNet = (totalHarga + (sumTax + sumBea));
    sumNet = (totalHarga + sumTax);

    if (!Ext.isEmpty(lbGross)) {
      lbGross.setText(myFormatNumber(sumGross));
    }
    if (!Ext.isEmpty(lbExtDisc)) {
      lbExtDisc.setText(myFormatNumber(nxDisc));
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
  }

  var voidFakturInsertedDataFromStoreFloat = function(store, rec) {
    if (rec.get('l_void')) {
      return false;
    }

    if (rec.get('l_new')) {
      deleteRecordOnStore(store, rec, function(stor) {
        recalculateFakturFloat(stor);
      });
    }
    else {
      voidInsertedDataFromStore(rec, function(txt) {
        rec.set('l_modified', false);
        rec.set('l_void', true);
        rec.set('v_ket', txt);

        recalculateFakturFloat(store);
      });
    }
  }

  //  var validateBeaTypeFloat = function(val, cb) {
  //    if (val == '01') {
  //      cb.enable();
  //    }
  //    else {
  //      cb.reset();
  //      cb.disable();
  //    }
  //  }

  var populateItemComboToDtl = function(o, valu, tSalpri, tDisc, tQty) {
      var stor = o.getStore();

      if (!Ext.isEmpty(stor)) {
          var idx = stor.find('c_iteno', valu);
          if (idx != -1) {
              var r = stor.getAt(idx);

              if (!Ext.isEmpty(tSalpri)) {
                  tSalpri.setValue(r.get('n_salpri'));
              }
              if (!Ext.isEmpty(tDisc)) {
                  tDisc.setValue(r.get('n_disc'));
              }
              if (!Ext.isEmpty(tQty)) {
                  tQty.setValue(r.get('n_qty'));
              }
          }
      }
  }

  var ChangPph = function(grid, NewValue) {


      var ppph = 0;
      var NewPph = NewValue.getValue();

      grid.each(function(r) {
          if ((!r.get('l_void'))) {
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
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfFaktur" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfGdg" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="185" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="185" AutoScroll="true"
          Layout="Fit">
          <Items>
            <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:TextField ID="txFakturHdr" runat="server" AllowBlank="false" FieldLabel="Faktur"
                      Width="250" />
                    <ext:DateField ID="txTanggalHdr" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                      Width="100" Format="dd-MM-yyyy" />
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
                            <ext:Parameter Name="model" Value="2021" />
                            <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                    ['l_aktif = @0', true, 'System.Boolean'],
                                    ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbPemasokHdr}), '']]" Mode="Raw" />
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
                        <Change Handler="clearRelatedComboRecursive(true, #{cbItemDtl});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:CompositeField runat="server" FieldLabel="Pajak">
                      <Items>
                        <ext:TextField ID="txTaxNoHdr" runat="server" AllowBlank="true" FieldLabel="No. Pajak"
                          Width="150" />
                        <ext:DateField ID="txTaxDateHdr" runat="server" AllowBlank="true" FieldLabel="Tanggal Pajak"
                          Width="100" Format="dd-MM-yyyy" />
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
                                <ext:Parameter Name="parameters" Value="[['@contains.c_kurs.Contains(@0) || @contains.v_desc.Contains(@0)', paramTextGetter(#{cbKursHdr}), '']]" Mode="Raw" />
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
                          <Listeners>
                            <Change Handler="changeKursAct(this, newValue, #{txKursValueHdr});" />
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
                            <Change Handler="recalculateFakturFloat(#{gridDetail}.getStore());" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Label runat="server" Text="&nbsp;" />
                        <ext:Label ID="lbExtDiscHdr" runat="server" Text="0" />
                      </Items>
                    </ext:CompositeField>
                    <ext:CompositeField runat="server" FieldLabel="T O P">
                      <Items>
                        <ext:NumberField ID="txTopHdr" runat="server" AllowBlank="false" FieldLabel="T O P"
                          Width="50" AllowDecimals="false" AllowNegative="false">
                          <Listeners>
                            <Change Handler="autoChangeDateTop(oldValue, newValue, #{lbDateTopHdr}, #{txTanggalHdr}, #{hfFaktur}.getValue());" />
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
                            <Change Handler="autoChangeDateTop(oldValue, newValue, #{lbDateTopPjgHdr}, #{txTanggalHdr}, #{hfFaktur}.getValue());" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Label runat="server" Text="&nbsp;" />
                        <ext:Label ID="lbDateTopPjgHdr" runat="server" Text="" />
                      </Items>
                    </ext:CompositeField>
                    <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="PPH 22">
                      <Items>
                        <ext:NumberField ID="txNPph22" runat="server" AllowBlank="false" FieldLabel="PPh 22 (%)"
                          Width="50" AllowDecimals="true" AllowNegative="false" MaxValue="100">
                          <Listeners>
                            <Change Handler="ChangPph(#{gridDetail}.getStore(), this)" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Label ID="Label1" runat="server" Text="&nbsp;" />
                        <ext:Label ID="lbNPph22" runat="server" Text="0" />
                      </Items>
                    </ext:CompositeField>
                    <ext:NumberField ID="txTotalFaktur" runat="server" AllowBlank="true" AllowDecimals="true"
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
                  <TopBar>
                    <ext:Toolbar runat="server">
                      <Items>
                        <ext:FormPanel ID="frmpnlDetailEntryGridDtl" runat="server" Frame="True" Layout="Table"
                          LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                          <Items>
                            <ext:ComboBox ID="cbItemDtl" runat="server" FieldLabel="Item" DisplayField="v_itnam"
                              ValueField="c_iteno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="500"
                              MinChars="3">
                              <Store>
                                <ext:Store runat="server">
                                  <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                      CallbackParam="soaScmsCallback" />
                                  </Proxy>
                                  <BaseParams>
                                    <ext:Parameter Name="start" Value="={0}" />
                                    <ext:Parameter Name="limit" Value="={10}" />
                                    <ext:Parameter Name="model" Value="14012" />
                                    <ext:Parameter Name="parameters" Value="[['suplierId', paramValueGetter(#{cbPemasokHdr}), ''],
                                      ['c_gdg = @0', '1', 'System.Char']]" Mode="Raw" />
                                    <ext:Parameter Name="sort" Value="v_itnam" />
                                    <ext:Parameter Name="dir" Value="ASC" />
                                  </BaseParams>
                                  <Reader>
                                    <ext:JsonReader Root="d.records" SuccessProperty="d.success"
                                      TotalProperty="d.totalRows">
                                      <Fields>
                                        <ext:RecordField Name="c_iteno" />
                                        <ext:RecordField Name="v_itnam" />
                                        <ext:RecordField Name="n_salpri" Type="Float" />
                                        <ext:RecordField Name="n_disc" Type="Float" />
                                        <ext:RecordField Name="n_qty" Type="Float" />
                                      </Fields>
                                    </ext:JsonReader>
                                  </Reader>
                                </ext:Store>
                              </Store>
                              <Template runat="server">
                                <Html>
                                <table cellpading="0" cellspacing="0" style="width: 500px">
                                <tr>
                                <td class="body-panel">Kode</td><td class="body-panel">Nama</td>
                                <td class="body-panel">Harga</td><td class="body-panel">Diskon</td>
                                <td class="body-panel">Jumlah</td>
                                </tr>
                                <tpl for="."><tr class="search-item">
                                <td>{c_iteno}</td><td>{v_itnam}</td>
                                <td>{n_salpri}</td><td>{n_disc}</td>
                                <td>{n_qty}</td>
                                </tr></tpl>
                                </table>
                                </Html>
                              </Template>
                              <Triggers>
                                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                              </Triggers>
                              <Listeners>
                                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                <Change Handler="populateItemComboToDtl(this, newValue, #{txHargaDtl}, #{txDiscDtl}, #{txQtyDtl});" />
                              </Listeners>
                            </ext:ComboBox>
                            <ext:NumberField ID="txHargaDtl" runat="server" AllowBlank="false" AllowDecimals="true"
                              DecimalPrecision="2" AllowNegative="false" FieldLabel="Harga" />
                            <ext:NumberField ID="txDiscDtl" runat="server" AllowBlank="false" AllowDecimals="true"
                              DecimalPrecision="2" AllowNegative="false" FieldLabel="Diskon" MaxValue="100" />
                            <ext:NumberField ID="txQtyDtl" runat="server" AllowBlank="false" AllowDecimals="true"
                              DecimalPrecision="2" AllowNegative="false" FieldLabel="Jumlah" />
                            <ext:Button runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add" Icon="Add">
                              <Listeners>
                                <Click Handler="storeToDetailGridFloat(#{frmpnlDetailEntryGridDtl}, #{gridDetail}, #{cbItemDtl}, #{txHargaDtl}, #{txDiscDtl}, #{txQtyDtl},#{txNPph22});" />
                              </Listeners>
                            </ext:Button>
                            <ext:Button runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                              Icon="Cancel">
                              <Listeners>
                                <Click Handler="#{frmpnlDetailEntryGridDtl}.getForm().reset();" />
                              </Listeners>
                            </ext:Button>
                          </Items>
                        </ext:FormPanel>
                      </Items>
                    </ext:Toolbar>
                  </TopBar>
                  <Items>
                    <ext:GridPanel ID="gridDetail" runat="server">
                      <LoadMask ShowMask="true" />
                      <Listeners>
                        <BeforeEdit Handler="onBeforeEditGridFloat(e);" />
                        <AfterEdit Handler="onAfterEditGridFloat(e);" />
                        <Command Handler="if ((command == 'Void') || (command == 'Delete')) { voidFakturInsertedDataFromStoreFloat(this.getStore(), record); }" />
                        <%--<Command Handler="if (command == 'Void') { voidFakturInsertedDataFromStoreFloat(this.getStore(), record); }" />--%>
                        <%--<Command Handler="if(command == 'Delete') { voidFakturInsertedDataFromStoreFloat(this, record); } else if (command == 'Void') { voidFakturInsertedDataFromStoreFloat(record); }" />--%>
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
                            <ext:Parameter Name="limit" Value="={10}" />
                          </AutoLoadParams>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                            <ext:Parameter Name="model" Value="0103" />
                            <ext:Parameter Name="sort" Value="v_itnam" />
                            <ext:Parameter Name="dir" Value="ASC" />
                            <ext:Parameter Name="parameters" Value="[['fakturNo', paramValueGetter(#{hfFaktur}), 'System.String']]"
                              Mode="Raw" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_iteno" TotalProperty="d.totalRows" Root="d.records"
                              SuccessProperty="d.success">
                              <Fields>
                                <ext:RecordField Name="c_iteno" />
                                <ext:RecordField Name="v_itnam" />
                                <ext:RecordField Name="n_bea" Type="Float" />
                                <ext:RecordField Name="n_disc" Type="Float" />
                                <ext:RecordField Name="n_qty" Type="Float" />
                                <ext:RecordField Name="n_salpri" Type="Float" />
                                <ext:RecordField Name="n_ppph" Type="Float" />
                                <ext:RecordField Name="l_new" Type="Boolean" />
                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                <ext:RecordField Name="l_void" Type="Boolean" />
                                <ext:RecordField Name="v_ket" />
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
                          <ext:NumberColumn DataIndex="n_qty" Header="Jumlah" Format="0.000,00/i" />
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
                              <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                                DecimalPrecision="2" MinValue="0" >
                                <%--<Listeners>
                                  <Change Handler="calcPph22(#{gridDetail}.getStore(), #{txNPph22});" />
                                  <KeyDown Handler="calcPph22(#{gridDetail}.getStore(), #{txNPph22});" />
                                </Listeners>--%>
                              </ext:NumberField>
                            </Editor>
                          </ext:NumberColumn>
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
                            <ext:ComboBox ID="cbExpedisiDtl" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
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
                                <Click Handler="storeToDetailGridBeaFloat(#{frmpnlDetailEntryGridDtlBea}, #{gridDetail}.getStore(), #{gridDetailBea}, #{cbTipeDtl}, #{cbExpedisiDtl}, #{txDateTopBeaDtl}, #{txValueBeaDtl});" />
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
                            <ext:Parameter Name="limit" Value="={10}" />
                          </AutoLoadParams>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="0" />
                            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
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
                        <%--<Added Handler="recalculateFakturFloat(#{gridDetail}.getStore());" />--%>
                        <BeforeEdit Handler="onBeforeEditGridFloatBea(e);" />
                        <AfterEdit Handler="onAfterEditGridFloatBea(e, #{gridDetail}.getStore());" />
                        <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
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
            <%--<ext:Parameter Name="NumberID" Value="#{hfFaktur}.getValue()" Mode="Raw" />--%>
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
