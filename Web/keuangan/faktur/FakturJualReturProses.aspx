<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="FakturJualReturProses.aspx.cs" Inherits="keuangan_faktur_FakturJualReturProses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var voidDataFromMainStore = function(rec) {
      if (Ext.isEmpty(rec)) {
        return;
      }

      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
        function(btn) {
          if (btn == 'yes') {
            ShowAsk('Alasan ?', 'Masukkan alasan kenapa harus dibatalkan.',
              function(btnP, txt) {
                if (btnP == 'ok') {
                  if (txt.trim().length < 1) {
                    txt = 'Kesalahan pemakai.';
                  }

                  Ext.net.DirectMethods.DeleteMethod(rec.get('c_fjno'), txt);
                }
              });
          }
        });
    }

    var reloadStore = function(store) {
      if (!Ext.isEmpty(store)) {
        store.removeAll();
        store.reload();
      }
    }

    //    var clearTreePanel = function(tp) {
    //      var root = tp.getRootNode();

    //      root.removeChildren();
    //    }

    var beforeLoadStore = function(gph) {
      showMaskLoad(Ext, "Sedang membaca data...");

      var storGrid = '';

      if (!Ext.isEmpty(gph)) {
        storGrid = gph.getStore();

        storGrid.removeAll();
        storGrid.commitChanges();
      }
    }
    
//    var beforeLoadStore = function(gph, gp) {
//      showMaskLoad(Ext, "Sedang membaca data...");

//      var storGrid = '';

//      if (!Ext.isEmpty(gph)) {
//        storGrid = gph.getStore();

//        storGrid.removeAll();
//        storGrid.commitChanges();
//      }

//      if (!Ext.isEmpty(gp)) {
//        storGrid = gp.getStore();

//        storGrid.removeAll();
//        storGrid.commitChanges();
//      }
//    }

    var afterLoadStorePopulate = function(store, recs, gph) {
      var lm = showMaskLoad(Ext, "Ekstrak data...");

      if ((!Ext.isEmpty(gph)) && (recs.length > 0)) {
        var rcNo = '',
          doNo = '';
        var idx = 0;
        var storHeader = gph.getStore();

        Ext.each(recs, function(r) {
          r.set('l_new', true);

          rcNo = r.get('c_norc');
          doNo = r.get('c_nodo');

          idx = storeFindMultiple(storHeader, ['c_rcno', 'c_dono'], [rcNo, doNo]);
          if (idx == -1) {

            storHeader.insert(0, new Ext.data.Record({
              'c_dono': r.get('c_nodo'),
              'c_rcno': r.get('c_norc'),
              'c_exno': r.get('c_exno'),
              'd_rcdate': r.get('d_rcdate'),
              'l_new': true
            }));
          }
        });

        storHeader.commitChanges();

        store.commitChanges();
      }

      lm.hide();
    }

    //    var afterLoadStorePopulate = function(store, recs, tp) {
    //      showMaskLoad(Ext, "Ekstrak data...");

    //      if ((!Ext.isEmpty(tp)) && (recs.length > 0)) {
    //        var rootNode = tp.getRootNode();
    //        var node = undefined,
    //          nodeChld = undefined;
    //        var rcNo = '',
    //          prevRcNo = '',
    //          doNo = '',
    //          prevDoNo = '';
    //        var idx = 0;

    //        Ext.each(recs, function(r) {
    //          idx++;

    //          r.set('l_new', true);

    //          rcNo = r.get('c_norc');
    //          doNo = r.get('c_nodo');

    //          if (prevRcNo == rcNo) {
    //            // Fastest Logic
    //            if (prevDoNo != doNo) {
    //              nodeChld = new Ext.tree.TreeNode({
    //                text: doNo
    //              });
    //              nodeChld.id = 'do' + idx.toString();
    //              nodeChld.iconCls = 'icon-package';

    //              node.appendChild(nodeChld);
    //            }
    //          }
    //          else {
    //            node = new Ext.tree.TreeNode({
    //              text: rcNo
    //            });
    //            node.id = 'rc' + idx.toString();
    //            node.iconCls = 'icon-briefcase';

    //            nodeChld = new Ext.tree.TreeNode({
    //              text: doNo
    //            });
    //            nodeChld.id = 'do' + idx.toString();
    //            nodeChld.iconCls = 'icon-package';

    //            node.appendChild(nodeChld);

    //            rootNode.appendChild(node);

    //            //icon-briefcase
    //            //icon-package
    //          }

    //          prevRcNo = rcNo;
    //          prevDoNo = doNo;
    //        });

    //        store.commitChanges();
    //      }

    //      afterLoadStore();
    //    }

    var afterLoadStore = function() {
      showMaskLoad(Ext, "Sedang membaca data...", true);
    }

    var onGPHeaderRowSelected = function(item, store, grid) {
      if (Ext.isEmpty(item) || Ext.isEmpty(item.getSelected()) || Ext.isEmpty(store) || Ext.isEmpty(grid) || (store.getCount() < 1)) {
        return;
      }

      var lm = showMaskLoad(grid, 'Mohon tunggu...');

      var r = item.getSelected();

      var doNo = r.get('c_dono');
      var rcNo = r.get('c_rcno');

      var recs = null;
      var storGrid = grid.getStore();

      storGrid.removeAll();

      store.filter([
        {
          property: 'c_norc',
          value: rcNo,
          exactMatch: true
        },
        {
          property: 'c_nodo',
          value: doNo,
          exactMatch: true
        }
      ]);

      recs = store.getRange();

      Ext.each(recs, function(r) {
        storGrid.insert(0, new Ext.data.Record({
          'c_rcno': r.get('c_norc'),
          'c_dono': r.get('c_nodo'),
          'c_iteno': r.get('c_iteno'),
          'v_itnam': r.get('v_itnam'),
          'c_type': r.get('c_type'),
          'n_salpri': r.get('n_salpri'),
          'n_disc': r.get('n_disc'),
          'n_qty': r.get('n_qty'),
          'l_new': r.get('l_new')
        }));
      });

      store.clearFilter(true);

      storGrid.commitChanges();

      lm.hide();
    }

    //    var nodeClickPopulate = function(store, grid, node) {
    //      if (Ext.isEmpty(store) || Ext.isEmpty(grid) || (store.getCount() < 1)) {
    //        return;
    //      }
    //      
    //      lm = showMaskLoad(grid, 'Mohon tunggu...');
    //      
    //      var nodeId = (node.id.length >= 2 ? node.id.substr(0, 2) : '');
    //      var storGrid = grid.getStore();
    //      
    //      var doNo = node.text;
    //      var rcNo = (Ext.isEmpty(node.parentNode) ? '' : node.parentNode.text);

    //      var recs = null;

    //      storGrid.removeAll();
    //      storGrid.commitChanges();
    //      
    //      if(nodeId != 'rc') {
    //        store.filter([
    //          {
    //            property: 'c_norc',
    //            value: rcNo,
    //            exactMatch: true
    //          },
    //          {
    //            property: 'c_nodo',
    //            value: doNo,
    //            exactMatch: true
    //          }
    //        ]);
    //      }
    //      else{
    //        rcNo = node.text;
    //        
    //        store.filter([
    //          {
    //            property: 'c_norc',
    //            value: rcNo,
    //            exactMatch: true
    //          }
    //        ]);
    //      }

    //      recs = store.getRange();

    //      Ext.each(recs, function(r) {
    //        storGrid.insert(0, new Ext.data.Record({
    //          'd_rcdate': r.get('d_rcdate'),
    //          'c_exno': r.get('c_exno'),
    //          'c_rcno': r.get('c_norc'),
    //          'c_dono': r.get('c_nodo'),
    //          'c_iteno': r.get('c_iteno'),
    //          'v_itnam': r.get('v_itnam'),
    //          'c_type': r.get('c_type'),
    //          'n_salpri': r.get('n_salpri'),
    //          'n_disc': r.get('n_disc'),
    //          'n_qty': r.get('n_qty'),
    //          'l_new': r.get('l_new'),
    //        }));
    //      });      

    //      store.clearFilter(true);
    //      
    //      lm.hide();
    //    }

    var onAfterEditGridHeader = function(e, storeTemp) {
      if (Ext.isEmpty(storeTemp)) {
        return;
      }
      var rcNo = e.record.get('c_rcno');
      var doNo = e.record.get('c_dono');

      var idx = 0;
      var r = undefined;

      do {
        idx = storeFindMultiple(storeTemp, ['c_norc', 'c_nodo'], [rcNo, doNo], idx);

        if (idx != -1) {
          r = storeTemp.getAt(idx);

          r.set(e.field, e.record.get(e.field));

          r.commit();

          idx++;
        }
      } while (idx != -1);

      e.record.commit();
    }

    var onBeforeEditGrid = function(e) {
      if ((!e.record.get('l_new')) && (e.field != 'l_new')) {
        e.cancel = true;
        return;
      }
    }

    var onAfterEditGrid = function(e, storeTemp) {
      if (Ext.isEmpty(storeTemp)) {
        return;
      }
      var rcNo = e.record.get('c_rcno');
      var doNo = e.record.get('c_dono');

      var idx = storeFindMultiple(storeTemp, ['c_norc', 'c_nodo'], [rcNo, doNo]);
      if (idx != -1) {
        var r = storeTemp.getAt(idx);

        r.set(e.field, e.record.get(e.field));

        r.commit();
      }

      e.record.commit();
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Store ID="storeTempFJR" runat="server">
    <Proxy>
      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
        CallbackParam="soaScmsCallback" />
    </Proxy>
    <BaseParams>
      <ext:Parameter Name="start" Value="0" />
      <ext:Parameter Name="limit" Value="-1" />
      <ext:Parameter Name="allQuery" Value="true" />
      <ext:Parameter Name="model" Value="14101" />
      <%--<ext:Parameter Name="parameters" Value="[['customerNo', paramValueGetter(#{cbCustomerFltr}), ''],
                    ['rcFrom', paramValueGetter(#{txRCFromFltr}) , ''],
                    ['rcTo', paramValueGetter(#{txRCToFltr}), '']]" Mode="Raw" />--%>
      <ext:Parameter Name="parameters" Value="[['customerNo', paramValueGetter(#{cbCustomerFltr}), ''],
                    ['bulan', paramValueGetter(#{cbBulan}) , 'System.Int32'],
                    ['tahun', paramValueGetter(#{cbTahun}), 'System.Int32']]" Mode="Raw" />
    </BaseParams>
    <Reader>
      <ext:JsonReader Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
        <Fields>
          <ext:RecordField Name="c_cusno" />
          <ext:RecordField Name="d_rcdate" Type="Date" DateFormat="M$" />
          <ext:RecordField Name="c_exno" />
          <ext:RecordField Name="c_norc" />
          <ext:RecordField Name="c_nodo" />
         <%-- <ext:RecordField Name="c_iteno" />
          <ext:RecordField Name="v_itnam" />
          <ext:RecordField Name="c_type" />
          <ext:RecordField Name="n_salpri" Type="Float" />
          <ext:RecordField Name="n_disc" Type="Float" />
          <ext:RecordField Name="n_qty" Type="Float" />--%>
          <ext:RecordField Name="l_cab" Type="Boolean" />
          <ext:RecordField Name="l_new" Type="Boolean" />
        </Fields>
      </ext:JsonReader>
    </Reader>
    <Listeners>
      <BeforeLoad Handler="beforeLoadStore(#{gridFaktur});" />
      <Load Handler="afterLoadStorePopulate(this, records, #{gridFaktur});" />
      <LoadException Handler="afterLoadStore();" />
      <Exception Handler="afterLoadStore();" />
    </Listeners>
  </ext:Store>
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit" ButtonAlign="Right" Border="false">
        <Items>
          <ext:BorderLayout ID="bllayout" runat="server">
            <North Collapsible="false" MinHeight="135" Split="false">
              <ext:FormPanel ID="frmHeaders" runat="server" Height="135" ButtonAlign="Center" Padding="5"
                Title="Kriteria">
                <Items>
                  <ext:ComboBox ID="cbCustomerFltr" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
                    Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
                    FieldLabel="Cabang" AllowBlank="true" ForceSelection="true">
                    <CustomConfig>
                      <ext:ConfigItem Name="allowBlank" Value="true" />
                    </CustomConfig>
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
                                    ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0)', paramTextGetter(#{cbCustomerFltr}), '']]"
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
                  </ext:ComboBox>
                  <ext:CompositeField runat="server" FieldLabel="Periode">
                    <Items>
                      <%--<ext:TextField ID="txRCFromFltr" runat="server" MaxLength="11" AllowBlank="false" />
                      <ext:Label runat="server" Text="&nbsp;" />
                      <ext:TextField ID="txRCToFltr" runat="server" MaxLength="11" AllowBlank="true" />--%>
                      <ext:SelectBox ID="cbBulan" runat="server" FieldLabel="Bulan" Width="100" AllowBlank="false" />
                      <ext:Label ID="Label1" runat="server" Text="&nbsp;" />
                      <ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
                    </Items>
                  </ext:CompositeField>
                </Items>
                <Buttons>
                  <ext:Button ID="btnProses" runat="server" Text="Proses" Icon="CogStart">
                    <Listeners>
                      <Click Handler="if(validasiProses(#{frmHeaders})) { reloadStore(#{storeTempFJR}); }" />
                    </Listeners>
                    <%--<DirectEvents>
                      <Click Before="return validasiProses(#{frmHeaders});" OnEvent="ProcessFJR_Click">
                        <Confirmation BeforeConfirm="return validasiProses(#{frmHeaders});"
                              ConfirmRequest="true" Title="Proses ?" Message="Anda yakin ingin memproses kriteria ini." />
                        <EventMask ShowMask="true" />
                        <ExtraParams>
                          <ext:Parameter Name="customerNo" Value="#{cbCustomerFltr}.getValue()" Mode="Raw" />
                          <ext:Parameter Name="bulan" Value="#{cbTahun}.getValue()" Mode="Raw" />
                          <ext:Parameter Name="tahun" Value="#{cbBulan}.getValue()" Mode="Raw" />
                        </ExtraParams>
                      </Click>
                    </DirectEvents>--%>
                  </ext:Button>
                </Buttons>
              </ext:FormPanel>
            </North>
            <%-- <West MinWidth="175" Split="true">
              <ext:Panel runat="server" Width="175" Title="Nomor Retur" Layout="Fit">
                <Items>
                  <ext:TreePanel ID="tpRetur" runat="server" AutoScroll="true" MultiSelect="false"
                    RootVisible="false" EnableDD="true" Animate="true" UseArrows="true" ContainerScroll="false">
                    <Root>
                      <ext:TreeNode Text="Root" />
                    </Root>
                    <Listeners>
                      <Click Handler="nodeClickPopulate(#{storeTempFJR}, #{gridDetail}, node);" />
                    </Listeners>
                  </ext:TreePanel>
                </Items>
              </ext:Panel>
            </West>--%>
            <Center MinHeight="125">
              <ext:Panel runat="server" Title="Faktur Header" Layout="Fit">
                <Items>
                  <ext:GridPanel ID="gridFaktur" runat="server">
                    <Listeners>
                      <AfterEdit Handler="onAfterEditGridHeader(e, #{storeTempFJR});" />
                      <BeforeEdit Handler="onBeforeEditGrid(e);" />
                    </Listeners>
                    <SelectionModel>
                      <ext:RowSelectionModel SingleSelect="true">
                        <%--<Listeners>
                          <SelectionChange Handler="onGPHeaderRowSelected(item, #{storeTempFJR}, #{gridDetail});" />
                        </Listeners>--%>
                      </ext:RowSelectionModel>
                    </SelectionModel>
                    <Store>
                      <ext:Store runat="server" RemoteSort="true">
                        <Reader>
                          <ext:JsonReader IDProperty="c_iteno" TotalProperty="d.totalRows" Root="d.records"
                            SuccessProperty="d.success">
                            <Fields>
                              <ext:RecordField Name="c_rcno" />
                              <ext:RecordField Name="c_dono" />
                              <ext:RecordField Name="d_rcdate" Type="Date" DateFormat="M$" />
                              <ext:RecordField Name="c_exno" />
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="v_itnam" Direction="ASC" />
                      </ext:Store>
                    </Store>
                    <ColumnModel>
                      <Columns>
                        <ext:Column DataIndex="c_rcno" Header="No. Retur" />
                        <ext:DateColumn DataIndex="d_rcdate" Header="Tanggal" Format="dd-MM-yyyy" />
                        <ext:Column DataIndex="c_dono" Header="No. Delivery" />
                        <ext:Column DataIndex="c_exno" Header="Ex. Faktur" Editable="true">
                          <Editor>
                            <ext:TextField runat="server" MaxLength="10" />
                          </Editor>
                        </ext:Column>
                        <ext:CheckColumn DataIndex="l_cab" Header="Cabang" Width="50" />
                        <ext:CheckColumn DataIndex="l_new" Header="Aktif" Width="50" Editable="true" />
                      </Columns>
                    </ColumnModel>
                  </ext:GridPanel>
                </Items>
              </ext:Panel>
            </Center>
            <%--<South MinHeight="150" Split="true">
              <ext:Panel runat="server" Title="Details Item" Layout="Fit" Height="200">
                <Items>
                  <ext:GridPanel ID="gridDetail" runat="server">
                    <LoadMask ShowMask="true" Msg="Mohon tunggu..." />
                    <Listeners>
                      <AfterEdit Handler="onAfterEditGrid(e, #{storeTempFJR});" />
                      <BeforeEdit Handler="onBeforeEditGrid(e);" />
                    </Listeners>
                    <SelectionModel>
                      <ext:RowSelectionModel SingleSelect="true" />
                    </SelectionModel>
                    <Store>
                      <ext:Store runat="server" RemoteSort="true">
                        <Reader>
                          <ext:JsonReader IDProperty="c_iteno" TotalProperty="d.totalRows" Root="d.records"
                            SuccessProperty="d.success">
                            <Fields>
                              <ext:RecordField Name="c_rcno" />
                              <ext:RecordField Name="c_dono" />
                              <ext:RecordField Name="c_iteno" />
                              <ext:RecordField Name="v_itnam" />
                              <ext:RecordField Name="c_type" />
                              <ext:RecordField Name="n_salpri" Type="Float" />
                              <ext:RecordField Name="n_disc" Type="Float" />
                              <ext:RecordField Name="n_qty" Type="Float" />
                              <ext:RecordField Name="l_new" Type="Boolean" />
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                        <SortInfo Field="v_itnam" Direction="ASC" />
                      </ext:Store>
                    </Store>
                    <ColumnModel>
                      <Columns>
                        <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                        <ext:Column DataIndex="v_itnam" Header="Nama" Width="250" />
                        <ext:NumberColumn DataIndex="n_qty" Header="Jumlah" Format="0.000,00/i" Editable="true">
                          <Editor>
                            <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false"
                              DecimalPrecision="2" MinValue="0">
                              <Listeners>
                                <Focus Handler="this.selectText();" />
                              </Listeners>
                            </ext:NumberField>
                          </Editor>
                        </ext:NumberColumn>
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
                        <ext:CheckColumn DataIndex="l_new" Header="Aktif" Width="50" Editable="true" />
                      </Columns>
                    </ColumnModel>
                  </ext:GridPanel>
                </Items>
              </ext:Panel>
            </South>--%>
          </ext:BorderLayout>
        </Items>
        <Buttons>
          <ext:Button ID="btnSimpan" runat="server" Icon="Disk" Text="Simpan">
            <DirectEvents>
              <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndCustomStore('', #{storeTempFJR});">
                <Confirmation BeforeConfirm="return verifyHeaderAndCustomStore('', #{storeTempFJR});"
                  ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="storeValues" Value="saveStoreToServer(#{storeTempFJR})" Mode="Raw" />
                  <ext:Parameter Name="customerId" Value="#{cbCustomerFltr}.getValue()" Mode="Raw" />
                </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>
        </Buttons>
      </ext:Panel>
    </Items>
  </ext:Viewport>
</asp:Content>
