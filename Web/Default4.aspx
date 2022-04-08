<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default4.aspx.cs" Inherits="Default4" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
  <style type="text/css" media="screen"></style>
  <script type="text/javascript">
    var nodeClick = function(node, grid) {
      if (Ext.isEmpty(node)) {
        return;
      }
      
      if (!node.isLeaf()) {
        if (!Ext.isEmpty(grid)) {
          var store = grid.getStore();
          if (!Ext.isEmpty(store)) {
            store.removeAll();
            store.commitChanges();
          }
        }        
        return;
      }

      var nodeId = node.attributes['NodeID'];
      var nodeX = node.attributes['nodeX'];

      Ext.net.DirectMethods.NodeClick(
        (Ext.isEmpty(node.id) ? '' : node.id),
        (Ext.isEmpty(nodeId) ? '' : nodeId),
        (Ext.isEmpty(nodeX) ? '' : nodeX),
        {
          success: function(result) {
            //ShowInformasi('Pemanggilan sukses.');
            ;
          },
          failure: function(result) {
            ShowError('Terdapat kesalahan dalam parsing javascript.');
          },
          eventMask: {
            showMask: true,
            minDelay: 500
          }
        });
    }

    var verifyBeforeApply = function(tv, grid) {
      if (Ext.isEmpty(tv) || Ext.isEmpty(grid)) {
        return false;
      }

      var selNode = tv.getSelectionModel().getSelectedNode();

      if (Ext.isEmpty(selNode)) {
        return false;
      }

      if (!selNode.isLeaf) {
        return false;
      }

      var store = grid.getStore();

      if (Ext.isEmpty(store)) {
        return false;
      }

      if (store.getCount() < 1) {
        return false;
      }

      var nodeId = selNode.attributes['NodeID'];
      var nodeX = selNode.attributes['nodeX'];
    }

    var beforeEditVerify = function(e) {
      var isOk = false;

      //e.record.get(e.field)

      switch (e.field.trim()) {
        case 'isView':
          isOk = e.record.get('isOkView');
          break;
        case 'isPrint':
          isOk = e.record.get('isOkPrint');
          break;
        case 'isAdd':
          isOk = e.record.get('isOkAdd');
          break;
        case 'isEdit':
          isOk = e.record.get('isOkEdit');
          break;
        case 'isDelete':
          isOk = e.record.get('isOkDelete');
          break;
        default:
          isOk = false;
          break;
      }

      e.cancel = (!isOk);
    }

    var onPrepareRowGrid = function(grid, rowIdx, rec) {
      var col = grid.getColumnModel();

      var colView = col.getColumnById('IsView');
      var colPrint = col.getColumnById('IsPrint');
      var colAdd = col.getColumnById('IsAdd');
      var colEdit = col.getColumnById('IsEdit');
      var colDelete = col.getColumnById('IsDelete');

      colView.editable = rec.get('isOkView');
      colPrint.editable = rec.get('isOkPrint');
      colAdd.editable = rec.get('isOkAdd');
      colEdit.editable = rec.get('isOkEdit');
      colDelete.editable = rec.get('isOkDelete');
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Button runat="server" Text="Populate Menu">
    <DirectEvents>
      <Click OnEvent="Button_Click" />
    </DirectEvents>
  </ext:Button>
  <asp:Button ID="btnPostBack" runat="server" Text="Post Back" 
    onclick="btnPostBack_Click" />
  <br />
  <ext:Panel runat="server" Width="700" Height="400">
    <Items>
      <ext:BorderLayout runat="server">
        <West Split="true" MarginsSummary="5 0 5 5" MinWidth="150">
          <ext:TreePanel ID="treeApp" runat="server" Width="250" Title="Akses Modul" AutoScroll="true">
            <Root>
              <ext:AsyncTreeNode NodeID="0" Text="Root" />
            </Root>
            <Listeners>
              <Click Handler="nodeClick(node, #{gridListAccess})" />
            </Listeners>
          </ext:TreePanel>
        </West>
        <Center MarginsSummary="5 5 5 0" MinWidth="250">
          <ext:GridPanel ID="gridListAccess" runat="server" StripeRows="true" Title="Hak Akses">
            <Store>
              <ext:Store runat="server">
                <Reader>
                  <ext:ArrayReader>
                    <Fields>
                      <ext:RecordField Name="Nama" />
                      <ext:RecordField Name="isOkView" Type="Boolean" />
                      <ext:RecordField Name="isOkPrint" Type="Boolean" />
                      <ext:RecordField Name="isOkAdd" Type="Boolean" />
                      <ext:RecordField Name="isOkEdit" Type="Boolean" />
                      <ext:RecordField Name="isOkDelete" Type="Boolean" />
                      <ext:RecordField Name="isView" Type="Boolean" />
                      <ext:RecordField Name="isPrint" Type="Boolean" />
                      <ext:RecordField Name="isAdd" Type="Boolean" />
                      <ext:RecordField Name="isEdit" Type="Boolean" />
                      <ext:RecordField Name="isDelete" Type="Boolean" />
                    </Fields>
                  </ext:ArrayReader>
                </Reader>
              </ext:Store>
            </Store>
            <ColumnModel runat="server">
              <Columns>
                <ext:CommandColumn Hidden="true" Hideable="true" Width="0" Locked="true">
                  <PrepareToolbar Handler="onPrepareRowGrid(grid, rowIndex, record);" />
                </ext:CommandColumn>
                <ext:Column DataIndex="Nama" ColumnID="Nama" Header="Page" Width="160" Sortable="true"
                  Editable="false" />
                <ext:CheckColumn DataIndex="isView" ColumnID="IsView" Header="Lihat" Width="50" Editable="true" />
                <ext:CheckColumn DataIndex="isPrint" ColumnID="IsPrint" Header="Cetak" Width="50"
                  Editable="true" />
                <ext:CheckColumn DataIndex="isAdd" ColumnID="IsAdd" Header="Tambah" Width="50" Editable="true" />
                <ext:CheckColumn DataIndex="isEdit" ColumnID="IsEdit" Header="Ubah" Width="50" Editable="true" />
                <ext:CheckColumn DataIndex="isDelete" ColumnID="IsDelete" Header="Hapus" Width="50"
                  Editable="true" />
                <%--<ext:Column DataIndex="isOkView" Hidden="true" Hideable="false" ColumnID="IsOkView" />
                <ext:Column DataIndex="isOkPrint" Hidden="true" Hideable="false" ColumnID="IsOkPrint" />
                <ext:Column DataIndex="isOkAdd" Hidden="true" Hideable="false" ColumnID="IsOkAdd" />
                <ext:Column DataIndex="isOkEdit" Hidden="true" Hideable="false" ColumnID="IsOkEdit" />
                <ext:Column DataIndex="isOkDelete" Hidden="true" Hideable="false" ColumnID="IsOkDelete" />--%>
              </Columns>
            </ColumnModel>
            <SelectionModel>
              <ext:RowSelectionModel runat="server" />
            </SelectionModel>
            <Listeners>
              <BeforeEdit Handler="beforeEditVerify(e);" />
            </Listeners>
            <LoadMask ShowMask="true" />
          </ext:GridPanel>
        </Center>
      </ext:BorderLayout>
    </Items>
    <BottomBar>
      <ext:Toolbar runat="server">
        <CustomConfig>
          <ext:ConfigItem Name="buttonAlign" Value="right" Mode="Value" />
        </CustomConfig>
        <Items>
          <ext:Button ID="btnApply" runat="server" Text="Apply" Icon="Accept">
            <DirectEvents>
              <Click OnEvent="btnApply_Click" Before="return verifyBeforeApply(#{treeApp}, #{gridListAccess});" >
                <EventMask ShowMask="true" />
                <ExtraParams>
                  <ext:Parameter Name="DataAccess" Value="saveStoreToServer(#{gridListAccess}.getStore())" Mode="Raw" />
                  <ext:Parameter Name="DataNodeID" Value="#{treeApp}.getSelectionModel().getSelectedNode().id" Mode="Raw" />
                  <ext:Parameter Name="DataNode" Value="#{treeApp}.getSelectionModel().getSelectedNode().attributes['nodeX']" Mode="Raw" />
                </ExtraParams>
              </Click>
            </DirectEvents>
          </ext:Button>
          <ext:Button ID="btnReset" runat="server" Text="Reset" Icon="Cross">
            <Listeners>
              <Click Handler="#{gridListAccess}.getStore().rejectChanges();" />
            </Listeners>
          </ext:Button>
        </Items>
      </ext:Toolbar>
    </BottomBar>
  </ext:Panel>
</asp:Content>
