<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterEstimasiCostExpCtrl.ascx.cs"
    Inherits="master_expedisi_MasterEstimasiCostExpCtrl" %>

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

    var storeToDetailGrid = function(frm, grid, gudang, custom, udara, darat, ice, kirim, cdd, fuso, tronton, container, cde, l300, minKg, activeDate) {
        if (!frm.getForm().isValid()) {
            ShowWarning("Terdapat kesalahan dari inputan yang isi, silahkan diperbaiki.");
            return;
        }

        if (Ext.isEmpty(grid) ||
          Ext.isEmpty(gudang) ||
          Ext.isEmpty(custom) ||
          Ext.isEmpty(udara.getValue()) ||
          Ext.isEmpty(darat.getValue()) ||
          Ext.isEmpty(kirim.getValue())) {
            ShowWarning("Objek tidak terdefinisi.");
            return;
        }

        if ((udara.getValue() == 0) &&
    (darat.getValue() == 0) &&
    (ice.getValue() == 0) &&
    (cdd.getValue() == 0) &&
    (fuso.getValue() == 0) &&
    (tronton.getValue() == 0) &&
    (container.getValue() == 0) &&
    (cde.getValue() == 0) &&
    (l300.getValue() == 0)) {
            ShowWarning("Estimasi Biaya tidak boleh 0 semua.");
            return;
        }

        var store = grid.getStore();
        if (Ext.isEmpty(store)) {
            ShowWarning("Objek store tidak terdefinisi.");
            return;
        }

        var valX = [custom.getValue()];
        var fieldX = ['c_cusno'];

        //    var isDup = findDuplicateEntryGrid(store, fieldX, valX);

        //    if (!isDup) {

        var tudara = udara.getValue();
        var tdarat = darat.getValue();
        var tice = ice.getValue();
        var tCdd = cdd.getValue();
        var tFuso = fuso.getValue();
        var tTronton = tronton.getValue();
        var tContainer = container.getValue();
        var tCde = cde.getValue();
        var tL300 = l300.getValue();
        var tMinKg = minKg.getValue();
        var cusno = custom.getValue();
        var krmVal = kirim.getValue();
        var nGdg = gudang.getValue();
        var dEffective = activeDate.getValue();
        var gdgdesc = 'Gudang ' + nGdg;


        store.insert(0, new Ext.data.Record({
            'c_gdg': nGdg,
            'v_gdgdesc': gdgdesc,
            'c_cusno': cusno,
            'c_typekrm': krmVal,
            'v_typekrm': kirim.getText(),
            'v_cunam': custom.getText(),
            'n_udara': tudara,
            'n_daratlaut': tdarat,
            'n_icepack': tice,
            'n_cdd': tCdd,
            'n_fuso': tFuso,
            'n_tronton': tTronton,
            'n_container': tContainer,
            'n_cde': tCde,
            'n_l300': tL300,
            'n_expmin': tMinKg,
            'd_effective': dEffective,
            'l_new': true
        }));

        custom.reset();
        udara.reset();
        darat.reset();
        kirim.reset();
        ice.reset();
        cdd.reset();
        fuso.reset();
        tronton.reset();
        container.reset();
        cde.reset();
        l300.reset();
        minKg.reset();

        gudang.focus();
        //    }
        //    else {
        //      ShowError('Data telah ada.');

        //      return false;
        //    }
    }

    var afterEdit = function(e) {
        if (!e.record.get('l_new')) {
            e.record.set('l_modified', true);
        }
    };

    var voidInsertedDataFromStore = function(rec) {
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
                      }
                  });
                }
            });
        }
    }

    var fillDefaultEntry = function(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10) {
        if (!Ext.isEmpty(t1)) {
            t1.setValue(0);
        }
        if (!Ext.isEmpty(t2)) {
            t2.setValue(0);
        }
        if (!Ext.isEmpty(t3)) {
            t3.setValue(0);
        }
        if (!Ext.isEmpty(t4)) {
            t4.setValue(0);
        }
        if (!Ext.isEmpty(t5)) {
            t5.setValue(0);
        } 
        if (!Ext.isEmpty(t6)) {
            t6.setValue(0);
        } 
        if (!Ext.isEmpty(t7)) {
            t7.setValue(0);
        }
        if (!Ext.isEmpty(t8)) {
            t8.setValue(0);
        }
        if (!Ext.isEmpty(t9)) {
            t9.setValue(0);
        }
        if (!Ext.isEmpty(t10)) {
            t10.setValue(0);
        }
    }
</script>

<ext:Window ID="winDetail" runat="server" Height="400" Width="950" Hidden="true"
    Maximizable="true" MinHeight="400" MinWidth="950" Layout="Fit">
    <Content>
        <ext:Hidden ID="hfNoExpEst" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
        <ext:Hidden ID="hfGdg" runat="server" />
        <ext:Hidden ID="hfGdgdesc" runat="server" />
    </Content>
    <Items>
        <ext:BorderLayout ID="bllayout" runat="server">
            <North MinHeight="50" MaxHeight="75" Collapsible="false">
                <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="75" Padding="10">
                    <Items>
                        <ext:ComboBox ID="cbEks" runat="server" FieldLabel="Ekspedisi" DisplayField="v_ket"
                            ValueField="c_exp" Width="250" ItemSelector="tr.search-item" MinChars="3" PageSize="10"
                            ListWidth="300" AllowBlank="false" ForceSelection="false">
                            <%--<DirectEvents>
                <Change OnEvent="cbeks_OnChange">
                  <EventMask ShowMask="true" />
                </Change>
              </DirectEvents>--%>
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
                                        <ext:Parameter Name="model" Value="2081" />
                                        <ext:Parameter Name="parameters" Value="[['@contains.v_ket.Contains(@0)', paramTextGetter(#{cbEks}), '']]"
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
                            <Template ID="Template1" runat="server">
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
                                <Change Handler="clearRelatedComboRecursive(true, #{cbCustomer});" />
                            </Listeners>
                        </ext:ComboBox>
                    </Items>
                </ext:FormPanel>
            </North>
            <Center MinHeight="150">
                <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Cabang" Height="150" Layout="Fit">
                    <TopBar>
                        <ext:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                                    LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                                    <Items>
                                        <%--<ext:ComboBox ID="cbToHdr" runat="server" FieldLabel="Tujuan" DisplayField="v_gdgdesc"
                                          ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
                                          MinChars="3" AllowBlank="false" ForceSelection="false">
                                          <CustomConfig>
                                            <ext:ConfigItem Name="allowBlank" Value="false" />
                                          </CustomConfig>
                                          <Store>
                                            <ext:Store ID="Store2" runat="server" RemotePaging="false">
                                              <Proxy>
                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                  CallbackParam="soaScmsCallback" />
                                              </Proxy>
                                              <BaseParams>
                                                <ext:Parameter Name="allQuery" Value="true" />
                                                <ext:Parameter Name="model" Value="2031" />
                                                <ext:Parameter Name="parameters" Value="[['c_gdg != @0', #{hfGudang}.getValue(), 'System.Char'],
                                                ['v_gdgdesc != @0', 'Gudang Karantin', 'System.String']]"
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
                                          <Template ID="Template2" runat="server">
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
                                            <Change Handler="clearRelatedComboRecursive(true, #{cbPrincipalHdr});" />
                                          </Listeners>
                                        </ext:ComboBox>--%>
                                        <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" ValueField="c_gdg"
                                          DisplayField="v_gdgdesc" Width="120" AllowBlank="false">
                                          <Store>
                                            <ext:Store ID="Store2" runat="server">
                                              <Proxy>
                                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                  CallbackParam="soaScmsCallback" />
                                              </Proxy>
                                              <BaseParams>
                                                <ext:Parameter Name="allQuery" Value="true" />
                                                <ext:Parameter Name="model" Value="2031" />
                                                <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
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
                                          <Triggers>
                                            <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                                          </Triggers>
                                          <Listeners>
                                            <Select Handler="this.triggers[0].show();" />
                                            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                                            <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); }" />
                                          </Listeners>
                                        </ext:ComboBox>
                                        <ext:ComboBox ID="cbCustomer" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                                            ValueField="c_cusno" Width="150" ItemSelector="tr.search-item" PageSize="10"
                                            ListWidth="300" MinChars="3" AllowBlank="false" ForceSelection="false">
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
                                                        <ext:Parameter Name="model" Value="2011-a" />
                                                        <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), ''],
                                                                    ['exp', #{cbEks}.getValue(), 'System.String']]" Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="v_cunam" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
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
                                                <Change Handler="fillDefaultEntry(#{txUdara}, #{txDarat}, #{txIcePack}, #{txCdd}, #{txFuso}, #{txTronton}, #{txMin}, #{txContainer}, #{txCde}, #{txL300});" />
                                            </Listeners>
                                        </ext:ComboBox>
                                        <%--<ext:ComboBox ID="cbTipeStn" runat="server" FieldLabel="Satuan" DisplayField="v_ket"
                                              ValueField="c_type" Width="150" AllowBlank="false" ForceSelection="false">
                                              <CustomConfig>
                                                <ext:ConfigItem Name="allowBlank" Value="false" />
                                              </CustomConfig>
                                              <Store>
                                                <ext:Store ID="Store2" runat="server" RemotePaging="false">
                                                  <Proxy>
                                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                      CallbackParam="soaScmsCallback" />
                                                  </Proxy>
                                                  <BaseParams>
                                                    <ext:Parameter Name="allQuery" Value="true" />
                                                    <ext:Parameter Name="model" Value="2001" />
                                                    <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                                                      ['c_notrans = @0', '004', '']]" Mode="Raw" />
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
                                            </ext:ComboBox>--%>
                                        <ext:ComboBox ID="cbTipeKrm" runat="server" FieldLabel="Tipe Kirim" DisplayField="v_ket"
                                            ValueField="c_type" Width="80" AllowBlank="false" ForceSelection="false">
                                            <CustomConfig>
                                                <ext:ConfigItem Name="allowBlank" Value="false" />
                                            </CustomConfig>
                                            <Store>
                                                <ext:Store ID="Store4" runat="server" RemotePaging="false">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="allQuery" Value="true" />
                                                        <ext:Parameter Name="model" Value="2001" />
                                                        <ext:Parameter Name="parameters" Value="[['c_portal = @0', '9', 'System.Char'],
                                              ['c_notrans = @0', '005', '']]" Mode="Raw" />
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
                                        </ext:ComboBox>
                                        <ext:NumberField runat="server" ID="txUdara" FieldLabel="Udara" Width="75" />
                                        <ext:NumberField runat="server" ID="txDarat" FieldLabel="Darat/ Laut" Width="75" />
                                        <ext:NumberField runat="server" ID="txIcePack" FieldLabel="Ice Pack" Width="75" />
                                        <ext:NumberField runat="server" ID="txCdd" FieldLabel="CDD" Width="75" />
                                        <ext:NumberField runat="server" ID="txFuso" FieldLabel="Fuso" Width="75" />
                                        <ext:NumberField runat="server" ID="txTronton" FieldLabel="Tronton" Width="75" />
                                        <ext:NumberField runat="server" ID="txContainer" FieldLabel="Container" Width="75" />
                                        <ext:NumberField runat="server" ID="txCde" FieldLabel="CDE" Width="75" />
                                        <ext:NumberField runat="server" ID="txL300" FieldLabel="L300" Width="75" />                                        
                                        <ext:NumberField runat="server" ID="txMin" FieldLabel="Min.Berat(KG)" Width="75" />
                                        <ext:DateField ID="txTanggal" runat="server" FieldLabel="Tanggal Effective" AllowBlank="false" Format="dd-MM-yyyy" EnableKeyEvents="true" />
                                        <ext:Button ID="btnAdd" runat="server" FieldLabel="&nbsp;" LabelSeparator=" " ToolTip="Add"
                                            Icon="Add">
                                            <Listeners>
                                                <Click Handler="storeToDetailGrid(#{frmpnlDetailEntry}, #{gridDetail}, #{cbGudang}, #{cbCustomer}, #{txUdara}, #{txDarat}, #{txIcePack} , #{cbTipeKrm}, #{txCdd}, #{txFuso}, #{txTronton}, #{txContainer}, #{txCde}, #{txL300}, #{txMin}, #{txTanggal});" />
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
                        <ext:GridPanel ID="gridDetail" runat="server">
                            <SelectionModel>
                                <ext:RowSelectionModel SingleSelect="true" />
                            </SelectionModel>
                            <Store>
                                <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false">
                                    <Proxy>
                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                            CallbackParam="soaScmsCallback" />
                                    </Proxy>
                                    <BaseParams>
                                        <ext:Parameter Name="start" Value="0" />
                                        <ext:Parameter Name="limit" Value="-1" />
                                        <ext:Parameter Name="allQuery" Value="true" />
                                        <ext:Parameter Name="model" Value="0199" />
                                        <ext:Parameter Name="sort" Value="" />
                                        <ext:Parameter Name="dir" Value="" />
                                        <ext:Parameter Name="parameters" Value="[['c_exp = @0', #{hfNoExpEst}.getValue(), 'System.String']]" Mode="Raw" />
                                    </BaseParams>
                                    <Reader>
                                        <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                            <Fields>
                                                <ext:RecordField Name="c_gdg" />
                                                <ext:RecordField Name="v_gdgdesc" />
                                                <ext:RecordField Name="c_cusno" />
                                                <ext:RecordField Name="v_cunam" />
                                                <ext:RecordField Name="c_typekrm" />
                                                <ext:RecordField Name="c_typesat" />
                                                <ext:RecordField Name="v_typekrm" />
                                                <ext:RecordField Name="v_typesat" />
                                                <ext:RecordField Name="v_cunam" />
                                                <ext:RecordField Name="n_udara" Type="Float" />
                                                <ext:RecordField Name="n_daratlaut" Type="Float" />
                                                <ext:RecordField Name="n_icepack" Type="Float" />
                                                <ext:RecordField Name="n_cdd" Type="Float" />
                                                <ext:RecordField Name="n_fuso" Type="Float" />
                                                <ext:RecordField Name="n_tronton" Type="Float" />
                                                <ext:RecordField Name="n_container" Type="Float" />
                                                <ext:RecordField Name="n_cde" Type="Float" />
                                                <ext:RecordField Name="n_l300" Type="Float" />
                                                <ext:RecordField Name="n_expmin" Type="Float" />
                                                <ext:RecordField Name="d_effective" Type="Date" DateFormat="M$" />                    
                                                <ext:RecordField Name="l_new" Type="Boolean" />
                                                <ext:RecordField Name="l_void" Type="Boolean" />
                                                <ext:RecordField Name="l_modified" Type="Boolean" />
                                                <ext:RecordField Name="v_ket" />
                                            </Fields>
                                        </ext:JsonReader>
                                    </Reader>
                                    <SortInfo Field="v_cunam" Direction="ASC" />
                                </ext:Store>
                            </Store>
                            <ColumnModel>
                                <Columns>
                                    <ext:CommandColumn Width="25">
                                        <Commands>
                                            <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                                            <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                                        </Commands>
                                        <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfNoExpEst}.getValue());" />
                                    </ext:CommandColumn>
                                    <%-- <ext:Column DataIndex="c_cusno" Header="Kode" Width="50" />--%>
                                    <ext:Column ColumnID="v_gdgdesc" DataIndex="v_gdgdesc" Header="Gudang" Width="70" />
                                    <ext:Column DataIndex="v_cunam" Header="Cabang" Width="120" />
                                    <ext:Column DataIndex="v_typekrm" Header="Tipe Kirim" Width="70" />
                                    <%--<ext:Column DataIndex="v_typesat" Header="Tipe Satuan" Width="80" />--%>
                                    <ext:NumberColumn DataIndex="n_udara" Header="Udara" Format="0.000,00/i" Width="80"
                                        Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField1" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_daratlaut" Header="Darat / Laut" Format="0.000,00/i"
                                        Width="80" Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField2" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_icepack" Header="Ice Pack" Format="0.000,00/i" Width="80"
                                        Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField3" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_cdd" Header="CDD" Format="0.000,00/i" Width="80" Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField4" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_fuso" Header="Fuso" Format="0.000,00/i" Width="80"
                                        Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField5" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_tronton" Header="Tronton" Format="0.000,00/i" Width="80"
                                        Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField6" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_container" Header="Container" Format="0.000,00/i"
                                        Width="80" Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField8" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_cde" Header="CDE" Format="0.000,00/i"
                                        Width="80" Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField7" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_l300" Header="L300" Format="0.000,00/i"
                                        Width="80" Editable="true">
                                        <Editor>
                                            <ext:NumberField ID="NumberField9" runat="server" AllowDecimals="true" AllowNegative="false"
                                                DecimalPrecision="2" />
                                        </Editor>
                                    </ext:NumberColumn>
                                    <ext:NumberColumn DataIndex="n_expmin" Header="Min.Berat(KG)" Format="0.000,00/i"
                                        Width="80" Editable="true">
                                    </ext:NumberColumn>
                                    <ext:DateColumn ColumnID="d_effective" DataIndex="d_effective" Header="Tanggal Effective" Format="dd-MM-yyyy" />                
                                    <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                                </Columns>
                            </ColumnModel>
                            <Listeners>
                                <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                                <AfterEdit Fn="afterEdit" />
                            </Listeners>
                        </ext:GridPanel>
                    </Items>
                </ext:Panel>
            </Center>
        </ext:BorderLayout>
    </Items>
    <Buttons>
        <ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Simpan">
            <DirectEvents>
                <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
                    <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
                        Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                        <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                            Mode="Raw" />
                        <ext:Parameter Name="NumberID" Value="#{hfNoExpEst}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="expId" Value="#{cbEks}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="expDesc" Value="#{cbEks}.getText()" Mode="Raw" />
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
        <ext:Button ID="Button2" runat="server" Icon="Cancel" Text="Keluar">
            <Listeners>
                <Click Handler="#{winDetail}.hide();" />
            </Listeners>
        </ext:Button>
    </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
