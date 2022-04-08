<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PiutangVCCtrl.ascx.cs"
  Inherits="keuangan_pembayaran_PiutangVCCtrl" %>

<script type="text/javascript">
  var bufferStore;
  var valuSisa = 0,
    valuSisaAwal = 0;

  Ext.onReady(function() {
    collectToBuffer(true);
  });
  
  var changeJenisBayar = function(val, tGiro, tTgl) {
    if (val.trim() == '02') {
      if (!Ext.isEmpty(tGiro)) {
        tGiro.enable();
      }
      if (!Ext.isEmpty(tTgl)) {
        tTgl.enable();
      }
    }
    else {
      if (!Ext.isEmpty(tGiro)) {
        tGiro.reset();
        tGiro.disable();
      }
      if (!Ext.isEmpty(tTgl)) {
        tTgl.reset();
        tTgl.disable();
      }
    }
  }
  var changeTipeBayar = function(val, tJml, cbBank, cbRek) {
    if (val.trim() == '01') {
      if (!Ext.isEmpty(tJml)) {
        tJml.enable();
      }
      if (!Ext.isEmpty(cbBank)) {
        cbBank.enable();
      }
      if (!Ext.isEmpty(cbRek)) {
        cbRek.enable();
      }
    }
    else {
      if (!Ext.isEmpty(tJml)) {
        tJml.reset();
        tJml.disable();
      }
      if (!Ext.isEmpty(cbBank)) {
        cbBank.reset();
        cbBank.disable();
      }
      if (!Ext.isEmpty(cbRek)) {
        cbRek.reset();
        cbRek.disable();
      }
    }
  }
  var changeKursAct = function(c, val, t) {
    var store = c.getStore();
    if (!Ext.isEmpty(store)) {
      var idx = store.findExact('c_kurs', val);
      if (idx != -1) {
        var r = store.getAt(idx);
        t.setValue(r.get('n_currency'));
      }
    }
  }
  var changesJumlah = function(valu, lb, hf) {
    var data = hf.getValue();
    if (Ext.isEmpty(data)) {
      lb.setText(myFormatNumber(valu));
    }
    valuSisaAwal = valuSisa = valu;
  }

  var collectToBuffer = function(clearData) {
    if (Ext.isEmpty(bufferStore)) {
      /*
      bufferStore = new Ext.data.ArrayStore({
      autoDestroy: true,
      storeId: 'bufStore',
      idIndex: 0,
      fields: [
      { name: 'Faktur' },
      { name: 'FakturEx' },
      { name: 'DebitNote' },
      { name: 'CustomerID' },
      { name: 'FakturDate', type: 'date', format: 'M$' },
      { name: 'Value', type: 'float' },
      { name: 'SisaTagihan', type: 'float' },
      { name: 'Pembayaran', type: 'float' },
      { name: 'l_bayar', type: 'boolean' },
      { name: 'l_void', type: 'boolean' },
      { name: 'l_modified', type: 'boolean' },
      { name: 'v_type' },
      { name: 'v_ket' },
      { name: 'isCabang', type: 'boolean' }
      ]
      });
      */

      bufferStore = <%= bufferStore.ClientID %>;
    }

    if (clearData) {
      bufferStore.removeAll();
    }
  }

  var beforeGridEdit = function(e, suplId, lb) {
    var rec = e.record;
    var valuTagihan = 0;
    var valBayar = 0;
    var isBayar = (!e.value);
    var isBaru = rec.get('l_new');
    var valX = '',
      fieldX = '';
    var iRow = 0;
    var recNew = '';
    var fakt = rec.get('Faktur');
    var vTipe = rec.get('Tipe');

    var valX = [fakt, vTipe];
    var fieldX = ['Faktur', 'v_type'];

    var col = e.grid.colModel.columns[e.column];

    var resultOk = false;

    // Cek Pembayaran
    if (col.id == 'cekBayar') {
      if ((valuSisa > 0) && isBayar || ((valuSisa <= 0) && (vTipe == '02') && isBayar)) {
        iRow = storeFindMultiple(bufferStore, fieldX, valX);

        if (iRow == -1) {
          valuTagihan = rec.get('SisaTagihan');

          if (vTipe == '02') {
            valuTagihan = (-valuTagihan);
          }

          if (valuSisa > valuTagihan) {
            valBayar = valuTagihan;
            valuSisa -= valuTagihan;
          }
          else {
            valBayar = valuSisa;
            valuSisa = 0;
          }

          lb.setText(myFormatNumber(valuSisa));
          rec.set('Pembayaran', valBayar);

          recNew = new Ext.data.Record({
            'Faktur': fakt,
            'FakturEx': rec.get('FakturEx'),
            'CustomerID': suplId,
            'FakturDate': rec.get('FakturDate'),
            'Value': rec.get('Value'),
            'SisaTagihan': rec.get('SisaTagihan'),
            'Pembayaran': valBayar,
            'isCabang': rec.get('isCabang'),
            'l_bayar': true,
            'l_new': true,
            'l_void': false,
            'l_modified': false,
            'v_type': vTipe,
            'v_ket': rec.get('v_ket')
          });

          bufferStore.insert(0, recNew);
          bufferStore.commitChanges();

//          e.grid.save();

          resultOk = true;
        }
      }
      else if (!isBayar) {
        iRow = storeFindMultiple(bufferStore, fieldX, valX);

        recNew = bufferStore.getAt(iRow);
        if (!Ext.isEmpty(recNew)) {
          if (recNew.get('l_new')) {
            valBayar = recNew.get('Pembayaran');

            if (vTipe == '02' && (valBayar > 0)) {
              valBayar = (-valBayar);
            }

            var valuTagihan = (valuSisa + valBayar);
            if (valuTagihan >= 0) {
              valuSisa += valBayar;
              valBayar = 0;

              lb.setText(myFormatNumber(valuSisa));
              rec.set('Pembayaran', valBayar);

              bufferStore.remove(recNew);
              bufferStore.commitChanges();

//              e.grid.save();

              resultOk = true;
            }
            else {
              ShowWarning("Maaf, tidak dapat menghapus pembayaran ini " +
                "karena akan membuat voucher pembayaran menjadi minus (-).");
              
              e.cancel = true;
            }
          }
          else {
            e.cancel = true;
          }
        }
        else {
          e.cancel = true;
        }
      }
      else {
        ShowWarning("Maaf, voucher sudah tidak mencukupi.");
        e.cancel = true;
      }
    }
    else if(col.id == 'jmlBayar') {
      isBayar = (e.record.get('l_bayar') || false);
      if(!isBayar) {
        e.cancel = true;
        return;
      }
      var c = (col.editable ? col.editor : null);
    }

    return resultOk;
  }
  
  var afterGridEdit = function(e, suplId, lb) {
    var rec = e.record;
    var valuTagihan = 0;
    var valBayar = 0;
    var isBayar = (!e.value);
    var isBaru = rec.get('l_new');
    var valX = '',
      fieldX = '';
    var iRow = 0;
    var recNew = '';
    var fakt = rec.get('Faktur');
    var vTipe = rec.get('Tipe');

    var valX = [fakt, vTipe];
    var fieldX = ['Faktur', 'v_type'];

    var col = e.grid.colModel.columns[e.column];

    var resultOk = false;

    // Cek Pembayaran
    if (col.id == 'jmlBayar') {
      valBayar = (e.value || 0);
      valuTagihan = (rec.get('SisaTagihan') || 0);
      
      if((vTipe == '01') && ((valBayar < 0) || (valBayar > valuTagihan))) {
        rec.reject();
      }
      else if((vTipe == '02') && ((valBayar > 0) || (valBayar < (-valuTagihan)))) {
        rec.reject();
      }
      else if(valBayar > (e.originalValue + valuSisa)) {
        rec.reject();
      }
      else {
        //recNew = 
        iRow = storeFindMultiple(bufferStore, fieldX, valX);
        if(iRow != -1) {
          recNew = bufferStore.getAt(iRow);
          if(!Ext.isEmpty(recNew)) {
            recNew.set('Pembayaran', valBayar);
            recNew.commit();
            
            bufferStore.commitChanges();
            
            valuTagihan = (e.originalValue - valBayar);
            
            valuSisa += valuTagihan;
            
            lb.setText(myFormatNumber(valuSisa));
            
            rec.commit();
            
            resultOk = true;
          }
        }
      }
    }
    else if (col.id == 'cekBayar') {
      resultOk = true;
    }
    
    if(resultOk) {
      e.grid.save();
    }

    return resultOk;
  }

  var resetDefaultValue = function(lbl, grid, allValue) {
    var store = '',
      hasChanges = false;
    if (!Ext.isEmpty(grid)) {
      store = grid.getStore();
      if(store.getCount() > 0) {        
        store.each(function(r) {
          if(r.get('l_bayar')) {
            r.set('l_bayar', false);
            r.set('Pembayaran', 0);
            
            hasChanges = true;
          }
        });
        
        if(hasChanges) {
          store.commitChanges();
        }
      }
    }
    
    collectToBuffer(true);

    if(allValue) {
      valuSisa = 0;
      valuSisaAwal = 0;
    }
    
    if (!Ext.isEmpty(lbl)) {
      lbl.setText(myFormatNumber(valuSisaAwal));
    }
    
    valuSisa = valuSisaAwal;
  }

  var loadPopulateMergeBuffer = function(storeObject, recs) {
    if (Ext.isEmpty(recs) || Ext.isEmpty(bufferStore) || (bufferStore.getCount() < 1)) {
      return;
    }

    var recDb = '',
      recBuf = '';

    var valX = '',
      fieldX = ['Faktur', 'v_type'],
      iRow = -1;

    for (var i = 0, len = recs.length; i < len; i++) {
      recDb = recs[i];

      valX = [recDb.get('Faktur'), recDb.get('Tipe')];

      iRow = storeFindMultiple(bufferStore, fieldX, valX);
      if (iRow != -1) {
        recBuf = bufferStore.getAt(iRow);

        if (!Ext.isEmpty(recBuf)) {

          recDb.set('Pembayaran', recBuf.get('Pembayaran'));
          recDb.set('l_bayar', true);
        }
      }
    }

    storeObject.commitChanges();
  }
  var autoSelectedFaktur = function(valMoney, g){
    var stor = g.getStore();
    //var r = stor.getAt(0);
    //r.set('l_bayar', true);
    
    if(valMoney > valuSisa) {
      valMoney = valuSisa;
    }
    
    if(valMoney <= 0){
      return;
    }
    
    var fakNo = '';
    var nLoop = 0;
    var col = g.getColumnModel().findColumnIndex('l_bayar');
    var valTagih = 0,
      valBayar = 0;
    
    stor.each(function(rec){
      if(valMoney <= 0) {
        return;
      }
      tipFak = rec.get('Tipe');
      valTagih = rec.get('SisaTagihan');
      if ((tipFak == '01') && (!rec.get('l_bayar')) && (valTagih > 0)) {
        valBayar = (valMoney > valTagih ? valTagih : valMoney);
        valMoney -= valBayar;
        
        rec.set('Pembayaran', valBayar);
        
        g.startEditing(nLoop, col);
        
        g.stopEditing();
        
        rec.set('l_bayar', true);
        
        rec.commit();
      }
      nLoop++;
    });
        
    valuSisa = valMoney;
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfDebit" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Store runat="server" ID="bufferStore">
      <Reader>
        <ext:ArrayReader>
          <Fields>
            <ext:RecordField Name="Faktur" />
            <ext:RecordField Name="FakturEx" />
            <ext:RecordField Name="DebitNote" />
            <ext:RecordField Name="CustomerID" />
            <ext:RecordField Name="FakturDate" Type="Date" DateFormat="M$" />
            <ext:RecordField Name="Value" Type="Float" />
            <ext:RecordField Name="SisaTagihan" Type="Float" />
            <ext:RecordField Name="Pembayaran" Type="Float" />
            <ext:RecordField Name="l_bayar" Type="Boolean" />
            <ext:RecordField Name="l_void" Type="Boolean" />
            <ext:RecordField Name="l_modified" Type="Boolean" />
            <ext:RecordField Name="isCabang" Type="Boolean" />
            <ext:RecordField Name="v_type" />
            <ext:RecordField Name="v_ket" />
          </Fields>
        </ext:ArrayReader>
      </Reader>
    </ext:Store>
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="220" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="220" AutoScroll="true"
          Layout="Fit">
          <Items>
            <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:DateField ID="txTanggalHdr" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                      Width="100" Format="dd-MM-yyyy" />
                    <ext:ComboBox ID="cbTipeHdr" runat="server" FieldLabel="Jenis Bayar" DisplayField="v_ket"
                      ValueField="c_type" ItemSelector="tr.search-item" ListWidth="200" MinChars="3">
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
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '48', 'System.String'],
                                    ['c_portal = @0', '0', 'System.Char']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_notrans" />
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
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 200px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_type}</td><td>{v_ket}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Listeners>
                        <Change Handler="changeJenisBayar(newValue, #{txGiroHdr}, #{txTempoGiroHdr});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbCustomerHdr" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                      FieldLabel="Cabang" Width="255" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
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
                            <ext:Parameter Name="model" Value="2011" />
                            <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                    ['l_stscus = @0', true, 'System.Boolean'],
                                    ['l_cabang = @0', true, 'System.Boolean'],
                                    ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
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
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Listeners>
                        <Change Handler="refreshGrid(#{gridDetailBayarVC});resetDefaultValue(#{lbSisaHdr}, #{gridDetailBayarVC});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbTipeBayarHdr" runat="server" FieldLabel="Tipe Bayar" DisplayField="v_ket"
                      ValueField="c_type" ItemSelector="tr.search-item" ListWidth="200" MinChars="3"
                      Width="100">
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
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '49', 'System.String'],
                                    ['c_portal = @0', '3', 'System.Char']]" Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_notrans" />
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
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 200px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_type}</td><td>{v_ket}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Listeners>
                        <Change Handler="changeTipeBayar(newValue, #{txJumlahHdr}, #{cbBankHdr}, #{cbRekeningHdr});resetDefaultValue(#{lbSisaHdr}, #{gridDetailBayarVC}, true);" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBankHdr" runat="server" DisplayField="v_bank" ValueField="c_bank"
                      FieldLabel="Bank" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
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
                            <ext:Parameter Name="model" Value="2091" />
                            <ext:Parameter Name="parameters" Value="[['c_cab1 = @0', 'X9', 'System.String'],
                              ['@contains.c_bank.Contains(@0) || @contains.v_bank.Contains(@0) || @contains.c_cab1.Contains(@0) || @contains.v_bankcab.Contains(@0)', paramTextGetter(#{cbBankHdr}), '']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_bank" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_bank" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_bank" />
                                <ext:RecordField Name="c_cab1" />
                                <ext:RecordField Name="v_bank" />
                                <ext:RecordField Name="v_bankcab" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 300px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td>
                        <td class="body-panel">Cabang</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_bank}</td><td>{v_bank}</td><td>{v_bankcab}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <Listeners>
                        <Change Handler="clearRelatedComboRecursive(true, #{cbRekeningHdr});" />
                      </Listeners>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbRekeningHdr" runat="server" DisplayField="c_rekno" ValueField="c_rekno"
                      FieldLabel="Rekening" Width="225" MinChars="3" Mode="Local">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2101" />
                            <ext:Parameter Name="parameters" Value="[['c_bank = @0', paramValueGetter(#{cbBankHdr}), 'System.String']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_rekno" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_rekno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_bank" />
                                <ext:RecordField Name="c_rekno" />
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
                  </Items>
                </ext:Panel>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:TextField ID="txGiroHdr" runat="server" FieldLabel="Nomor Giro" MaxLength="50"
                      AllowBlank="true" />
                    <ext:DateField ID="txTempoGiroHdr" runat="server" FieldLabel="Tanggal" Width="100"
                      Format="dd-MM-yyyy" AllowBlank="true" />
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
                                <ext:Parameter Name="parameters" Value="[['@contains.v_desc.Contains(@0) || @contains.c_kurs.Contains(@0)', paramTextGetter(#{cbKursHdr}), '']]"
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
                          <Listeners>
                            <Change Handler="changeKursAct(this, newValue, #{txKursValueHdr});" />
                          </Listeners>
                        </ext:ComboBox>
                        <ext:Label runat="server" Text="&nbsp;" />
                        <ext:NumberField ID="txKursValueHdr" runat="server" AllowBlank="false" AllowDecimals="true"
                          DecimalPrecision="2" AllowNegative="false" Width="75" />
                      </Items>
                    </ext:CompositeField>
                    <ext:CompositeField runat="server">
                      <Items>
                        <ext:NumberField ID="txJumlahHdr" runat="server" AllowBlank="false" FieldLabel="Jumlah"
                          AllowDecimals="true" DecimalPrecision="2" AllowNegative="false">
                          <Listeners>
                            <Change Handler="changesJumlah(newValue, #{lbSisaHdr}, #{hfDebit});resetDefaultValue(#{lbSisaHdr}, #{gridDetailBayarVC});" />
                          </Listeners>
                        </ext:NumberField>
                        <ext:Button ID="btnAutoCalc" runat="server" Icon="Calculator" ToolTip="Otomatis Cek">
                          <Listeners>
                            <Click Handler="autoSelectedFaktur(#{txJumlahHdr}.getValue(), #{gridDetailBayarVC});" />
                          </Listeners>
                        </ext:Button>
                      </Items>
                    </ext:CompositeField>
                    <ext:Label ID="lbSisaHdr" runat="server" FieldLabel="Sisa" />
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
        <ext:GridPanel ID="gridDetailBayarVC" runat="server">
          <LoadMask ShowMask="true" />
          <Listeners>
            <BeforeEdit Handler="beforeGridEdit(e, #{cbCustomerHdr}.getValue(), #{lbSisaHdr});" />
            <AfterEdit Handler="afterGridEdit(e, #{cbCustomerHdr}.getValue(), #{lbSisaHdr});" />
          </Listeners>
          <SelectionModel>
            <ext:RowSelectionModel SingleSelect="true" />
          </SelectionModel>
          <Store>
            <ext:Store ID="storeDetailBayarVC" runat="server" RemoteSort="true">
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
                <ext:Parameter Name="model" Value="13011" />
                <ext:Parameter Name="sort" Value="FakturTopDate" />
                <ext:Parameter Name="dir" Value="ASC" />
                <ext:Parameter Name="parameters" Value="[['cust', paramValueGetter(#{cbCustomerHdr}), 'System.String'],
                      ['Kurs = @0', paramValueGetter(#{cbKursHdr}), 'System.String'],
                      ['Faktur', paramValueGetter(#{txFakturIDFltr}) + '%', ''],
                      ['FakturEx', paramValueGetter(#{txFakturCustIDFltr}) + '%', ''],
                      ['FakturDate = @0', paramRawValueGetter(#{txDateFakturFltr}) , 'System.DateTime'],
                      ['FakturTopDate = @0', paramRawValueGetter(#{txDateTempoFakturFltr}), 'System.DateTime']]"
                  Mode="Raw" />
              </BaseParams>
              <Reader>
                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                  <Fields>
                    <ext:RecordField Name="Faktur" />
                    <ext:RecordField Name="FakturEx" />
                    <ext:RecordField Name="Kurs" />
                    <ext:RecordField Name="CustomerID" />
                    <ext:RecordField Name="Tipe" />
                    <ext:RecordField Name="FakturDate" Type="Date" DateFormat="M$" />
                    <ext:RecordField Name="FakturTopDate" Type="Date" DateFormat="M$" />
                    <ext:RecordField Name="Value" Type="Float" />
                    <ext:RecordField Name="SisaTagihan" Type="Float" />
                    <ext:RecordField Name="Pembayaran" Type="Float" />
                    <ext:RecordField Name="isCabang" Type="Boolean" />
                    <ext:RecordField Name="l_bayar" Type="Boolean" />
                    <ext:RecordField Name="l_new" Type="Boolean" />
                    <ext:RecordField Name="l_void" Type="Boolean" />
                    <ext:RecordField Name="l_modified" Type="Boolean" />
                    <ext:RecordField Name="v_ket" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
              <SortInfo Field="FakturTopDate" Direction="ASC" />
              <Listeners>
                <Load Handler="loadPopulateMergeBuffer(store, records);" />
              </Listeners>
            </ext:Store>
          </Store>
          <ColumnModel>
            <Columns>
              <ext:CheckColumn DataIndex="l_bayar" ColumnID="cekBayar" Width="25" Editable="true" />
              <ext:Column DataIndex="Faktur" Header="Nomor" Width="75" />
              <ext:Column DataIndex="FakturEx" Header="Ex Faktur" />
              <ext:DateColumn DataIndex="FakturDate" Header="Tanggal" Format="dd-MM-yyyy" />
              <ext:DateColumn DataIndex="FakturTopDate" Header="Tempo" Format="dd-MM-yyyy" />
              <ext:NumberColumn DataIndex="Value" Header="Tagihan" Format="0.000,00/i" />
              <ext:NumberColumn DataIndex="SisaTagihan" Header="Sisa Tagihan" Format="0.000,00/i" />
              <ext:NumberColumn DataIndex="Pembayaran" ColumnID="jmlBayar" Header="Pembayaran"
                Format="0.000,00/i" Editable="true">
                <Editor>
                  <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="true"
                    DecimalPrecision="2">
                    <Listeners>
                      <Focus Handler="this.selectText();" />
                    </Listeners>
                  </ext:NumberField>
                </Editor>
              </ext:NumberColumn>
            </Columns>
          </ColumnModel>
          <BottomBar>
            <ext:PagingToolbar ID="gmPagingBB" runat="server" PageSize="20">
              <Items>
                <ext:Label runat="server" Text="Page size:" />
                <ext:ToolbarSpacer runat="server" Width="10" />
                <ext:ComboBox runat="server" Width="80" SelectedIndex="2">
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
          <View>
            <ext:GridView runat="server" StandardHeaderRow="true">
              <HeaderRows>
                <ext:HeaderRow>
                  <Columns>
                    <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                      <Component>
                        <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                          <Listeners>
                            <Click Handler="clearFilterGridHeader(#{gridDetailBayarVC}, #{txFakturIDFltr}, #{txFakturCustIDFltr}, #{txDateFakturFltr}, #{txDateTempoFakturFltr});reloadFilterGrid(#{gridDetailBayarVC});"
                              Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:Button>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:TextField ID="txFakturIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                          <Listeners>
                            <KeyUp Handler="reloadFilterGrid(#{gridDetailBayarVC})" Buffer="700" Delay="700" />
                          </Listeners>
                        </ext:TextField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:TextField ID="txFakturCustIDFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                          <Listeners>
                            <KeyUp Handler="reloadFilterGrid(#{gridDetailBayarVC})" Buffer="700" Delay="700" />
                          </Listeners>
                        </ext:TextField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:DateField ID="txDateFakturFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                          AllowBlank="true">
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridDetailBayarVC})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:DateField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:DateField ID="txDateTempoFakturFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                          AllowBlank="true">
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridDetailBayarVC})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:DateField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn />
                    <ext:HeaderColumn />
                    <ext:HeaderColumn />
                  </Columns>
                </ext:HeaderRow>
              </HeaderRows>
            </ext:GridView>
          </View>
        </ext:GridPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndCustomStore(#{frmHeaders},#{bufferStore});">
          <Confirmation BeforeConfirm="return verifyHeaderAndCustomStore(#{frmHeaders},#{bufferStore});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="" />
            <ext:Parameter Name="storeValues" Value="saveStoreToServer(#{bufferStore})" Mode="Raw" />
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
            <ext:Parameter Name="KursValue" Value="#{txKursValueHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="JumlahTransaksi" Value="#{txJumlahHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SisaTransaksi" Value="0" />
            <ext:Parameter Name="Keterangan" Value="#{txKeteranganHdr}.getValue()" Mode="Raw" />
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
