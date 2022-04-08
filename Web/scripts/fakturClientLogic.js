
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


var autoChangeDateTop = function(oValu, nValu, lb, tx, fakt) {
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
    else if (!Ext.isEmpty(tx)) {
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

var validateBeaType = function(val, cb) {
  if (val == '01') {
    cb.enable();
  }
  else {
    cb.reset();
    cb.disable();
  }
}
