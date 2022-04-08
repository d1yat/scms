<%@ Page Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="Ekspedisi.aspx.cs"
  Inherits="transaksi_pengiriman_Ekspedisi" %>

<%@ Register Src="EkspedisiPrintCtrl.ascx" TagName="EkspedisiPrintCtrl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
  
    
//    var enterKeyPressHandler = function(f, e) {

//      var i = Ext.getCmp('cbDODtl').getValue();

//      if (this.getText().Length > 9) {
//        Ext.StoreMgr.items[8].insert(0, new Ext.data.Record({
//          'c_dono': this.getText(),
//          'l_new': true
//        }));
//      }

//      if (e.getKey() == 13) {
//        this.setValue(this.getValue());
//        //e.stopEvent();
//        //var sd = Ext.Net.getCmp('txKetHdr').getValue();
//      }
//    }

//    var enterKeyPressHandler = function(f, e) {
//      if (e.getKey() == 13) {
//        this.setValue(this.getValue());
//        e.stopEvent();
//        //var sd = Ext.Net.getCmp('txKetHdr').getValue();
//        Ext.StoreMgr.items[8].insert(0, new Ext.data.Record({
//          'c_dono': this.getValue(),
//          'l_new': true
//        }));


//      }
    //    }


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
      
           

//      if(berat.getValue() == ""){
//      if(berat.getValue() == "0"){}
//      else
//      {ShowWarning("Inputan berat kosong");
//       return;}}
//      
//      if(koli.getValue() == ""){
//      if(koli.getValue() == "0"){}
//      else
//      {ShowWarning("Inputan Koli kosong");
//       return;}}
//       
//       if(receh.getValue() == ""){
//      if(receh.getValue() == "0"){}
//      else
//      {ShowWarning("Inputan Receh kosong");
//       return;}}
//       
//      if(vol.getValue() == ""){
//      if(vol.getValue() == "0"){}
//      else
//      {ShowWarning("Inputan Volume kosong");
//       return;}}
            
      var store = grid.getStore();
      var store2 = grid2.getStore();

      if (Ext.isEmpty(store) ||
          Ext.isEmpty(store2)) {
        ShowWarning("Objek store tidak terdefinisi.");
        return;
      }
      
       var c_dono = dono.getValue();
//       if (c_dono.length != 10) {
//            ShowWarning("No tidak terdefinisi.");
//            return false;
//       }

      var valX = [dono.getValue()];
      var fieldX = ['c_dono'];

      var isDup = findDuplicateEntryGrid(store, fieldX, valX);
      if (!isDup ) {
      
          var i = 0;
          var isCheck = false;
//Indra 20170828          
//          var nKoliCheck = 0;
//          var nRecehCheck = 0;
//          var nBeratCheck = 0;
//          var nVolCheck = 0;
          var nKoliCheck = 0.00;
          var nRecehCheck = 0.00;
          var nBeratCheck = 0.00;
          var nVolCheck = 0.00;
          var nTotkoli = 0.00;
          var nTotreceh = 0.00;
          var nTotberat = 0.00;
          var nTotvol = 0.00000;
          var nPart;
          var nHit;
          var nHitAvailable = 0;
          
          for (i= 0; i < store2.data.items.length; i++)
          {
            if (store2.data.items[i].data.chk1)
            {
              isCheck = true;
                        
              nKoliCheck = store2.data.items[i].data.n_koli;
              nRecehCheck = store2.data.items[i].data.n_receh;
              nBeratCheck = store2.data.items[i].data.n_berat;
              nVolCheck = store2.data.items[i].data.n_vol;
              nTotkoli = nTotkoli + nKoliCheck;
              nTotreceh = nTotreceh + nRecehCheck;
              nTotberat = nTotberat + nBeratCheck;
              nTotvol = nTotvol + nVolCheck;
               
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
                            
          //var n_berat = (berat.getValue() == 0 ? nBeratCheck : berat.getValue());
          //var n_koli = (koli.getValue()== 0 ? nKoliCheck : koli.getValue());
          
//Indra 20170608          
//          var n_berat = berat.getValue();
//          var n_koli = koli.getValue();
//          var n_receh = receh.getValue();
//          var n_vol = vol.getValue();

//Indra 20170828          
//          var n_berat = 0;
//          var n_koli = 0;
//          var n_receh = 0;
//          var n_vol = 0;
          var n_berat = 0.00;
          var n_koli = 0.00;
          var n_receh = 0.00;
          var n_vol = 0.00;
          
          n_berat = berat.getValue();
          n_koli = koli.getValue();
          n_receh = receh.getValue();
          n_vol = vol.getValue();
          
          if(n_berat == ""){
//          n_berat = 0;
          n_berat = 0.00;
          }
          if(n_koli == ""){
//          n_koli = 0;
          n_koli = 0.00;
          }
          if(n_receh == ""){
//          n_receh = 0;
          n_receh = 0.00;
          }
          if(n_vol == ""){
//          n_vol = 0;
          n_vol = 0.00;
          }
          
//Indra 20170608           
//          var TotalKoli = totKoli.getValue() + n_koli;
//          var TotalReceh = totReceh.getValue() + n_receh;
//          var TotalBerat = totBerat.getValue() + n_berat;
//          var TotalVol = totVol.getValue() + n_vol;

//Indra 20170828           
//          var TotalKoli = 0;
//          var TotalReceh = 0;
//          var TotalBerat = 0;
//          var TotalVol = 0;
          var TotalKoli = 0.00;
          var TotalReceh = 0.00;
          var TotalBerat = 0.00;
          var TotalVol = 0.00;
          
//          TotalKoli = parseInt(totKoli.getValue()) + parseInt(n_koli);
//          TotalReceh = parseInt(totReceh.getValue()) + n_receh;
//          TotalBerat = parseInt(totBerat.getValue()) + n_berat;
//          TotalVol = parseInt(totVol.getValue()) + n_vol;
          
          TotalKoli = parseFloat(totKoli.getValue()) + parseFloat(n_koli);
          TotalReceh = parseFloat(totReceh.getValue()) + n_receh;
          TotalBerat = parseFloat(totBerat.getValue()) + n_berat;
          TotalVol = parseFloat(totVol.getValue()) + n_vol;
          
          totKoli.setValue(TotalKoli);
          totReceh.setValue(TotalReceh);
          totBerat.setValue(TotalBerat);
          totVol.setValue(TotalVol);
          
          totKoli.setValue(nTotkoli);
          totReceh.setValue(nTotreceh);
          totBerat.setValue(nTotberat);
          totVol.setValue(nTotvol);

          if (!isCheck)
          {
              nHit++;
          }
          
          nPart = FormatNumberLength(nHit, 3) ;
          
          
          
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

    var deleteOnGrid = function(grid, rec) {
        var store = grid.getStore();
        store.remove(rec);
    }

//    var printShipment = function(print,reprint)
//    {
//        ShowConfirm('Cetak ?', 'Anda yakin ingin mencetak data ini ?', function(btn) 
//        {
//          if (btn == 'yes') 
//          {
//            if (print.getValue() == "True")
//            {          
//                ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dicetak ulang ?', function(btnP, txt) 
//                {
//                  if (btnP == 'ok') 
//                  {
//                    if (txt.trim().length < 1) 
//                    {
//                      txt = 'cetak ulang tanpa alasan.';
//                    }
//                    reprint.setValue(txt);              
//                  }
//                });   
//            }
//          }
//        });
//    }
    
    var voidInsertedDataFromStoreEP = function(rec, grid2, totKoli, totReceh, totBerat, totVol) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      var isVoid = rec.get('l_void');

      if (isVoid) {
        ShowWarning('Item ini telah di batalkan.');
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
                    if (!store2.data.items[i].data.l_void)
                    {
                    
//Indra 20170608                   
//                      var nKoliCheck = store2.data.items[i].data.n_koli;
//                      var nRecehCheck = store2.data.items[i].data.n_receh;
//                      var nBeratCheck = store2.data.items[i].data.n_berat;
//                      var nVolCheck = store2.data.items[i].data.n_vol;
//Indra 20170828                       
//                      var nKoliCheck = 0;
//                      var nRecehCheck = 0;
//                      var nBeratCheck = 0;
//                      var nVolCheck = 0;
                      var nKoliCheck = 0.00;
                      var nRecehCheck = 0.00;
                      var nBeratCheck = 0.00;
                      var nVolCheck = 0.00;
                      
//                      nKoliCheck = parseInt(store2.data.items[i].data.n_koli);
//                      nRecehCheck = parseInt(store2.data.items[i].data.n_receh);
//                      nBeratCheck = parseInt(store2.data.items[i].data.n_berat);
//                      nVolCheck = parseInt(store2.data.items[i].data.n_vol);
                      
                      nKoliCheck = parseFloat(store2.data.items[i].data.n_koli);
                      nRecehCheck = parseFloat(store2.data.items[i].data.n_receh);
                      nBeratCheck = parseFloat(store2.data.items[i].data.n_berat);
                      nVolCheck = parseFloat(store2.data.items[i].data.n_vol);
                      
                      
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

    var voidEXPDataFromStore = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?', function(btn) {
        if (btn == 'yes') {
          ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.', function(btnP, txt) {
            if (btnP == 'ok') {
              if (txt.trim().length < 1) {
                txt = 'Kesalahan pemakai.';
              }

              Ext.net.DirectMethods.DeleteMethod(rec.get('c_expno'), txt);
            }
          });
        }
      });
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

    var cekPilihExp = function(cb, cbExp, driverType, cbDriver, txNoPol, txExp, txNoResiHdr, cbasal, txKoli, txReceh, txBerat, txVol, txBeratVol) {
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
            cbasal.setVisible(false);
        }
        else {
            if (cb.getValue() == '02'){
                if (!Ext.isEmpty(cbExp)) {
                    cbExp.disable();
                    cbExp.clearValue();
                    txNoResiHdr.disable();
                    txNoResiHdr.setValue("");
                    cbasal.setVisible(false);
                }
            driverType.setValue("01"); 
            }           
            else 
            {
                if (cb.getValue() == '03'){
                txExp.setVisible(true);
                txExp.enable();
                cbasal.setVisible(false);
                }
                else 
                {
                    if(cb.getValue() == '4'){
                        cbasal.setVisible(true);
            //            txKoli.setDisabled(false);
            //            txReceh.setDisabled(false);
            //            txBerat.setDisabled(false);
            //            txVol.setDisabled(false);
                        cbExp.setDisabled(false);
                        txNoResiHdr.setDisabled(false);
                        txNoResiHdr.setValue("");
                        txBeratVol.setValue("0");
                    }
                    else
                    {
                        cbasal.setVisible(false);
                        txBerat.setDisabled(true);
                        txReceh.setDisabled(true);
                        txKoli.setDisabled(true);
                        txVol.setDisabled(true);
                        cbExp.setDisabled(true);
                    }
                }
            }
        }
        
        if(cb.getValue() == '03'){
            txExp.setVisible(true);
            txExp.enable();
        }
        else {
        txExp.setValue("");
        txExp.setVisible(false); 
        }
//        if(cb.getValue() == '04'){
//            cbasal.setVisible(true);
////            txKoli.setDisabled(false);
////            txReceh.setDisabled(false);
////            txBerat.setDisabled(false);
////            txVol.setDisabled(false);
//            cbExp.setDisabled(false);
//            txNoResiHdr.setDisabled(false);
//            txNoResiHdr.setValue("");
//            txBeratVol.setValue("0");
//        }
//        else
//        {
//            cbasal.setVisible(false);
////            txKoli.setDisabled(true);
////            txReceh.setDisabled(true);
////            txBerat.setDisabled(true);
////            txVol.setDisabled(true);
//            cbExp.setDisabled(true);
//            txNoResiHdr.setDisabled(true);
//            txNoResiHdr.setValue("");
//            txBeratVol.setValue("0");
//        }
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
    
    var testingsdfg = function(gridsd)
    {
      var kd = gridsd.getSelectionModel();
      var sdf = kd.getSelected().data.c_dono;
      var sd = '';
    };
    
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

//Indra 20170608                     
//                      var nKoliCheck = store2.data.items[i].data.n_koli;
//                      var nRecehCheck = store2.data.items[i].data.n_receh;
//                      var nBeratCheck = store2.data.items[i].data.n_berat;
//                      var nVolCheck = store2.data.items[i].data.n_vol;
//Indra 20170828                        
//                      var nKoliCheck = 0;
//                      var nRecehCheck = 0;
//                      var nBeratCheck = 0;
//                      var nVolCheck = 0;
                      var nKoliCheck = 0.00;
                      var nRecehCheck = 0.00;
                      var nBeratCheck = 0.00;
                      var nVolCheck = 0.00;
                      
//                      nKoliCheck = parseInt(store2.data.items[i].data.n_koli);
//                      nRecehCheck = parseInt(store2.data.items[i].data.n_receh);
//                      nBeratCheck = parseInt(store2.data.items[i].data.n_berat);
//                      nVolCheck = parseInt(store2.data.items[i].data.n_vol);
                      
                      nKoliCheck = parseFloat(store2.data.items[i].data.n_koli);
                      nRecehCheck = parseFloat(store2.data.items[i].data.n_receh);
                      nBeratCheck = parseFloat(store2.data.items[i].data.n_berat);
                      nVolCheck = parseFloat(store2.data.items[i].data.n_vol);
                      
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
    
    var selectedItemDO = function(rec, txKoliInp, txRecehInp) {
        var getkarton = rec.get('n_karton');
        var getreceh = rec.get('n_receh');
        
        txKoliInp.setValue(getkarton);
        txRecehInp.setValue(getreceh);
        }
    
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfGdg" runat="server" />
  <ext:Hidden ID="hfExpNo" runat="server" />
  <ext:Hidden ID="hfNip" runat="server" />
  <ext:Hidden ID="hfTest" runat="server" />
  <ext:Hidden ID="hfDriver" runat="server" />
  <ext:Hidden ID="hfNo" runat="server" />
  <ext:Hidden ID="hfHitPart" runat="server" />
  <ext:Hidden ID="hfPrint" runat="server" /> 
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit">
        <TopBar>
          <ext:Toolbar ID="Toolbar1" runat="server">
            <Items>
              <ext:Button ID="btnAddNew" runat="server" Text="Tambah" Icon="Add">
                <DirectEvents>
                  <Click OnEvent="btnAddNew_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
              <ext:ToolbarSeparator />
              <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
                <Listeners>
                  <Click Handler="refreshGrid(#{gridMain});" />
                </Listeners>
              </ext:Button>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel runat="server" ID="gridMain">
            <LoadMask ShowMask="true" />
            <Listeners>
              <Command Handler="if(command == 'Delete') { voidEXPDataFromStore(record); }" />
            </Listeners>
            <DirectEvents>
              <Command OnEvent="GridMainCommand" Before="if(command != 'Select') { return false; }">
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Parameter" Value="c_expno" />
                  <ext:Parameter Name="PrimaryID" Value="record.data.c_expno" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="true" />
            </SelectionModel>
            <Store>
              <ext:Store ID="strGridMain" runat="server" RemotePaging="true" RemoteSort="true"
                SkinID="OriginalExtStore">
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
                  <ext:Parameter Name="model" Value="0005" />
                  <ext:Parameter Name="parameters" Value="[['c_expno', paramValueGetter(#{txEXPFltr}) + '%', ''],
                    ['d_expdate = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime'],
                    ['c_cusno = @0', paramValueGetter(#{cbCustomerFltr}) , 'System.String'],
                    ['c_gdg = @0', paramValueGetter(#{hfGdg}) , 'System.Char'],
                    ['c_via = @0', paramValueGetter(#{cbViaFltr}) , 'System.String'],
                    ['c_ref', paramValueGetter(#{txRefFltr}) + '%', ''],
                    ['c_exp = @0', paramValueGetter(#{cbEksFltr}) , 'System.String'],
                    ['c_resi', paramValueGetter(#{txResiFltr}), ''],
                    ['(l_delete == null ? false : l_delete) = @0', 'false' , 'System.Boolean']]" Mode="Raw" />
                    <%--['c_entry = @0', paramValueGetter(#{hfNip}) , 'System.String'],--%>
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="c_expno">
                    <Fields>
                      <ext:RecordField Name="c_expno" />
                      <ext:RecordField Name="d_expdate" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_gdgdesc" />
                      <ext:RecordField Name="v_cunam" />
                      <ext:RecordField Name="v_ketTran" />
                      <ext:RecordField Name="c_ref" />
                      <ext:RecordField Name="v_ketMsek" />
                      <ext:RecordField Name="c_resi" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="c_expno" Direction="DESC" />
              </ext:Store>
            </Store>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50" Resizable="false">
                  <Commands>
                    <ext:GridCommand CommandName="Select" Icon="DatabaseEdit" ToolTip-Title="" ToolTip-Text="Lihat detil transaksi" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="" ToolTip-Text="Hapus transaksi" />
                  </Commands>
                </ext:CommandColumn>
                <ext:Column ColumnID="c_expno" DataIndex="c_expno" Header="Nomor" Hideable="false" />
                <ext:DateColumn ColumnID="d_expdate" DataIndex="d_expdate" Header="Tanggal" Format="dd-MM-yyyy" />
                <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="120" />
                <ext:Column ColumnID="v_cunam" DataIndex="v_cunam" Header="Tujuan" Width="150" />
                <ext:Column ColumnID="v_ketTran" DataIndex="v_ketTran" Header="Via" />
                <ext:Column ColumnID="c_ref" DataIndex="c_ref" Header="Referensi" />
                <ext:Column ColumnID="v_ketMsek" DataIndex="v_ketMsek" Header="Ekspedisi" Width="150" />
                <ext:Column ColumnID="c_resi" DataIndex="c_resi" Header="Resi" />
              </Columns>
            </ColumnModel>
            <View>
              <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
                <HeaderRows>
                  <ext:HeaderRow>
                    <Columns>
                      <ext:HeaderColumn Cls="x-small-editor" AutoWidthElement="false">
                        <Component>
                          <ext:Button ID="ClearFilterButton" runat="server" Icon="Cancel" ToolTip="Clear filter">
                            <Listeners>
                              <Click Handler="clearFilterGridHeader(#{GridMain}, #{txEXPFltr}, #{txDateFltr},#{cbGudang}, #{cbCustomerFltr},#{cbViaFltr}, #{txRefFltr}, #{cbEksFltr}, #{txResiFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txEXPFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDateFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbGudang" runat="server" DisplayField="v_gdgdesc" ValueField="c_gdg"
                            Width="300" AllowBlank="true" ForceSelection="false" TypeAhead="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store runat="server" RemotePaging="false">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="allQuery" Value="true" />
                                  <ext:Parameter Name="model" Value="2031" />
                                  <ext:Parameter Name="parameters" Value="[['@contains.c_gdg.Contains(@0) || @contains.v_gdgdesc.Contains(@0)', paramTextGetter(#{cbGudang}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="c_gdg" />
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                            Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                            AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="false" />
                            </CustomConfig>
                            <Store>
                              <ext:Store runat="server" AutoLoad="false">
                                <Proxy>
                                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                    CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                  <ext:Parameter Name="start" Value="={0}" />
                                  <ext:Parameter Name="limit" Value="={10}" />
                                  <ext:Parameter Name="model" Value="2011" />
                                  <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                  ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
                                    Mode="Raw" />
                                  <ext:Parameter Name="sort" Value="v_cunam" />
                                  <ext:Parameter Name="dir" Value="ASC" />
                                </BaseParams>
                                <Reader>
                                  <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{GridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbViaFltr" runat="server" DisplayField="v_ket" ValueField="c_type"
                            Width="250" TypeAhead="false" AllowBlank="true" ForceSelection="false">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
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
                                                                        ['c_notrans = @0', '16', ''],
                                                                        ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaFltr}), '']]"
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
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txRefFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:ComboBox ID="cbEksFltr" runat="server" DisplayField="v_ket"
                            ValueField="c_exp" WListWidth="275" MinChars="3" AllowBlank="true" ItemSelector="tr.search-item"
                            ForceSelection="false" ListWidth="275" PageSize="10">
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
                            <Store>
                              <ext:Store ID="Store13" runat="server">
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
                                  <ext:Parameter Name="parameters" Value="[['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbEksFltr}), '']]"
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
                            <Template ID="Template8" runat="server">
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
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:ComboBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txResiFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{GridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn />
                    </Columns>
                  </ext:HeaderRow>
                </HeaderRows>
              </ext:GridView>
            </View>
            <BottomBar>
              <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
                <Items>
                  <ext:Label ID="Label1" runat="server" Text="Page size:" />
                  <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
                  <ext:ComboBox ID="cbGmPagingBB" runat="server" Width="80">
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
          </ext:GridPanel>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>
  <ext:Window ID="winDetail" runat="server" Height="480" Width="930" Hidden="true"
    Maximizable="true" MinHeight="520" MinWidth="750" Layout="Fit">
    <Items>
      <ext:BorderLayout ID="bllayout" runat="server">
        <North MinHeight="210" Collapsible="false">
          <ext:FormPanel ID="frmHeaders" Title="Header" runat="server" Layout="Column" MinHeight="285">
            <Items>
              <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5"
                Layout="Form" LabelAlign="Right" Padding="10">
                <Items>
                  <ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
                    ValueField="c_type" Width="150" AllowBlank="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="false" />
                    </CustomConfig>
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
                                              ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbViaHdr}), '']]"
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
                  <ext:ComboBox ID="cbCustomerHdr" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                    ValueField="c_cusno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                    MinChars="3" AllowBlank="false" ForceSelection="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="false" />
                    </CustomConfig>
                    <Store>
                      <ext:Store ID="Store6" runat="server" SkinID="OriginalExtStore">
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
                      <Change Handler="clearRelatedComboRecursive(true, #{cbDODtl}, #{cbWPDtl});#{gridDetail}.getStore().removeAll();" />
                    </Listeners>
                  </ext:ComboBox>
                  <ext:ComboBox ID="cbByHdr" runat="server" FieldLabel="Cara kirim" DisplayField="v_ket" Width="250"
                    ValueField="c_type" MinChars="3" AllowBlank="false" ForceSelection="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="false" />
                    </CustomConfig>
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
                                          ['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbByHdr}), '']]"
                            Mode="Raw" />
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
                    <Listeners>
                      <Select Handler="cekPilihExp(this, #{cbEksHdr}, #{hfDriver}, #{cbDriver}, #{txNoPol}, #{txExp}, #{txNoResiHdr},#{cbasal},#{txKoli},#{txReceh},#{txBerat},#{txVol},#{txBeratVol});" />
                    </Listeners>
                  </ext:ComboBox>
                  <ext:ComboBox ID="cbasal" runat="server" FieldLabel="Asal Kirim" DisplayField="v_cunam"
                    ValueField="c_cusno" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                    MinChars="3" AllowBlank="true" ForceSelection="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="true" />
                    </CustomConfig>
                    <Store>
                      <ext:Store ID="Store14" runat="server" SkinID="OriginalExtStore">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="10" />
                          <ext:Parameter Name="model" Value="5005" />
                          <ext:Parameter Name="parameters" Value="[['c_via', #{cbViaHdr}.getValue() , 'System.String'],
                                    ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbasal}), '']]"
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
                    <Template ID="Template9" runat="server">
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
                      <Change Handler="clearRelatedComboRecursive(true, #{cbDODtl}, #{cbWPDtl});#{gridDetail}.getStore().removeAll();" />
                    </Listeners>
                  </ext:ComboBox>
                  <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Ekspedisi">
                  <Items>
                    <ext:ComboBox ID="cbEksHdr" runat="server" FieldLabel="Ekspedisi" DisplayField="v_ket"
                    ValueField="c_exp" WListWidth="275" MinChars="3" AllowBlank="false" ItemSelector="tr.search-item"
                    ForceSelection="false" ListWidth="275" PageSize="10">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="false" />
                    </CustomConfig>
                    <Store>
                      <ext:Store ID="Store3" runat="server">
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
                    <Template ID="Template2" runat="server">
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
                    <ext:ComboBox ID="cbDriver" runat="server" FieldLabel="Driver" PageSize="10" DisplayField="c_nip" ListWidth="300"
                        ItemSelector="tr.search-item" MinChars="3" ValueField="c_nopol" ForceSelection="false">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="true" />
                    </CustomConfig>
                    <Store>
                      <ext:Store ID="Store8" runat="server" >
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
                    <ext:TextField ID="txNoPol" runat="server" FieldLabel="No.Polisi" Width="75" MaxLength = "8" />
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
                  <ext:NumberField ID="txVol" runat="server" FieldLabel="Volume(M3)" AllowBlank="false"
                    AllowNegative="false" DecimalPrecision="5" />
                  <ext:NumberField ID="txBeratVol" runat="server" FieldLabel="Berat Vol(KG)" AllowBlank="false"
                    AllowNegative="false" />
                  <ext:NumberField ID="txBiayaLain" runat="server" FieldLabel="Biaya Lain - Lain" AllowBlank="false"
                    AllowNegative="false" Visible="false" />
                  <ext:CompositeField ID="CompositeField3" runat="server" FieldLabel="Biaya Ekspedisi">
                  <Items>
                     <ext:NumberField ID="txBiayaExp" runat="server" Text="0" AllowBlank="false"
                     AllowNegative="false" Disabled="true" />
                     <ext:Label ID="Label2" runat="server" Text="- Minimum (Kg) : "/>
                     <ext:Label ID="lbMinExp" runat="server" Text="0" />
                  </Items>
                  </ext:CompositeField>
                  <ext:Label ID="lbTotalBiaya" runat="server" Text="0" FieldLabel="Total Ekspedisi" />
                  <ext:TextField ID="txReprint" runat="server" FieldLabel="Alasan Reprint" width ="200"/>                  
                </Items>
              </ext:Panel>
            </Items>
          </ext:FormPanel>
        </North>
        <Center MinHeight="300">
          <ext:Panel ID="pnlDetailEntry" runat="server" itle="Detail" Height="300" Layout="Fit">
            <TopBar>
              <ext:Toolbar ID="Toolbar2" runat="server">
                <Items>
                  <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                    LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
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
                      <ext:ComboBox ID="cbDODtl" runat="server" FieldLabel="DO" ItemSelector="tr.search-item"
                        DisplayField="c_dono" ValueField="c_dono" MinChars="3" PageSize="10" ListWidth="300"
                        AllowBlank="false" ForceSelection="false">
                        <CustomConfig>
                          <ext:ConfigItem Name="allowBlank" Value="false" />
                        </CustomConfig>
                        <Store>
                          <ext:Store ID="Store7" runat="server" AutoLoad="false">
                            <Proxy>
                              <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                CallbackParam="soaScmsCallback" />
                            </Proxy>
                            <BaseParams>
                              <ext:Parameter Name="start" Value="={0}" />
                              <ext:Parameter Name="limit" Value="10" />
                              <ext:Parameter Name="model" Value="5004" />
                              <ext:Parameter Name="parameters" Value="[['gdg', #{hfGdg}.getValue(), 'System.Char'],
                                                 ['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
                                                 ['nosup', #{cbSupDtl}.getValue(), 'System.String'],
                                                 ['@contains.c_dono.Contains(@0)', paramTextGetter(#{cbDODtl}), '']]"
                                Mode="Raw" />
                              <ext:Parameter Name="sort" Value="c_dono" />
                              <ext:Parameter Name="dir" Value="ASC" />
                            </BaseParams>
                            <Reader>
                              <ext:JsonReader IDProperty="c_dono" Root="d.records" SuccessProperty="d.success"
                                TotalProperty="d.totalRows">
                                <Fields>
                                  <ext:RecordField Name="c_dono" />
                                  <ext:RecordField Name="n_karton" />
                                  <ext:RecordField Name="n_receh" />
                                  <ext:RecordField Name="d_dodate" Type="Date" DateFormat="M$" />
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
                          <td>{c_dono}</td><td>{d_dodate:this.formatDate}</td>
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
                                                    <%--<Render Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbDODtl});" Buffer="1200" Delay="1200" />--%>
                                                    <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                                    <Select Handler="selectedItemDO(record, #{txKoliInp}, #{txRecehInp})" />
                                                    <%--<KeyDown Handler="TrataEnter(#{frmpnlDetailEntry}, #{gridDetail}, #{cbDODtl});"/>--%>
                                                    <%--<KeyDown Fn="TrataEnter"/>--%>
                                                    <%--<KeyPress Handler="TrataEnter(#{frmpnlDetailEntry}, #{gridDetail}, #{cbDODtl});"/>--%>
                                                    <%--<SpecialKey  Fn="enterKeyPressHandler" Buffer="300" Delay="300"/>--%>
                                                    <%--<Focus   Fn="enterKeyPressHandler" Buffer="300" Delay="300"/>--%>
                                                    <%--<Change  Fn="enterKeyPressHandler"/>--%>
                                                    <%--<SpecialKey Handler="enterKeyPressHandlerTest(#{frmpnlDetailEntry}, #{gridDetail}, #{cbDODtl});"  />--%>
                                                </Listeners>
                                                <DirectEvents>
                                                  <SpecialKey Before="return e.getKey() == Ext.EventObject.ENTER;" OnEvent="Submit_scane" Buffer="250" Delay="250">
                                                 <%-- <SpecialKey OnEvent="Submit_scane" Buffer="250" Delay="250">--%>
                                                    <ExtraParams>
                                                      <ext:Parameter Name="DO" Value="#{cbDODtl}.getText()" Mode="Raw" />
                                                      <ext:Parameter Name="Cusno" Value="#{cbCustomerHdr}.getValue()" Mode="Raw" />
                                                      <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                                      <%--<ext:Parameter Name="Grid1" Value="Ext.encode(#{gridDetail2}.getRowsValues(true))" Mode="Raw" />--%>
                                                      <ext:Parameter Name="Grid1" Value="saveStoreToServer(#{gridDetail2}.getStore())" Mode="Raw" />
                                                    </ExtraParams>
                                                  </SpecialKey>
                                                </DirectEvents>
                                            </ext:ComboBox>
                                            <ext:NumberField runat="server" FieldLabel="Berat" AllowNegative="false" AllowDecimals="true"
                                                Width="75" ID="txBeratInp" DecimalPrecision="2">
                                            </ext:NumberField>
                                            <ext:NumberField runat="server" FieldLabel="Koli" AllowNegative="false" AllowDecimals="true"
                                                Width="75" ID="txKoliInp" DecimalPrecision="2"  >
                                            </ext:NumberField>
                                            <ext:NumberField runat="server" FieldLabel="Receh" AllowNegative="false" AllowDecimals="true"
                                                Width="75" ID="txRecehInp" DecimalPrecision="2">
                                            </ext:NumberField>
                                            <ext:NumberField runat="server" FieldLabel="Volume" AllowNegative="false" AllowDecimals="true"
                                                Width="75" ID="txVolInp" DecimalPrecision="5">
                                            </ext:NumberField>
                                            <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                                                Icon="Add">
                                                <%--<DirectEvents>
                            <Click OnEvent="AddBtn_Click">
                                <ExtraParams>
                                  <ext:Parameter Name="NO" Value="#{hfNo}.getValue()" Mode="Raw" />
                                  <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>--%>
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
                                                                       ['cusno', #{cbCustomerHdr}.getValue(), 'System.String'],
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
                                  <ext:Parameter Name="WP" Value="#{cbWPDtl}.getText()" Mode="Raw"/>
                                  <ext:Parameter Name="cusno" Value="#{cbCustomerHdr}.getValue()" Mode="Raw"/>
                                  <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                  <ext:Parameter Name="Grid2" Value="saveStoreToServer(#{gridDetail2}.getStore())" Mode="Raw" />
                                </ExtraParams>
                            </Click>
                        </DirectEvents>
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
                              <ext:RowSelectionModel SingleSelect="true">
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
                                              <ext:RecordField Name="n_berat"  />
                                              <ext:RecordField Name="n_vol"  />
                                              <ext:RecordField Name="chk1" />
                                              <ext:RecordField Name="l_void" Type="Boolean" />
                                              <ext:RecordField Name="l_new" Type="Boolean" />
                                          </Fields>
                                      </ext:JsonReader>
                                  </Reader>
                              </ext:Store>
                          </Store>
                          <ColumnModel>
                              <Columns>
                                  <ext:CommandColumn Width="25">
                                    <Commands>
                                      <%--<ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />--%>
                                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                                    </Commands>
                                    <PrepareToolbar Handler="prepareCommandsDetilBerat(record, toolbar, #{hfExpNo}.getValue());" />
                                  </ext:CommandColumn>
                                  <ext:Column DataIndex="n_koli" Header="Koli" Width="75" />
                                  <ext:Column DataIndex="n_receh" Header="Receh" Width="75" />
                                  <ext:Column DataIndex="n_berat" Header="Berat" Width="75" />
                                  <ext:Column DataIndex="n_vol" Header="Volume" Width="75" />
                                  <ext:Column DataIndex="c_nopart" Header="Kode" Width="75" />
                                  <ext:CheckColumn DataIndex="chk1" Header="Cek" Width="75"  Editable="true">
                                  </ext:CheckColumn>
                                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                  <ext:CheckColumn DataIndex="l_modified" Header="Modify" Width="50" Hidden ="true" />
                              </Columns>
                          </ColumnModel>
                          <Listeners>
                            <Command Handler="if(command == 'Delete') { deleteOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStoreEP(record, #{gridDetail2},#{txKoli},#{txReceh}, #{txBerat}, #{txVol}); }" />
                            <AfterEdit Handler="recalc(#{gridDetail2},#{txKoli},#{txReceh}, #{txBerat}, #{txVol});" />
                          </Listeners>
                          <%--<DirectEvents>
                            <AfterEdit OnEvent="RecalcBtn">
                            <ExtraParams>
                                  <ext:Parameter Name="Grid" Value="saveStoreToServer(#{gridDetail}.getStore())" Mode="Raw" />
                                  <ext:Parameter Name="exp" Value="#{cbEksHdr}.getValue()" Mode="Raw"/>
                                  <ext:Parameter Name="cusno" Value="#{cbCustomerHdr}.getValue()" Mode="Raw"/>
                                  <ext:Parameter Name="tipebiaya" Value="#{cbTipeKrmHdr}.getValue()" Mode="Raw"/>
                             </ExtraParams>
                            </AfterEdit>
                          </DirectEvents>--%>
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
                      <ext:GridPanel ID="gridDetail" runat="server" Layout="Fit" AutoScroll="true"> 
                          <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                          </SelectionModel>
                          <Store>
                            <ext:Store ID="Store5" runat="server" RemotePaging="false" RemoteSort="false" AutoLoad="false">
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
                                    <ext:RecordField Name="n_koli" />
                                    <ext:RecordField Name="n_berat" />
                                    <ext:RecordField Name="n_vol" />
                                    <ext:RecordField Name="c_nopart" />
                                    <ext:RecordField Name="l_void" Type="Boolean" />
                                    <ext:RecordField Name="l_new" Type="Boolean" />
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                              <SortInfo Field="c_nopart" Direction="DESC" />
                            </ext:Store>
                          </Store>
                          <ColumnModel>
                            <Columns>
                              <ext:CommandColumn Width="25">
                                <Commands>
                                  <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                                  <%--<ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />--%>
                                </Commands>
                                <PrepareToolbar Handler="prepareCommandsDetilDO(record, toolbar, #{hfExpNo}.getValue());" />
                              </ext:CommandColumn>
                              <ext:Column DataIndex="c_dono" Header="Nomor DO" Width="150" />
                              <ext:Column DataIndex="c_nopart" Header="Kode" Width="50" />
                              <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                            </Columns>
                          </ColumnModel>
                          <Listeners>
                            <Command Handler="if(command == 'Delete') { deleteOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
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
                  <ext:Parameter Name="cusno" Value="#{cbCustomerHdr}.getValue()" Mode="Raw"/>
                  <ext:Parameter Name="tipebiaya" Value="#{cbTipeKrmHdr}.getValue()" Mode="Raw"/>
             </ExtraParams>
            </Click>
          </DirectEvents>
        </ext:Button>
      <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
        <%--<Listeners>
           <Click Handler="printShipment(#{hfPrint},#{txReprint}); " />
        </Listeners>--%>
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
                  <ext:Parameter Name="cusno" Value="#{cbCustomerHdr}.getValue()" Mode="Raw"/>
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
  <uc1:EkspedisiPrintCtrl ID="eksctrl" runat="server" />
</asp:Content>
