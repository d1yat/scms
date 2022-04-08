var clearRelatedCombo = function(combo, auto) {
  if (!Ext.isEmpty(combo)) {
    combo.clearValue();
    var store = combo.getStore();
    if (!Ext.isEmpty(store)) {
      store.removeAll();
      if ((!Ext.isEmpty(auto)) && auto) {
        store.load();
      }
      else {
        combo.reset();
      }
    }
  }
}

var clearRelatedComboRecursive = function() {
  if (arguments.length < 1) {
    return;
  }

  var auto = arguments[0];
  
  for (x = 1; x < arguments.length; x++) {
    clearRelatedCombo(arguments[x], auto);
  }
}

var clearRelatedComboRecursiveMulti = function() {
  if (arguments.length < 1) {
    return;
  }

  var auto = arguments[0];

  for (x = 1; x < arguments.length; x++) {
    clearRelatedCombo(arguments[x], auto);
  }
}

var myFormatDate = function(v) {
  if (isNaN(v)) {
    return '';
  }
  
  var t = Ext.util.Format.date(v, 'd-M-Y');

  return t;
}

var myFormatTime = function(v) {
  if (isNaN(v)) {
    return '';
  }

  var t = Ext.util.Format.date(v, 'H:i:s');

  return t;
}

var myFormatNumber = function(v) {
  if (isNaN(v)) {
    return '';
  }

  var t = Ext.util.Format.number(v, '0.000,00/i');

  return t;
}

var reloadData = function(o) {
  if (Ext.isEmpty(o)) {
    return;
  }
  var store = o.getStore();
  if (!Ext.isEmpty(store)) {
    store.removeAll();
    store.load();
  }
}

function ShowWarning(m) {
  if (Ext.isEmpty(m)) {
    return;
  }
  
  Ext.Msg.show({
    title: 'Peringatan',
    msg: m,
    buttons: Ext.Msg.OK,
    animEl: 'elId',
    icon: Ext.MessageBox.WARNING
  });
}

function ShowError(m) {
  if (Ext.isEmpty(m)) {
    return;
  }
  
  Ext.Msg.show({
    title: 'Kesalahan',
    msg: m,
    buttons: Ext.Msg.OK,
    animEl: 'elId',
    icon: Ext.MessageBox.ERROR
  });
}

function ShowInformasi(m) {
  if (Ext.isEmpty(m)) {
    return;
  }
  
  Ext.Msg.show({
    title: 'Informasi',
    msg: m,
    buttons: Ext.Msg.OK,
    animEl: 'elId',
    icon: Ext.MessageBox.INFO
  });
}

function ShowMessage(m) {
  if (Ext.isEmpty(m)) {
    return;
  }

  Ext.Msg.show({
    title: 'Informasi',
    msg: m,
    buttons: Ext.Msg.OK,
    animEl: 'elId',
    icon: Ext.MessageBox.NONE
  });
}

function ShowConfirm(t, m, fn) {
  t = (Ext.isEmpty(t) ? 'Konfirmasi' : t);

  if (Ext.isFunction(fn)) {
    Ext.Msg.confirm(t, m, fn);
  }
  else {
    Ext.Msg.confirm(t, m);
  }
}

function ShowAsk(t, m, fn, ml, val) {
  t = (Ext.isEmpty(t) ? 'Input' : t);

  if (Ext.isFunction(fn)) {
    Ext.Msg.prompt(t, m, fn, this, ml, val);
  }
  else {
    Ext.Msg.prompt(t, m, undefined, this, ml, val);
  }
}

var clearFilterGridHeader = function() {
  if (arguments.length < 1) {
    return;
  }

  var grid = arguments[0];

  if (Ext.isEmpty(grid)) {
    return;
  }

  for (x = 1; x < arguments.length; x++) {
    if ((!Ext.isEmpty(arguments[x])) && (Ext.isFunction(arguments[x].reset()))) {
      arguments[x].reset();
    }
  }
}

var reloadFilterGrid = function(grid) {
  if (Ext.isEmpty(grid)) {
    return;
  }

  var store = grid.getStore();
  if (!Ext.isEmpty(store)) {
    store.removeAll();
    store.load();
  }
}

var paramTextGetter = function(o, def) {
  if (Ext.isEmpty(o)) {
    return;
  }

  return (Ext.isEmpty(o.getText()) ? (def || '') : o.getText());
}

var paramValueMultiGetter = function(o) {
  if (Ext.isEmpty(o)) {
    return;
  }

  var delim = (o.delimiter || ',');
  var valu = (o.getValue() || '');
  var arr = '';
  var data = '';

  valu = valu.trim();

  if (valu.length > 0) {
    arr = valu.split(delim);

    for (x = 0, y = arr.length; x < y; x++) {
      data += '\'' + arr[x] + '\'';
      if ((x + 1) < y) {
        data += ';';
      }
    }
  }

  return '[ ' + data + ' ]';
}

var paramValueGetter = function(o, def) {
  if (Ext.isEmpty(o)) {
    return;
  }

  return (Ext.isEmpty(o.getValue()) ? (def || '') : o.getValue());
}

var paramRawValueGetter = function(o, def) {
  if (Ext.isEmpty(o)) {
    return;
  }

  return (Ext.isEmpty(o.getRawValue()) ? (def || '') : o.getRawValue());
}

var deleteRecordOnGrid = function(grid, rec) {
  var store = grid.getStore();

  deleteRecordOnStore(store, rec);
  /*
  ShowConfirm('Hapus ?',
              "Apakah anda yakin ingin menghapus data ini ?",
              function(btn) {
                if (btn == 'yes') {
                  if ((!Ext.isEmpty(store)) && (!Ext.isEmpty(rec))) {
                    store.remove(rec);
                  }
                }
              });
  */
}

var deleteRecordOnStore = function(store, rec, fn) {
  ShowConfirm('Hapus ?',
              "Apakah anda yakin ingin menghapus data ini ?",
              function(btn) {
                if (btn == 'yes') {
                  if ((!Ext.isEmpty(store)) && (!Ext.isEmpty(rec))) {
                    store.remove(rec);
                    
                    if (Ext.isFunction(fn)) {
                      fn(store);
                    }
                  }
                }
              });
}


var clearForm = function(f) {
  if (Ext.isEmpty(f)) {
    return;
  }

  f.getForm().reset();
}

var refreshGrid = function(grid) {
  if (Ext.isEmpty(grid)) {
    return;
  }

  var store = grid.getStore();
  if (Ext.isEmpty(store)) {
    return;
  }

  store.removeAll();
  store.reload();
}

var verifyHeaderAndDetail = function(f, g) {
  if ((!Ext.isEmpty(f)) && (!f.getForm().isValid())) {
    ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
    return false;
  }
  if (!Ext.isEmpty(g)) {
    var store = g.getStore();

    if (!Ext.isEmpty(store)) {
      if (store.getCount() < 1) {
        ShowWarning("Detail data masih kosong.");
        return false;
      }
    }
  }

  return true;
}

var verifyHeaderAndCustomStore = function(f, store) {
  if ((!Ext.isEmpty(f)) && (!f.getForm().isValid())) {
    ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
    return false;
  }

  if (!Ext.isEmpty(store)) {
    if (store.getTotalCount() < 1) {
      ShowWarning("Detail data masih kosong.");
      return false;
    }
  }

  return true;
}

var storeFindMultiple = function(store, arr_kriteria, arr_values, startIndex) {
  startIndex = (Ext.isEmpty(startIndex) || startIndex < 0 ? 0 : startIndex);

  var idx = -1;

  if (Ext.isArray(arr_kriteria) && Ext.isArray(arr_values) && (arr_kriteria.length == arr_values.length)) {
    var tmp = '',
      tmp1 = '',
      vField = '';
    var nPos = 0,
      nFound = 0,
      nLen = arr_kriteria.length;

    idx = store.findBy(function(record, id) {
      if (Ext.isEmpty(record)) {
        return false;
      }

      for (nPos = 0; nPos < nLen; nPos++) {
        vField = arr_kriteria[nPos];
        
        tmp = record.get(vField);
        tmp = (Ext.isEmpty(tmp) ? '' : tmp.toString().trim());
        
        tmp1 = (Ext.isEmpty(arr_values[nPos]) ? '' : (arr_values[nPos]).toString().trim());

        if (tmp == tmp1) {
          nFound++;
        }
        else {
          nFound = 0;

          break;
        }
      }

      if ((nFound != 0) && (nFound >= nLen)) {
        return true;
      }
    }, this, startIndex);
  }

  return idx;
}

var findRowMultiple = function(store, fieldArray, valueArray) {
  var idx = storeFindMultiple(store, fieldArray, valueArray, 0);
  var rec = undefined;
  if (idx != -1) {
    rec = store.getAt(idx);
  }

  return rec;
}

var findDuplicateEntryGrid = function(store, fieldArray, valueArray) {
  return (storeFindMultiple(store, fieldArray, valueArray, 0) != -1);
  
  //  if (Ext.isEmpty(store)) {
  //    return false;
  //  }

  //  if ((Ext.isArray(fieldArray) && Ext.isArray(valueArray)) &&
  //         (fieldArray.length == valueArray.length)) {
  //    var idxTarget = 0;
  //    var r = '';
  //    var idxInTarget = 0,
  //        lenArray = fieldArray.length;
  //    var s1 = '',
  //        s2 = '';

  //    do {
  //      for (var x = 0, ln = lenArray; x < ln; x++) {
  //        if (idxTarget == -1) {
  //          break;
  //        }

  //        idxTarget = store.findExact(fieldArray[x], valueArray[x], idxTarget);
  //        if (idxTarget == -1) {
  //          break;
  //        }
  //        else if (lenArray == 1) {
  //          idxTarget = -1;
  //          idxInTarget = 1;
  //          break;
  //        }
  //        else {
  //          r = store.getAt(idxTarget);
  //          for (var y = x; y < ln; y++, x++) {
  //            s1 = r.get(fieldArray[y]).toString().trim().toLowerCase();
  //            s2 = valueArray[y].toString().trim().toLowerCase();

  //            if (s1 == s2) {
  //              idxInTarget++;
  //            }
  //            else {
  //              x = -1;
  //              idxInTarget = 0;
  //              break;
  //            }
  //          }

  //          if (idxInTarget == lenArray) {
  //            idxTarget = -1;
  //            break;
  //          }

  //          idxTarget++;
  //          if (idxTarget > store.getCount()) {
  //            idxTarget = -1;
  //            break;
  //          }
  //        }
  //      }
  //    } while (idxTarget != -1);
  //  }

  //  return (lenArray == idxInTarget);
}

var storeFindMultipleCollect = function(store, arr_kriteria, arr_values) {
  var resl = '';

  if (Ext.isArray(arr_kriteria) && Ext.isArray(arr_values) && (arr_kriteria.length == arr_values.length)) {
    var tmp = '',
      tmp1 = '',
      vField = '';
    var nPos = 0,
      nFound = 0,
      nLen = arr_kriteria.length;

    resl = store.queryBy(function(record, id) {
      if (Ext.isEmpty(record)) {
        return false;
      }

      for (nPos = 0; nPos < nLen; nPos++) {
        vField = arr_kriteria[nPos];

        tmp = record.get(vField);
        tmp = (Ext.isEmpty(tmp) ? '' : tmp.toString().trim());

        tmp1 = (Ext.isEmpty(arr_values[nPos]) ? '' : (arr_values[nPos]).toString().trim());

        if (tmp == tmp1) {
          nFound++;
        }
        else {
          nFound = 0;

          break;
        }
      }

      if ((nFound != 0) && (nFound >= nLen)) {
        return true;
      }
    });
  }

  return resl;
}

var deleteFromStoreMultiple = function(store, arr_kriteria, arr_values) {
  var col = storeFindMultipleCollect(store, arr_kriteria, arr_values);

  if ((!Ext.isEmpty(col)) && (col.getCount() > 0)) {
    var iChange = 0;

    col.each(function(c) {
      if (!Ext.isEmpty(c)) {
        store.remove(c);

        iChange++;
      }
    });

    if (iChange > 0) {
      store.commitChanges();
    }
  }
}

var filterStoreNE = function(valu, c, fieldName) {
  c.reset();

  var store = c.getStore();
  if (!Ext.isEmpty(store)) {
    store.filter({
      fn: function(r) {
        return (r.get(fieldName) != valu);
      },
      scope: this
    });
  }
}

var resetEntryWhenChange = function(g, f) {
  if (!Ext.isEmpty(g)) {
    g.getStore().removeAll();
  }
  if (!Ext.isEmpty(f)) {
    f.getForm().reset();
  }
}

var calculateMoqDenomination = function(qminord, total) {
  var totalResult = 0;

  if (qminord > 0) {
    var halfDenom = (qminord / 2);
    if (total < halfDenom) {
      totalResult = 0;
    }
    else if ((total >= halfDenom) && (total <= qminord)) {
      totalResult = qminord;
    }
    else if ((total % qminord) < halfDenom) {
      totalResult = (Math.floor(total / halfDenom) * halfDenom);
    }
    else if (((total % qminord) % halfDenom) > 0) {
      totalResult = ((Math.floor(total / halfDenom) + 1) * halfDenom);
    }
    else {
      totalResult = 0;
    }
  }
  else {
    totalResult = total;
  }

  return totalResult;
}

var validasiProses = function(f) {
  if (!f.getForm().isValid()) {
    ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
    return false;
  }
  
  return true;
}

var saveStoreToServer = function(store, onlyChanges) {
  if (Ext.isEmpty(store)) {
    return;
  }

  var temp = [];

  var recs = (onlyChanges ? store.getModifiedRecords() : store.getRange());

  for (var n = 0, len = recs.length; n < len; n++) {
    if (Ext.isEmpty(temp[n])) {
      temp[n] = {};
    }

    temp[n] = recs[n].data;
  }

  return Ext.encode(temp);
}


var voidInsertedDataFromStore = function(rec, fn) {
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
                  if (Ext.isFunction(fn)) {
                    fn(txt);
                  }
                  else {
                    rec.set('l_void', true);
                    rec.set('v_ket', txt);
                  }
                }
              });
          }
        });
  }
}

var prepareGridButtonNRCommands = function(rec, toolbar) {
  var del = toolbar.items.get(0); // delete button
  var vd = toolbar.items.get(1); // void button

  var isNew = false;

  if (!Ext.isEmpty(rec)) {
    isNew = rec.get('l_new');
  }

  if (isNew) {
    del.setVisible(true);
    vd.setVisible(false);
  }
  else {
    del.setVisible(false);
    vd.setVisible(true);
  }
}

var prepareGridButtonCommands = function(rec, toolbar, valX) {
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

var showMaskLoad = function(target, msgLoad, hide) {
  if (Ext.isEmpty(target)) {
    return;
  }

  var bdy = target.getBody();

  if (Ext.isEmpty(bdy)) {
    return;
  }

  var loadMask = new Ext.LoadMask(bdy,
      {
        msg: (Ext.isEmpty(msgLoad) || (msgLoad.trim().length < 0) ? 'Loading...' : msgLoad)
      });

  if (hide) {
    loadMask.hide();
  }
  else {
    loadMask.show();
  }

  return loadMask;
}

var destroyCtrlFromCache = function(container) {
  container.controlsCache = container.controlsCache || [];
  Ext.each(container.controlsCache, function(controlId) {
    var control = Ext.getCmp(controlId);
    if (control && control.destroy) {
      control.destroy();
    }
  });
};

var putCtrlToCache = function(container, controls) {
  container.controlsCache = controls;
};

var comboGetSelectedIndex = function(cb) {
  var r = cb.findRecord((cb.valueField || cb.displayField), cb.getValue());
  return ((!Ext.isEmpty(r)) ? cb.store.indexOf(r) : -1);
}


