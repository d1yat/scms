<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="bdprdp.aspx.cs" Inherits="proses_snapshot_bdprdp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript" language="javascript">
    var isWaitStore = false;
    var storeSet = '';
    var arrLoadingStore = '';
    var idxTotal = 0;
    
    var checkChangeTree = function(n, checked) {
      n.expand();
      //n.expandChildNodes(true);
      n.eachChild(function(child) {
        child.ui.toggleCheck(checked);
        //child.fireEvent('checkchange', child, checked);
      })
    }
    var onProsesClick = function(btn, cbTahun, cbBulan, tv, stor) {
      if (Ext.isEmpty(tv)) {
        ShowError('Gagal membaca object TreePanel.');
        return;
      }
      else if (Ext.isEmpty(stor)) {
        ShowError('Gagal membaca object TreePanel.');
        return;
      }
      var nods = tv.getChecked();
      if (Ext.isEmpty(nods) || (nods.length < 1)) {
        ShowWarning('Tidak ada node yang di centang.');
        return;
      }

      showMaskLoad(Ext, 'Mohon tunggu..', false);

      var pNode = '',
        isBdp = false;
      //      var lastOpt = stor.lastOptions
      var pars = [];
      idxTotal = 0;

      stor.setBaseParam('start', 0);
      stor.setBaseParam('limit', -1);
      stor.setBaseParam('allQuery', true);
      stor.setBaseParam('sort', '');
      stor.setBaseParam('dir', 'ASC');
      stor.setBaseParam('model', '30001');

      arrLoadingStore = new Array();
      var nodData = '';
      var tmp = '';

      Ext.each(nods, function(nod) {
        pNode = nod.parentNode;
        if ((!Ext.isEmpty(pNode)) && (!pNode.isRoot)) {
          if (pNode.id == 'BDP') {
            isBdp = true;
          }
          else {
            isBdp = false;
          }

          tmp = nod.id.substring(4);

          pars = [['Tahun', paramValueGetter(cbTahun), 'System.Int32'],
            ['Bulan', paramValueGetter(cbBulan), 'System.Int32'],
            ['@in.tipe', String.format('[\'{0}\']', tmp), 'System.String[]'],
            ['bdp', isBdp, 'System.Boolean']];

          nodData = { 'node': nod, 'param': pars };

          arrLoadingStore.push(nodData);
          idxTotal++;

          //stor.setBaseParam('parameters', pars);

          //stor.load();
        }
      });

      storeSet = stor;

      var task = new Ext.util.DelayedTask(function() {
        if (isWaitStore) {
          task.delay(500);
          return;
        }
        else if (Ext.isEmpty(storeSet) || (arrLoadingStore.length < 1)) {
          showMaskLoad(Ext, '', true);
          delete arrLoadingStore;
          delete storeSet;
          idxTotal = 0;
          return;
        }

        idxTotal--;

        isWaitStore = true;

        var parx = arrLoadingStore.pop();

        if (!Ext.isEmpty(parx)) {
          showMaskLoad(Ext, String.format('Menjalankan \'{0}\', mohon tunggu...', parx.node.text), false);

          storeSet.setBaseParam('parameters', parx.param);

          storeSet.load();

          parx.node.ui.toggleCheck(false);
        }

        if (idxTotal >= 0) {
          task.delay(500);
        }
        else {
          showMaskLoad(Ext, '', true);

          delete arrLoadingStore;
          delete storeSet;
          idxTotal = 0;
        }
      });

      task.delay(500);
    }
    
    var onLoadAndException = function() {
      isWaitStore = false;
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Store ID="storCallSP" runat="server">
    <Proxy>
      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
        CallbackParam="soaScmsCallback" />
    </Proxy>
    <Reader>
      <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
        IDProperty="ID">
        <Fields>
          <ext:RecordField Name="ID" />
          <ext:RecordField Name="Result" Type="Boolean" />
        </Fields>
      </ext:JsonReader>
    </Reader>
    <Listeners>
      <Load Fn="onLoadAndException" />
      <Exception Fn="onLoadAndException" />
    </Listeners>
  </ext:Store>
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" AutoScroll="true" Layout="FormLayout"
        Padding="5" ButtonAlign="Center">
        <Items>
          <ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
          <ext:SelectBox ID="cbBulan" runat="server" FieldLabel="Bulan" Width="100" AllowBlank="false" />
          <ext:TreePanel ID="tpBDPRDP" runat="server" UseArrows="true"
            AutoScroll="true" Animate="true" ContainerScroll="true" RootVisible="false" Height="300"
            Width="250" FieldLabel="Tipe Snapshot">
            <Root>
              <ext:TreeNode>
                <Nodes>
                  <ext:TreeNode Text="Barang Dalam Perjalanan (BDP)" Checked="False" NodeID="BDP">
                    <Nodes>
                      <ext:TreeNode Text="BDP" Checked="False" NodeID="BDP_01" />
                      <ext:TreeNode Text="Rill BDP" Checked="False" NodeID="BDP_02" />
                      <ext:TreeNode Text="Penerimaan Lain-lain (BDP)" Checked="False" NodeID="BDP_03" />
                      <ext:TreeNode Text="Selisih (BDP)" Checked="False" NodeID="BDP_04" />
                      <ext:TreeNode Text="RN Cabang" Checked="False" NodeID="BDP_05" />
                      <ext:TreeNode Text="RN Cabang Selisih" Checked="False" NodeID="BDP_06" />
                      <ext:TreeNode Text="Pending (BDP)" Checked="False" NodeID="BDP_07" />
                    </Nodes>
                  </ext:TreeNode>
                  <ext:TreeNode Text="Retur Dalam Perjalanan (RDP)" Checked="False" NodeID="RDP">
                    <Nodes>
                      <ext:TreeNode Text="RDP" Checked="False" NodeID="RDP_01" />
                      <ext:TreeNode Text="Rill RDP" Checked="False" NodeID="RDP_02" />
                      <ext:TreeNode Text="Penerimaan Lain-lain (RDP)" Checked="False" NodeID="RDP_03" />
                      <ext:TreeNode Text="Selisih (RDP)" Checked="False" NodeID="RDP_04" />
                      <ext:TreeNode Text="Retur Cabang" Checked="False" NodeID="RDP_05" />
                      <ext:TreeNode Text="Retur Cabang Selisih" Checked="False" NodeID="RDP_06" />
                      <ext:TreeNode Text="Pending (RDP)" Checked="False" NodeID="RDP_07" />
                    </Nodes>
                  </ext:TreeNode>
                </Nodes>
              </ext:TreeNode>
            </Root>
            <Listeners>
              <CheckChange Fn="checkChangeTree" />
            </Listeners>
          </ext:TreePanel>
        </Items>
        <Buttons>
          <ext:Button ID="btnProses" runat="server" Text="Proses" Icon="CogStart">
            <Listeners>
              <Click Handler="onProsesClick(this, #{cbTahun}, #{cbBulan}, #{tpBDPRDP}, #{storCallSP});" />
            </Listeners>
          </ext:Button>
        </Buttons>
      </ext:Panel>
    </Items>
  </ext:Viewport>
</asp:Content>
