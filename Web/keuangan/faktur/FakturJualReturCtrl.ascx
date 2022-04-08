<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FakturJualReturCtrl.ascx.cs"
  Inherits="keuangan_pembayaran_FakturJualReturCtrl" %>

<script type="text/javascript">
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

  var autoChangeDateTop = function(oValu, nValu, lb) {
    if (nValu < 0) {
      ShowError('Minimum jumlah tidak boleh lebih kecil dari 0');

      return;
    }

    var dat = new Date(),
      dt = new Date();
    var iVal = (nValu - oValu);

    if (!Ext.isEmpty(lb)) {
      dat = Date.parseDate(lb.getText(), 'd-m-Y');

      if (!Ext.isEmpty(dat)) {
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

  var onBeforeEditGrid = function(e) {
    if (e.record.get('l_void')) {
      e.cancel = true;
    }
  }

  var onAfterEditGrid = function(e) {
    if (!e.record.get('l_void')) {
      e.record.set('l_modified', true);

      recalculateFaktur(e.grid.getStore());
    }
  }

  var recalculateFaktur = function(store) {
    var lbGross = Ext.getCmp('<%= lbGrossBtm.ClientID %>');
    var lbTax = Ext.getCmp('<%= lbTaxBtm.ClientID %>');
    var lbDisc = Ext.getCmp('<%= lbDiscBtm.ClientID %>');
    var lbNet = Ext.getCmp('<%= lbNetBtm.ClientID %>');
    var hfCab = Ext.getCmp('<%= hfCabang.ClientID %>');

    var sumGross = 0,
      sumDisc = 0,
      sumTax = 0,
      sumNet = 0,
      qty = 0, hna = 0, disc = 0, totalHarga;

    store.each(function(r) {
      if (!r.get('l_void')) {
        qty = r.get('n_qty');
        hna = r.get('n_salpri');
        disc = r.get('n_discon');

        total = (qty * hna);

        sumGross += total;
        sumDisc += (total * (disc / 100));
      }
    });

    if (hfCab && ((hfCab.getValue() == '1') || (hfCab.getValue() == 'true'))) {
      sumTax = 0
    }
    else {
      sumTax = ((sumGross - sumDisc) * 0.1);
    }

    sumNet = (sumGross - sumDisc + sumTax);

    lbGross.setText(myFormatNumber(sumGross));
    lbDisc.setText(myFormatNumber(sumDisc));
    lbTax.setText(myFormatNumber(sumTax));
    lbNet.setText(myFormatNumber(sumNet));
  }

  var voidFakturInsertedDataFromStore = function(store, rec) {
    if (rec.get('l_void')) {
      return false;
    }

    voidInsertedDataFromStore(rec, function(txt) {
      rec.set('l_modified', false);
      rec.set('l_void', true);
      rec.set('v_ket', txt);

      recalculateFaktur(store);
    });
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfFaktur" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfCabang" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="130" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="130" AutoScroll="true"
          Layout="Fit">
          <Items>
            <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:DateField ID="txTanggalHdr" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                      Format="dd-MM-yyyy" />
                    <ext:TextField ID="txExFakturHdr" runat="server" AllowBlank="true" FieldLabel="Ex. Faktur" />
                    <ext:Label ID="lbCustomerHdr" runat="server" FieldLabel="Cabang" />
                  </Items>
                </ext:Panel>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
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
                                <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
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
                        <ext:Label ID="Label1" runat="server" Text="&nbsp;" />
                        <ext:NumberField ID="txKursValueHdr" runat="server" AllowBlank="false" AllowDecimals="true"
                          DecimalPrecision="2" AllowNegative="false" Width="75" />
                      </Items>
                    </ext:CompositeField>
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
            <ext:GridPanel ID="gridDetail" runat="server">
              <LoadMask ShowMask="true" />
              <Listeners>
                <BeforeEdit Handler="onBeforeEditGrid(e);" />
                <AfterEdit Handler="onAfterEditGrid(e);" />
                <Command Handler="if (command == 'Void') { voidFakturInsertedDataFromStore(this.getStore(), record); }" />
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
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0106" />
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
                        <ext:RecordField Name="c_type" />
                        <ext:RecordField Name="n_qty" Type="Float" />
                        <ext:RecordField Name="n_disc" Type="Float" />
                        <ext:RecordField Name="n_salpri" Type="Float" />
                        <ext:RecordField Name="n_discon" Type="Float" />
                        <ext:RecordField Name="n_discoff" Type="Float" />
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
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itnam" Header="Nama" Width="250" />
                  <ext:NumberColumn DataIndex="n_qty" Header="Jumlah" Format="0.000,00/i" />
                  <%--<ext:NumberColumn DataIndex="n_disc" Header="Bonus" Format="0.000,00/i" />--%>
                  <ext:NumberColumn DataIndex="n_discon" Header="Potongan" Format="0.000,00/i" Editable="true">
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
                  <ext:CheckColumn DataIndex="l_void" Header="Hapus" Width="50" />
                </Columns>
              </ColumnModel>
            </ext:GridPanel>
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
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndCustomStore(#{frmHeaders});">
          <Confirmation BeforeConfirm="return verifyHeaderAndCustomStore(#{frmHeaders});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfFaktur}.getValue()" Mode="Raw" />
            <ext:Parameter Name="gridValues" Value="Ext.encode(#{gridDetail}.getRowsValues())"
              Mode="Raw" />
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
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
