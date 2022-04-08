<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EkspedisiGudangCtrl.ascx.cs"
    Inherits="transaksi_pengiriman_EkspedisiGudangCtrl" %>

<script type="text/javascript">
  var applyFilterGrid = function(grid, grid2) {
      var iPartai = grid.getSelected().data['c_nopart'];
      var store = grid2.getStore();
      store.suspendEvents();
      store.filterBy(getRecordFilter(iPartai));
      store.resumeEvents();
      grid2.getView().refresh(false);                                          
    }
    
    var ClearFilterGrid = function(grid, grid2) {
      var store = grid.getStore();
      store.clearFilter();
      return true;
    };

    var getRecordFilter = function(iPartai) {
      var f = [];

      f.push({
        filter: function(record) {
        return filterNumber(iPartai, "c_nopart", record);
        }
      });

      var len = f.length;

      return function(record) {
        for (var i = 0; i < len; i++) {
          if (!f[i].filter(record)) {
            return false;
          }
        }
        return true;
      };
    };

    var filterNumber = function(value, dataIndex, record) {
      var val = record.get(dataIndex);

      if (!Ext.isEmpty(value, false) && val != value) {
        return false;
      }

      return true;
    };
    
    function FormatNumberLength(num, length) {
        var r = "" + num;
        while (r.length < length) {
            r = "0" + r;
        }
        return r;
    };

  var storeToDetailGrid = function(frm, grid, dono, no, berat, koli, receh, vol, grid2, hit, totKoli, totReceh, totBerat, totVol) {
    if (!frm.getForm().isValid()) {
      ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
      return;
    }

    if (Ext.isEmpty(grid) ||
          Ext.isEmpty(dono) ||
          Ext.isEmpty(grid2)) {
      ShowWarning("Objek tidak terdefinisi.");
      return;
    }

//    if(berat.getValue() == ""){
//    if(berat.getValue() == "0"){}
//    else
//    {ShowWarning("Inputan berat kosong");
//    return;}}

//    if(koli.getValue() == ""){
//    if(koli.getValue() == "0"){}
//    else
//    {ShowWarning("Inputan Koli kosong");
//    return;}}
//       
//    if(vol.getValue() == ""){
//    if(vol.getValue() == "0"){}
//    else
//    {ShowWarning("Inputan Volume kosong");
//    return;}}
    
    var store = grid.getStore();
    var store2 = grid2.getStore();

    if (Ext.isEmpty(store) ||
          Ext.isEmpty(store2)) {
      ShowWarning("Objek store tidak terdefinisi.");
      return;
    }
    
    var c_dono = dono.getValue();
    if (c_dono.length != 10) {
        ShowWarning("No tidak terdefinisi.");
        return false;
    }
        
    var valX = [dono.getValue()];
    var fieldX = ['c_dono'];
        
    var isDup = findDuplicateEntryGrid(store, fieldX, valX);
    if (!isDup ) 
    {
        var i = 0;
        var isCheck = false;
//Indra 20170828
//        var nKoliCheck = 0;
//        var nRecehCheck = 0;
//        var nBeratCheck = 0;
//        var nVolCheck = 0;
        var nKoliCheck = 0.00;
        var nRecehCheck = 0.00;
        var nBeratCheck = 0.00;
        var nVolCheck = 0.00;
        
        var nPart;
        var nHit;
        var nHitAvailable = 0;

        for (i = 0; i < store2.data.items.length; i++) {
          if (store2.data.items[i].data.chk1) {
            isCheck = true;
            
            nKoliCheck = store2.data.items[i].data.n_koli;
            nRecehCheck = store2.data.items[i].data.n_receh;
            nBeratCheck = store2.data.items[i].data.n_berat;
            nVolCheck = store2.data.items[i].data.n_vol;

            store2.data.items[i].data.n_koli += koli.getValue();
            store2.data.items[i].data.n_receh += receh.getValue();
            store2.data.items[i].data.n_berat += berat.getValue();
            store2.data.items[i].data.n_vol += vol.getValue();
            store2.data.items[i].commit();
            nHitAvailable = store2.data.items[i].data.c_nopart;        
            break;
          }
          
          if (nHitAvailable < store2.data.items[i].data.c_nopart)
          {
            nHitAvailable = store2.data.items[i].data.c_nopart;
          }
        }

        if (store2.data.items.length <= 0)
        {
        nHit = 0;
        }
        else if (store2.data.items.length > 0)
        {

        nHit = nHitAvailable == 0 ? hit.getValue() : nHitAvailable;
        }
    
        var valX2 = [nPart];
        var fieldX2 = ['c_nopart'];

        var isDup2 = findDuplicateEntryGrid(store2, fieldX2, valX2);

        //      var n_berat = (berat.getValue() == 0 ? nBeratCheck : berat.getValue());
        //      var n_koli = (koli.getValue()== 0 ? nKoliCheck : koli.getValue());
        var n_berat = berat.getValue();
        var n_koli = koli.getValue();
        var n_receh = receh.getValue();
        var n_vol = vol.getValue();
        var TotalKoli = totKoli.getValue() + n_koli;
        var TotalReceh = totReceh.getValue() + n_receh;
        var TotalBerat = totBerat.getValue() + n_berat;
        var TotalVol = totVol.getValue() + n_vol;

        totKoli.setValue(TotalKoli);
        totReceh.setValue(TotalReceh);
        totBerat.setValue(TotalBerat);
        totVol.setValue(TotalVol);

        if (!isCheck) {
        nHit++;
        }

        nPart = FormatNumberLength(nHit, 3);
        

      store.insert(0, new Ext.data.Record({
        'c_dono': c_dono,
        'n_koli': n_koli,
        'n_receh': n_receh,
        'n_berat': n_berat,
        'n_vol': n_vol,
        'c_nopart': nPart,
        'l_new': true
      }));

        hit.setValue(nHit);
        no.setValue(c_dono);
        dono.reset();
        berat.reset();
        koli.reset();
        receh.reset();
        vol.reset();
        dono.focus();
        
       if (!isDup2 && !isCheck){
          store2.insert(0, new Ext.data.Record({
            'n_koli': n_koli,
            'n_receh': n_receh,
            'n_berat': n_berat,
            'n_vol': n_vol,
            'c_nopart': nPart,
            'l_new': true,
          }));
        };
    }
    else {
      ShowError("Data Telah Ada");
      return;
    }
  }

  var validasiJamResi = function(obj) {
    if (Ext.isEmpty(obj)) {
      return;
    }

    var valu = obj.getValue();
    var tgl = (Ext.isDate(valu) ? valu : Date.parseDate(valu, 'g:i:s'));

    obj.setValue(myFormatTime(tgl));
  }

  var cekPilihDriver = function(cb, nopol) {
      if (!Ext.isEmpty(cb)) {
          var no = cb.getValue();
          nopol.setValue(no);
      }
  }

  var cekPilihExp = function(cb, cbExp, driverType, cbDriver, txNoPol, txExp, txNoResiHdr) {
      if (Ext.isEmpty(cb)) {
          if (!Ext.isEmpty(cbExp)) {
              cbExp.disable();
              cbExp.clearValue();
              txNoResiHdr.disable();
              txNoResiHdr.setValue("");
          }
          return;
      }

      cbDriver.clearValue();
      txNoPol.setValue("");
        
      if (cb.getValue() == '01') { // Tipe Expedisi
          if (!Ext.isEmpty(cbExp)) {
              cbExp.enable();
              cbExp.clearValue();
              txNoResiHdr.enable();
              
          }
          driverType.setValue("02");

      }
      else {
          if (!Ext.isEmpty(cbExp)) {
              cbExp.disable();
              cbExp.clearValue();
              txNoResiHdr.disable();
              txNoResiHdr.setValue("");
          }
          driverType.setValue("01");
      }
      
      if(cb.getValue() == '03'){
        txExp.setVisible(true);
        txExp.enable();
      }
      else {
        txExp.setValue("");
        txExp.setVisible(false); 
      }
  }
  
  var CabangChecked = function(chkCabang, cbCustomer) {
      
      var chk = chkCabang.getValue();
      
      if(chk)
      {
        cbCustomer.show();
        cbCustomer.reset();
      }
      else
      {
        cbCustomer.hide();
        cbCustomer.reset();
      }
  }
      

  var prepareCommandsExpGdg = function(rec, toolbar, valX) {
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
    
    var prepareCommandsDetilDO = function(rec, toolbar, valX) {
      var del = toolbar.items.get(0); // delete button
      //var vd = toolbar.items.get(1); // void button

      var isNew = false;

      if (!Ext.isEmpty(rec)) {
        isNew = rec.get('l_new');
      }

      if (Ext.isEmpty(valX) || isNew) {
        del.setVisible(true);
        //vd.setVisible(false);
      }
      else {
        del.setVisible(false);
        //vd.setVisible(true);
      }
    }
  
  
  var voidInsertedDataFromStoreEG = function(rec, grid2, totKoli, totBerat, totVol) {
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
                  
                  var store2 = grid2.getStore(); 
//Indra 20170828                  
//                  var TotalKoli = 0;
//                  var TotalReceh = 0;
//                  var TotalBerat = 0;
//                  var TotalVol = 0;
                  var TotalKoli = 0.00;
                  var TotalReceh = 0.00;
                  var TotalBerat = 0.00;
                  var TotalVol = 0.00;
                  
                  for (i= 0; i < store2.data.items.length; i++)
                    {
                      if (!store2.data.items[i].data.l_void)
                      {
                        var nKoliCheck = store2.data.items[i].data.n_koli;
                        var nRecehCheck = store2.data.items[i].data.n_receh;
                        var nBeratCheck = store2.data.items[i].data.n_berat;
                        var nVolCheck = store2.data.items[i].data.n_vol;                      
                        TotalKoli += nKoliCheck;
                        TotalReceh += nRecehCheck;
                        TotalBerat += nBeratCheck;
                        TotalVol += nVolCheck;                                          
                      }
                    }
                  totKoli.setValue(TotalKoli);
                  totReceh.setValue(TotalReceh);
                  totBerat.setValue(TotalBerat);
                  totVol.setValue(TotalVol);                
                }
              });
          }
        });
    }
  }
  
  var recalc = function(grid2, totKoli, totReceh, totBerat, totVol) {

                var store2 = grid2.getStore(); 
//Indra 20170828                
//                var TotalKoli = 0;
//                var TotalReceh = 0;
//                var TotalBerat = 0;
//                var TotalVol = 0;
                var TotalKoli = 0.00;
                var TotalReceh = 0.00;
                var TotalBerat = 0.00;
                var TotalVol = 0.00;
                for (i= 0; i < store2.data.items.length; i++)
                  {
                      var nKoliCheck = store2.data.items[i].data.n_koli;
                      var nRecehCheck = store2.data.items[i].data.n_receh;
                      var nBeratCheck = store2.data.items[i].data.n_berat;
                      var nVolCheck = store2.data.items[i].data.n_vol;
                      store2.data.items[i].data.l_modified = true;
                      
                      TotalKoli += nKoliCheck;
                      TotalReceh += nRecehCheck;
                      TotalBerat += nBeratCheck;          
                      TotalVol += nVolCheck;          
                  }
                totKoli.setValue(TotalKoli);
                totReceh.setValue(TotalReceh);
                totBerat.setValue(TotalBerat);
                totVol.setValue(TotalVol);
    }
    
     var deleteOnGrid = function(grid, rec) {
        var store = grid.getStore();
        store.remove(rec);
    }
    
    var selectedItemSJ = function(rec, txKoliInp, txRecehInp) {
        var getkarton = rec.get('n_karton');
        var getreceh = rec.get('n_receh');
        
        txKoliInp.setValue(getkarton);
        txRecehInp.setValue(getreceh);
        }

</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="1000" Hidden="true"
    Maximizable="true" MinHeight="520" MinWidth="800" Layout="Fit">
    <Content>
        <ext:Hidden ID="hfGudang" runat="server" />
        <ext:Hidden ID="hfGudangDesc" runat="server" />
        <ext:Hidden ID="hfExpNo" runat="server" />
        <ext:Hidden ID="hfType" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
        <ext:Hidden ID="hfDriver" runat="server" />
        <ext:Hidden ID="hfNo" runat="server" />
        <ext:Hidden ID="hfHitPart" runat="server" />
        <ext:Hidden ID="hfPrint" runat="server" />
    </Content>
    <Items>
        <ext:BorderLayout ID="bllayout" runat="server">
            <North MinHeight="210" Collapsible="false">
                <ext:FormPanel ID="frmHeaders" Title="Header" runat="server" Layout="Column" MinHeight="285">
                    <Items>
                        <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5"
                            Layout="Form" LabelAlign="Right" Padding="10">
                            <Items>
                                <ext:Label ID="lbGudangFrom" runat="server" FieldLabel="Asal" />
                                <ext:CompositeField ID="CompositeField5" runat="server" FieldLabel="Tujuan">
                                  <Items>
                                     <ext:ComboBox ID="cbGudangHdr" runat="server" FieldLabel="Tujuan" DisplayField="v_gdgdesc"
                                        ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
                                        MinChars="3" AllowBlank="false" ForceSelection="false">
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
                                                    <%--<ext:Parameter Name="start" Value="={0}" />
                                                    <ext:Parameter Name="limit" Value="={10}" />--%>
                                                    <ext:Parameter Name="allQuery" Value="true" />
                                                    <ext:Parameter Name="model" Value="2031" />
                                                    <ext:Parameter Name="parameters" Value="[['c_gdg != @0', #{hfGdg}.getValue(), 'System.Char']]"
                                                        Mode="Raw" />
                                                    <ext:Parameter Name="sort" Value="v_gdgdesc" />
                                                    <ext:Parameter Name="dir" Value="ASC" />
                                                </BaseParams>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                                                        <Fields>
                                                            <ext:RecordField Name="c_gdg" />
                                                            <ext:RecordField Name="v_gdgdesc" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                            </ext:Store>
                                        </Store>
                                        <Template ID="Template1" runat="server">
                                            <Html>
                                            <table cellpading="0" cellspacing="0" style="width: 200px">
                                              <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                                              <tpl for=".">
                                                <tr class="search-item">
                                                  <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                                                </tr>
                                              </tpl>
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
                                     <ext:Label ID="Label1" runat="server" Text="- Cabang: "/>
                                     <ext:Checkbox ID="chkCabang" runat="server" FieldLabel="">
                                     <Listeners>
                                        <Check Handler="CabangChecked(this, #{cbCustomerHdr});" />
                                    </Listeners>
                                     </ext:Checkbox>
                                  </Items>
                                </ext:CompositeField>
                                <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="" DisplayField="v_cunam"
                                    ValueField="c_cusno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                                    MinChars="3" AllowBlank="true" ForceSelection="false" Hidden="true">
                                    <CustomConfig>
                                      <ext:ConfigItem Name="allowBlank" Value="true" />
                                    </CustomConfig>
                                    <Store>
                                      <ext:Store ID="Store3" runat="server" SkinID="OriginalExtStore">
                                        <Proxy>
                                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                            CallbackParam="soaScmsCallback" />
                                        </Proxy>
                                        <BaseParams>
                                          <ext:Parameter Name="start" Value="={0}" />
                                          <ext:Parameter Name="limit" Value="10" />
                                          <ext:Parameter Name="model" Value="5005" />
                                          <ext:Parameter Name="parameters" Value="[['c_via', #{cbViaHdr}.getValue() , 'System.String'],
                                                    ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
                                            Mode="Raw" />
                                          <ext:Parameter Name="sort" Value="v_cunam" />
                                          <ext:Parameter Name="dir" Value="ASC" />
                                        </BaseParams>
                                        <Reader>
                                          <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                                            TotalProperty="d.totalRows">
                                            <Fields>
                                              <ext:RecordField Name="c_cusno" />
                                              <ext:RecordField Name="v_cunam" />
                                              <ext:RecordField Name="c_cab" />
                                            </Fields>
                                          </ext:JsonReader>
                                        </Reader>
                                      </ext:Store>
                                    </Store>
                                    <Template ID="Template2" runat="server">
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
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:CompositeField ID="CompositeField6" runat="server" FieldLabel="Via">
                                  <Items>
                                      <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
                                        ValueField="c_type" Width="150" AllowBlank="false">
                                        <CustomConfig>
                                            <ext:ConfigItem Name="allowBlank" Value="false" />
                                        </CustomConfig>
                                        <Template Visible="False" ID="ctl57" EnableViewState="False">
                                        </Template>
                                        <Store>
                                            <ext:Store ID="Store1" runat="server" RemotePaging="false" SkinID="OriginalExtStore">
                                                <Proxy>
                                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                        CallbackParam="soaScmsCallback" />
                                                </Proxy>
                                                <BaseParams>
                                                    <ext:Parameter Name="allQuery" Value="true" />
                                                    <ext:Parameter Name="model" Value="2001" />
                                                    <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                ['c_notrans = @0', '16', ''],
                                                ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaHdr}), '']]" Mode="Raw" />
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
                                     <ext:Label ID="Label3" runat="server" Text="- Tipe: "/>
                                     <ext:ComboBox ID="cbTipeKrmHdr" runat="server" FieldLabel="Tipe" DisplayField="v_ket"
                                      ValueField="c_type" Width="150" AllowBlank="false">
                                        <CustomConfig>
                                          <ext:ConfigItem Name="allowBlank" Value="false" />
                                        </CustomConfig>
                                        <Store>
                                          <ext:Store ID="Store11" runat="server" RemotePaging="false" SkinID="OriginalExtStore">
                                            <Proxy>
                                              <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                CallbackParam="soaScmsCallback" />
                                            </Proxy>
                                            <BaseParams>
                                              <ext:Parameter Name="allQuery" Value="true" />
                                              <ext:Parameter Name="model" Value="2001" />
                                              <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                                                  ['c_notrans = @0', '005', ''],
                                                                  ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbTipeKrmHdr}), '']]"
                                                Mode="Raw" />
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
                                  </Items>
                                </ext:CompositeField>
                                <ext:ComboBox ID="cbByHdr" runat="server" FieldLabel="Cara kirim" DisplayField="v_ket"
                                    ValueField="c_type" MinChars="3" AllowBlank="false" ForceSelection="false">
                                    <CustomConfig>
                                        <ext:ConfigItem Name="allowBlank" Value="false" />
                                    </CustomConfig>
                                    <Template Visible="False" ID="ctl59" EnableViewState="False">
                                    </Template>
                                    <Store>
                                        <ext:Store ID="Store2" runat="server">
                                            <Proxy>
                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                    CallbackParam="soaScmsCallback" />
                                            </Proxy>
                                            <AutoLoadParams>
                                                <ext:Parameter Name="start" Value="={0}" />
                                                <ext:Parameter Name="limit" Value="={20}" />
                                            </AutoLoadParams>
                                            <BaseParams>
                                                <ext:Parameter Name="start" Value="={0}" />
                                                <ext:Parameter Name="limit" Value="={10}" />
                                                <ext:Parameter Name="model" Value="2001" />
                                                <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                        ['c_notrans = @0', '08', ''],
                                        ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbByHdr}), '']]" Mode="Raw" />
                                                <ext:Parameter Name="sort" Value="v_ket" />
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
                                        <Select Handler="cekPilihExp(this, #{cbEksHdr}, #{hfDriver}, #{cbDriver}, #{txNoPol}, #{txExp}, #{txNoResiHdr});" />
                                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Ekspedisi">
                                    <Items>
                                        <ext:ComboBox ID="cbEksHdr" runat="server" FieldLabel="Ekspedisi" DisplayField="v_ket"
                                            ValueField="c_exp" Width="250" MinChars="3" AllowBlank="false" ItemSelector="tr.search-item"
                                            ForceSelection="false" ListWidth="300" PageSize="10">
                                            <CustomConfig>
                                                <ext:ConfigItem Name="allowBlank" Value="false" />
                                            </CustomConfig>
                                            <Store>
                                                <ext:Store ID="Store5" runat="server">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <AutoLoadParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="={20}" />
                                                    </AutoLoadParams>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="={10}" />
                                                        <ext:Parameter Name="model" Value="5002" />
                                                        <ext:Parameter Name="parameters" Value="[['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbEksHdr}), '']]"
                                                            Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="v_ket" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                                                            TotalProperty="d.totalRows">
                                                            <Fields>
                                                                <ext:RecordField Name="c_exp" />
                                                                <ext:RecordField Name="v_ket" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <Template ID="Template3" runat="server">
                                                <Html>
                                                <table cellpading="0" cellspacing="1" style="width: 400px">
                                            <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                                            <tpl for="."><tr class="search-item">
                                                <td>{c_exp}</td><td>{v_ket}</td>
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
                                        </ext:ComboBox>
                                        <ext:TextField ID="txExp" runat="server" Width="100" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Driver">
                                    <Items>
                                        <ext:ComboBox ID="cbDriver" runat="server" FieldLabel="Driver" PageSize="10" DisplayField="c_nip"
                                            ListWidth="300" ItemSelector="tr.search-item" MinChars="3" ValueField="c_nopol"
                                            ForceSelection="false">
                                            <CustomConfig>
                                                <ext:ConfigItem Name="allowBlank" Value="true" />
                                            </CustomConfig>
                                            <Store>
                                                <ext:Store ID="Store8" runat="server">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <AutoLoadParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="={20}" />
                                                    </AutoLoadParams>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="10" />
                                                        <ext:Parameter Name="model" Value="5006" />
                                                        <ext:Parameter Name="parameters" Value="[['c_type', #{hfDriver}.getValue() , 'System.String'],
                                    ['@contains.c_nip.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbDriver}), '']]"
                                                            Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="c_nip" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                                                            TotalProperty="d.totalRows">
                                                            <Fields>
                                                                <ext:RecordField Name="c_nip" />
                                                                <ext:RecordField Name="v_nama" />
                                                                <ext:RecordField Name="c_nopol" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <Template ID="Template6" runat="server">
                                                <Html>
                                                <table cellpading="0" cellspacing="1" style="width: 200px">
                                                  <tr><td class="body-panel">NIP</td><td class="body-panel">Nama</td><td class="body-panel">No.Polisi</td></tr>
                                                  <tpl for="."><tr class="search-item">
                                                      <td>{c_nip}</td><td>{v_nama}</td><td>{c_nopol}</td>
                                                  </tr></tpl>
                                                </table>
                                                </Html>
                                            </Template>
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                                <Select Handler="cekPilihDriver(this, #{txNoPol});" />
                                                <%--<Change Handler="clearRelatedComboRecursive(true, #{cbDODtl});#{gridDetail}.getStore().removeAll();" />--%>
                                            </Listeners>
                                        </ext:ComboBox>
                                        <ext:TextField ID="txNoPol" runat="server" FieldLabel="No.Polisi" Width="75" MaxLength="8" />
                                    </Items>
                                </ext:CompositeField>
                                <ext:ComboBox ID="cbRefHo" runat="server" FieldLabel="Referensi EP." DisplayField="c_expno"
                                    ValueField="c_expno" Width="150" AllowBlank="true" ForceSelection="false">
                                    <CustomConfig>
                                      <ext:ConfigItem Name="allowBlank" Value="true" />
                                    </CustomConfig>
                                    <Store>
                                      <ext:Store ID="Store12" runat="server" RemotePaging="false" SkinID="OriginalExtStore">
                                        <Proxy>
                                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                            CallbackParam="soaScmsCallback" />
                                        </Proxy>
                                        <BaseParams>
                                          <ext:Parameter Name="allQuery" Value="true" />
                                          <ext:Parameter Name="model" Value="5009" />
                                          <ext:Parameter Name="parameters" Value="[['@contains.c_expno.Contains(@0)', paramTextGetter(#{cbRefHo}), ''],
                                          ['gdg', #{hfGdg}.getValue() , 'System.Char'],
                                          ['EPexcept', #{hfExpNo}.getValue() , 'System.String']
                                          ]"
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
                                    <Triggers>
                                      <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                    </Triggers>
                                    <Listeners>
                                      <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                    </Listeners>
                                </ext:ComboBox>
                                <ext:TextField ID="txKetHdr" runat="server" FieldLabel="Keterangan" Width="250" />
                            </Items>
                        </ext:Panel>
                        <ext:Panel ID="Panel2" runat="server" Border="false" Layout="Form" ColumnWidth=".5"
                            LabelAlign="Right" Padding="10">
                            <Items>
                                <ext:TextField ID="txNoResiHdr" runat="server" FieldLabel="No Resi" AllowBlank="false" />
                                <ext:CompositeField ID="CompositeField4" runat="server" FieldLabel="Tanggal Resi">
                                  <Items>
                                      <ext:DateField ID="txDayResiHdr" runat="server" FieldLabel="Tanggal Resi" AllowBlank="false"
                                        Format="dd-MM-yyyy" />
                                        <ext:Label runat="server" ID="label25" Text=" Jam Resi : " />
                                      <ext:TextField ID="txTimeResiHdr" runat="server" FieldLabel="Jam Resi" MaxLength="8"
                                        AllowBlank="false" Width="75">
                                        <Listeners>
                                          <Change Fn="validasiJamResi" />
                                        </Listeners>
                                        <Plugins>
                                          <ux:InputTextMask Mask="99:99:99" />
                                        </Plugins>
                                      </ext:TextField>
                                  </Items>
                                  </ext:CompositeField>
                                  <ext:NumberField ID="txKoli" runat="server" FieldLabel="Jumlah Koli" AllowBlank="false"
                                    AllowNegative="false" />
                                  <ext:NumberField ID="txReceh" runat="server" FieldLabel="Qty Receh" AllowBlank="false"
                                    AllowNegative="false" />
                                  <ext:NumberField ID="txBerat" runat="server" FieldLabel="Berat Tmbng(Kg)" AllowBlank="false"
                                    AllowNegative="false" />
                                  <ext:NumberField ID="txVol" runat="server" FieldLabel="Volume(M3)" AllowBlank="false" DecimalPrecision = "5"
                                    AllowNegative="false" />
                                  <ext:NumberField ID="txBeratVolume" runat="server" FieldLabel="Berat Vol(Kg)" AllowBlank="false"
                                    AllowNegative="false" />
                                  <ext:NumberField ID="txBiayaLain" runat="server" FieldLabel="Biaya Lain-lain" AllowBlank="false"
                                    AllowNegative="false" Hidden="true" />
                                <ext:CompositeField ID="CompositeField3" runat="server" FieldLabel="Biaya Ekspedisi">
                                  <Items>
                                     <ext:NumberField ID="txBiayaExp" runat="server" Text="0" AllowBlank="false"
                                    AllowNegative="false" Disabled="true" />
                                     <ext:Label ID="Label2" runat="server" Text="- Minimum (Kg) : "/>
                                     <ext:Label ID="lbMinExp" runat="server" Text="0" />
                                  </Items>
                                </ext:CompositeField>
                                <ext:Label ID="lbTotalBiaya" runat="server" Text="0" FieldLabel="Total Ekspedisi" />
                                <ext:TextField ID="txReprint" runat="server" FieldLabel="Alasan Reprint" Width="250" />
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:FormPanel>
            </North>
            <Center MinHeight="300">
                <ext:Panel ID="pnlDetailEntry" runat="server" Title="Daftar Items" Height="300" Layout="Fit">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                                    LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                                    <Items>
                                        <ext:ComboBox ID="cbDODtl" runat="server" FieldLabel="SJ" ItemSelector="tr.search-item"
                                            DisplayField="c_sjno" ValueField="c_sjno" MinChars="3" PageSize="10" ListWidth="300"
                                            AllowBlank="false" ForceSelection="false">
                                            <CustomConfig>
                                                <ext:ConfigItem Name="allowBlank" Value="false" />
                                            </CustomConfig>
                                            <Store>
                                                <ext:Store ID="strDO" runat="server" AutoLoad="false">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="10" />
                                                        <ext:Parameter Name="model" Value="0213" />
                                                        <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                                                           ['gdgTo', #{cbGudangHdr}.getValue(), 'System.Char'],
                                                           ['@contains.c_sjno.Contains(@0)', paramTextGetter(#{cbDODtl}), '']]" Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="c_sjno" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="c_sjno" Root="d.records" SuccessProperty="d.success"
                                                            TotalProperty="d.totalRows">
                                                            <Fields>
                                                                <ext:RecordField Name="c_sjno" />
                                                                <ext:RecordField Name="n_karton" />
                                                                <ext:RecordField Name="n_receh" />
                                                                <ext:RecordField Name="d_sjdate" Type="Date" DateFormat="M$" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <Template ID="Template4" runat="server">
                                                <Html>
                                                <table cellpading="0" cellspacing="1" style="width: 300px">
                                                    <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td></tr>
                                                    <tpl for="."><tr class="search-item">
                                                    <td>{c_sjno}</td><td>{d_sjdate:this.formatDate}</td>
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
                                                <Select Handler="selectedItemSJ(record, #{txKoliInp}, #{txRecehInp})" />
                                            </Listeners>
                                            <DirectEvents>
                                            <SpecialKey OnEvent="Submit_scane" Before="return e.getKey() == Ext.EventObject.ENTER;" Buffer="250" Delay="250">
                                            <%--<SpecialKey OnEvent="Submit_scane" Buffer="300" Delay="300">--%>
                                              <ExtraParams>
                                                <ext:Parameter Name="DO" Value="#{cbDODtl}.getText()" Mode="Raw" />
                                                <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                <ext:Parameter Name="Grid1" Value="saveStoreToServer(#{gridDetail2}.getStore())" Mode="Raw" />                                               
                                              </ExtraParams>
                                            </SpecialKey>
                                          </DirectEvents>
                                        </ext:ComboBox>
                                        <ext:NumberField runat="server" FieldLabel="Berat" AllowNegative="false" AllowDecimals="true"
                                            Width="75" ID="txBeratInp" DecimalPrecision="2">
                                        </ext:NumberField>
                                        <ext:NumberField runat="server" FieldLabel="Koli" AllowNegative="false" AllowDecimals="true"
                                            Width="75" ID="txKoliInp" DecimalPrecision="2" >
                                        </ext:NumberField>
                                        <ext:NumberField runat="server" FieldLabel="Receh" AllowNegative="false" AllowDecimals="true"
                                            Width="75" ID="txRecehInp" DecimalPrecision="2">
                                        </ext:NumberField>
                                        <ext:NumberField runat="server" FieldLabel="Volume" AllowNegative="false" AllowDecimals="true"
                                            Width="75" ID="txVolInp" DecimalPrecision="2">
                                        </ext:NumberField>
                                        <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                                            Icon="Add">
                                          <DirectEvents>
                                          <Click OnEvent="AddBtn_Click">
                                              <ExtraParams>
                                                <ext:Parameter Name="NO" Value="#{hfNo}.getValue()" Mode="Raw" />
                                                <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                              </ExtraParams>
                                          </Click>
                                      </DirectEvents>
                                            <Listeners>
                                                <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbDODtl}, #{hfNo}, #{txBeratInp}, #{txKoliInp}, #{txRecehInp}, #{txVolInp}, #{gridDetail2}, #{hfHitPart}, #{txKoli},#{txReceh}, #{txBerat}, #{txVol});" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:Button ID="btnClear" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Clear"
                                            Icon="Cancel">
                                            <Listeners>
                                                <Click Handler="#{frmpnlDetailEntry}.getForm().reset()" />
                                            </Listeners>
                                        </ext:Button>
                                        <ext:ComboBox ID="cbWPDtl" runat="server" FieldLabel="WP" ItemSelector="tr.search-item"
                                            DisplayField="c_nodoc" ValueField="c_nodoc" MinChars="3" PageSize="10" ListWidth="300"
                                            AllowBlank="false" ForceSelection="false">
                                            <CustomConfig>
                                                <ext:ConfigItem Name="allowBlank" Value="true" />
                                            </CustomConfig>
                                            <Store>
                                                <ext:Store ID="Store9" runat="server" AutoLoad="false">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="10" />
                                                        <ext:Parameter Name="model" Value="5007" />
                                                        <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                                                                     ['cusno', #{cbGudangHdr}.getValue(), 'System.String'],
                                                                     ['@contains.c_nodoc.Contains(@0)', paramTextGetter(#{cbWPDtl}), '']]"
                                                            Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="c_nodoc" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="c_nodoc" Root="d.records" SuccessProperty="d.success"
                                                            TotalProperty="d.totalRows">
                                                            <Fields>
                                                                <ext:RecordField Name="c_nodoc" />
                                                                <ext:RecordField Name="d_date" Type="Date" DateFormat="M$" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <Template ID="Template7" runat="server">
                                                <Html>
                                                <table cellpading="0" cellspacing="1" style="width: 300px">
                                                    <tr><td class="body-panel">Nomor</td><td class="body-panel">Tanggal</td></tr>
                                                    <tpl for="."><tr class="search-item">
                                                    <td>{c_nodoc}</td><td>{d_date:this.formatDate}</td>
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
                                            </Listeners>
                                        </ext:ComboBox>
                                        <ext:Button ID="btnAddWP" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                                            Icon="Add">
                                            <DirectEvents>
                                                <Click OnEvent="AddBtnWP_Click">
                                                    <ExtraParams>
                                                        <ext:Parameter Name="WP" Value="#{cbWPDtl}.getText()" Mode="Raw" />
                                                        <ext:Parameter Name="cusno" Value="#{cbGudangHdr}.getValue()" Mode="Raw" />
                                                        <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                        <ext:Parameter Name="Grid2" Value="saveStoreToServer(#{gridDetail2}.getStore())"
                                                            Mode="Raw" />
                                                    </ExtraParams>
                                                </Click>
                                            </DirectEvents>
                                        </ext:Button>
                                        <ext:Button ID="btnClearWP" runat="server" FieldLabel="&nbsp;" LabelSeparator=" "
                                            ToolTip="Clear" Icon="Cancel">
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
                        <ext:FormPanel ID="pnlGridDetail" runat="server" Layout="ColumnLayout">
                            <%--<TopBar>
                  <ext:Toolbar runat="server">
                    <Items>
                      <ext:Button runat="server" Icon="Erase" Text="Clear Filter">
                        <Listeners>
                          <Click Handler="ClearFilterGrid(#{gridDetail}, #{gridDetail2});" />
                        </Listeners>
                      </ext:Button>
                    </Items>
                  </ext:Toolbar>
                </TopBar>--%>
                            <Items>
                                <ext:Panel ID="Panel3" runat="server" ColumnWidth="0.60" Layout="FitLayout">
                                    <Items>
                                        <ext:GridPanel ID="gridDetail2" runat="server">
                                            <LoadMask ShowMask="true" />
                                            <SelectionModel>
                                                <ext:RowSelectionModel SingleSelect="true" ID="ctl1204">
                                                    <%--<Listeners>
                                    <RowSelect Handler="applyFilterGrid(this, #{gridDetail});" />
                                  </Listeners>--%>
                                                </ext:RowSelectionModel>
                                            </SelectionModel>
                                            <Store>
                                                <ext:Store ID="Store10" runat="server" RemotePaging="false" RemoteSort="false">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="0" />
                                                        <ext:Parameter Name="limit" Value="-1" />
                                                        <ext:Parameter Name="allQuery" Value="true" />
                                                        <ext:Parameter Name="sort" Value="" />
                                                        <ext:Parameter Name="dir" Value="" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                            <Fields>
                                                                <ext:RecordField Name="c_partai" />
                                                                <ext:RecordField Name="c_expno" />
                                                                <ext:RecordField Name="n_koli" />
                                                                <ext:RecordField Name="n_receh" />
                                                                <ext:RecordField Name="c_nopart" />
                                                                <ext:RecordField Name="n_berat" />
                                                                <ext:RecordField Name="n_vol" />
                                                                <ext:RecordField Name="chk1" />
                                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <ColumnModel ID="ctl1205">
                                                <Columns>
                                                    <ext:CommandColumn Width="25">
                                                        <Commands>
                                                            <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />--%>
                                                            <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry">
                                                                <ToolTip Title="Command" Text="Void Entry"></ToolTip>
                                                            </ext:GridCommand>
                                                        </Commands>
                                                        <PrepareToolbar Handler="prepareCommandsDetilBerat(record, toolbar, #{hfExpNo}.getValue());" />
                                                    </ext:CommandColumn>
                                                    <ext:Column DataIndex="n_koli" Header="Koli" Width="75" />
                                                    <ext:Column DataIndex="n_receh" Header="Receh" Width="75" />
                                                    <ext:Column DataIndex="n_berat" Header="Berat" Width="75" />
                                                    <ext:Column DataIndex="n_vol" Header="Volume" Width="75" />
                                                    <ext:Column DataIndex="c_nopart" Header="Kode" Width="75" />
                                                    <ext:CheckColumn DataIndex="chk1" Header="Cek" Width="75" Editable="true">
                                                    </ext:CheckColumn>
                                                    <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                                    <ext:CheckColumn DataIndex="l_modified" Header="Modify" Width="50" Hidden ="true" />                                  
                                                </Columns>
                                            </ColumnModel>
                                            <Listeners>
                                                <Command Handler="if(command == 'Delete') { deleteOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreEG(record, #{gridDetail2},#{txKoli}, #{txBerat}, #{txVol}); }" />
                                                <AfterEdit Handler="recalc(#{gridDetail2},#{txKoli},#{txReceh}, #{txBerat}, #{txVol});" />
                                            </Listeners>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                                <ext:Panel ID="Panel4" runat="server" ColumnWidth="0.40" Layout="FitLayout">
                                  <%--<Content>
                                  <ext:Menu ID="mnuPopup" runat="server">
                                    <Items>
                                      <ext:MenuItem ID="MenuItem1" runat="server" Text="Delete Selected" Icon="Delete">
                                        <Listeners>
                                            <Click Handler="alert(#{gridDetail}.getSelectionModel().getSelectedNode().text);"/>
                                        </Listeners>
                                      </ext:MenuItem>
                                    </Items>
                                  </ext:Menu>
                                </Content>--%>
                                    <Items>
                                        <ext:GridPanel ID="gridDetail" runat="server">
                                            <SelectionModel>
                                                <ext:RowSelectionModel SingleSelect="true" ID="ctl1208" />
                                            </SelectionModel>
                                            <Store>
                                                <ext:Store ID="Store4" runat="server" RemotePaging="false" RemoteSort="false">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="0" />
                                                        <ext:Parameter Name="limit" Value="-1" />
                                                        <ext:Parameter Name="allQuery" Value="true" />
                                                        <ext:Parameter Name="model" Value="0006" />
                                                        <ext:Parameter Name="sort" Value="" />
                                                        <ext:Parameter Name="dir" Value="" />
                                                        <ext:Parameter Name="parameters" Value="[['c_expno = @0', #{hfExpNo}.getValue(), 'System.String']]"
                                                            Mode="Raw" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                                            <Fields>
                                                                <ext:RecordField Name="c_dono" />
                                                                <ext:RecordField Name="c_nopart" />
                                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                    <SortInfo Field="c_nopart" Direction="DESC" />                                                                                  
                                                </ext:Store>
                                            </Store>
                                            <ColumnModel ID="ctl1209">
                                                <Columns>
                                                    <ext:CommandColumn Width="25">
                                                        <Commands>
                                                            <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry">
                                                                <ToolTip Title="Command" Text="Hapus entry"></ToolTip>
                                                            </ext:GridCommand>
                                                            <%--<ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />--%>
                                                        </Commands>
                                                        <PrepareToolbar Handler="prepareCommandsDetilDO(record, toolbar, #{hfExpNo}.getValue());" />
                                                    </ext:CommandColumn>
                                                    <ext:Column DataIndex="c_dono" Header="Kode" Width="150" />
                                                    <ext:Column DataIndex="c_nopart" Header="Kode" Width="50" />
                                                    <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                                </Columns>
                                            </ColumnModel>
                                            <Listeners>
                                                <Command Handler="if(command == 'Delete') { deleteOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                                            </Listeners>
                                        </ext:GridPanel>
                                    </Items>
                                </ext:Panel>
                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
            </Center>
        </ext:BorderLayout>
        </Items>
        <Buttons>
        <ext:Button ID="btnRecalc" runat="server" Icon="Calculator" Text="Kalkulasi Biaya">
        <DirectEvents>
            <Click OnEvent="RecalcBtn" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
             <Confirmation ConfirmRequest="true" Title="Kalkulasi" Message="Kalkulasi biaya ekspedisi?" /> 
            <EventMask ShowMask="true" />  
            <ExtraParams>
                  <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                  <ext:Parameter Name="exp" Value="#{cbEksHdr}.getValue()" Mode="Raw"/>
                  <ext:Parameter Name="cusno" Value="#{cbGudangHdr}.getValue()" Mode="Raw"/>
                  <ext:Parameter Name="tipebiaya" Value="#{cbTipeKrmHdr}.getValue()" Mode="Raw"/>
             </ExtraParams>
            </Click>
          </DirectEvents>
        </ext:Button>
        <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
            <DirectEvents>
                <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                    <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                        ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                        <ext:Parameter Name="NumberID" Value="#{hfExpNo}.getValue()" Mode="Raw" />
                    </ExtraParams>
                </Click>
            </DirectEvents>
        </ext:Button>
        <ext:Button ID="btnSimpan" runat="server" Icon="Disk" Text="Simpan">
        <DirectEvents>
          <Click OnEvent="RecalcBtn" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
            <Confirmation ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." /> 
            <EventMask ShowMask="true" />  
            <ExtraParams>
                  <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                  <ext:Parameter Name="exp" Value="#{cbEksHdr}.getValue()" Mode="Raw"/>
                  <ext:Parameter Name="cusno" Value="#{cbGudangHdr}.getValue()" Mode="Raw"/>
                  <ext:Parameter Name="tipebiaya" Value="#{cbTipeKrmHdr}.getValue()" Mode="Raw"/>
             </ExtraParams>
          </Click>
          <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
            <%--<Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
              ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
            <EventMask ShowMask="true" />--%>
            <ExtraParams>
              <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                Mode="Raw" />
              <ext:Parameter Name="gridValues2" Value="saveStoreToServer(#{gridDetail2}.getStore())"
                Mode="Raw" />
              <ext:Parameter Name="NumberID" Value="#{hfExpNo}.getValue()" Mode="Raw" />
              <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
            </ExtraParams>
          </Click>
        </DirectEvents>
      </ext:Button>
        <ext:Button ID="Button2" runat="server" Icon="Reload" Text="Bersihkan">
        </ext:Button>
        <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
            <Listeners>
                <Click Handler="#{winDetail}.hide();" />
            </Listeners>
        </ext:Button>
    </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
