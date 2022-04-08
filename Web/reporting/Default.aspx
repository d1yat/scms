<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default.aspx.cs" Inherits="reporting_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript" language="javascript">
    var execDeleteCommand = function(grid, idx, rpt, rptName, user) {
      var gridStore = grid.getStore();

      Ext.net.DirectMethods.DeleteReportMethod(idx, rpt, user, {
        success: function(result) {
          //var ss = '';
          var idx = 0;
          var isSuccess = result.IsSuccess;

          idx = storeSimple.findExact('idx', result.Idx);
          if (idx != -1) {
            storeSimple.removeAt(idx);
          }
          else {
            storeSimple.removeAll();
          }

          if (isSuccess) {
            if (!Ext.isEmpty(gridStore)) {
              idx = gridStore.findExact('Idx', result.Idx);
              if (idx != -1) {
                gridStore.removeAt(idx);
                gridStore.commitChanges();
              }
            }
          }

          callExecDeleteRunner(grid);
        },
        failure: function(result) {
          callExecDeleteRunner(grid);
        },
        eventMask: {
          showMask: true,
          msg: String.format("Menghapus report '{0}', mohon tunggu...", rptName)
        }
      });
    }

    var callExecDeleteRunner = function(grid) {
      if (storeSimple.getCount() > 0) {
        storeSimple.commitChanges();

        var r = storeSimple.getAt(0);
        if (!Ext.isEmpty(r)) {
          execDeleteCommand(grid, r.get('idx'), r.get('report'), r.get('rptName'), r.get('userName'));
        }
      }      
    }
    
    var prepareCommands = function(toolbar, rec) {
      var del = toolbar.items.get(1); // delete button
      var userName = ('<%= this.Nip %>').trim().toLowerCase();

      var isShare = rec.get('l_share');
      var entrUser = rec.get('c_entry');

      if (isShare && (userName != entrUser.trim().toLowerCase())) {
        del.setVisible(false);
      }
      else {
        del.setVisible(true);
      }
    }
    var deleteAllSelectedRows = function(g) {
      var sm = g.getSelectionModel();
      var userName = ('<%= this.Nip %>').trim().toLowerCase();
      if (sm.getCount() < 1) {
        return;
      }

      var isShare = false;
      var entrUser = '';

      ShowConfirm('Hapus ?', 'Apakah anda yakin ingin menghapus data ini ?',
        function(btn) {
          if (btn == 'yes') {
            sm.each(function(rec) {
              isShare = rec.get('l_share');
              entrUser = rec.get('c_entry');

              if (isShare && (userName != entrUser.trim().toLowerCase())) {
                return;
              }

              storeSimple.insert(0, new Ext.data.Record({
                'idx': rec.get('Idx'),
                'report': rec.get('v_report'),
                'rptName': rec.get('v_reportname'),
                'userName': userName
              }));
            });

            callExecDeleteRunner(g);
          }
        });
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfActiveUser" runat="server" />
  <ext:Window ID="wndDown" runat="server" Hidden="true" />
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit">
        <Content>
          <ext:Menu ID="mnuPopup" runat="server">
            <Items>
              <ext:MenuItem ID="MenuItem1" runat="server" Text="Delete Selected" Icon="Delete">
                <Listeners>
                  <Click Handler="deleteAllSelectedRows(#{gridMain}, #{hfActiveUser});" />
                </Listeners>
              </ext:MenuItem>
            </Items>
          </ext:Menu>
        </Content>
        <TopBar>
          <ext:Toolbar runat="server">
            <Items>
              <ext:Button runat="server" Text="Segarkan" Icon="ArrowRefresh">
                <Listeners>
                  <Click Handler="refreshGrid(#{gridMain});" />
                </Listeners>
              </ext:Button>
            </Items>
          </ext:Toolbar>
        </TopBar>
        <Items>
          <ext:GridPanel ID="gridMain" runat="server" ContextMenuID="mnuPopup">
            <LoadMask ShowMask="true" />
            <Store>
              <ext:Store runat="server" SkinID="OriginalExtStore" RemoteSort="true">
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
                  <ext:Parameter Name="model" Value="1000001" />
                  <ext:Parameter Name="parameters" Value="[['nipUser', paramValueGetter(#{hfActiveUser}), ''],
                    ['@contains.v_reportname.Contains(@0)', paramValueGetter(#{txNamaRptFltr}) , 'System.String'],
                    ['@contains.v_reportusername.Contains(@0)', paramValueGetter(#{txNameSignFltr}) , 'System.String'],
                    ['v_filetype = @0', paramValueGetter(#{cbTipeRptFltr}) , 'System.String'],
                    ['d_entry_date = @0', paramRawValueGetter(#{txDateFltr}) , 'System.DateTime']]"
                    Mode="Raw" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
                    IDProperty="Idx">
                    <Fields>
                      <ext:RecordField Name="Idx" />
                      <ext:RecordField Name="c_entry" />
                      <ext:RecordField Name="c_type" />
                      <ext:RecordField Name="v_report" />
                      <ext:RecordField Name="d_entry" Type="Date" DateFormat="M$" />
                      <ext:RecordField Name="v_filetype" />
                      <ext:RecordField Name="v_size" />
                      <ext:RecordField Name="l_share" Type="Boolean" />
                      <ext:RecordField Name="v_reportname" />
                      <ext:RecordField Name="l_download" Type="Int" />
                      <ext:RecordField Name="v_reportusername" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
                <SortInfo Field="d_entry" Direction="DESC" />
              </ext:Store>
            </Store>
            <SelectionModel>
              <ext:RowSelectionModel SingleSelect="false" />
            </SelectionModel>
            <DirectEvents>
              <Command OnEvent="gridMain_Command">
                <EventMask ShowMask="true" />
                <Confirmation BeforeConfirm="return ((command != 'Delete') ? false : true);" ConfirmRequest="true"
                  Message="Anda yakin ingin menghapus data ini ?" Title="Hapus ?" />
                <ExtraParams>
                  <ext:Parameter Name="Command" Value="command" Mode="Raw" />
                  <ext:Parameter Name="Index" Value="record.get('Idx')" Mode="Raw" />
                  <ext:Parameter Name="Report" Value="record.get('v_report')" Mode="Raw" />
                  <ext:Parameter Name="Type" Value="record.get('v_filetype')" Mode="Raw" />
                  <ext:Parameter Name="Name" Value="record.get('v_reportname')" Mode="Raw" />
                  <ext:Parameter Name="Entry" Value="record.get('c_entry')" Mode="Raw" />
                  <ext:Parameter Name="RowIndex" Value="rowIndex" Mode="Raw" />
                  <ext:Parameter Name="JmlDownload" Value="record.get('l_download')" Mode="Raw" />
                </ExtraParams>
              </Command>
            </DirectEvents>
            <ColumnModel>
              <Columns>
                <ext:CommandColumn Width="50">
                  <Commands>
                    <ext:GridCommand CommandName="Download" Icon="DiskDownload" ToolTip-Title="Command"
                      ToolTip-Text="Unduh report" />
                    <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus report" />
                  </Commands>
                  <PrepareToolbar Handler="prepareCommands(toolbar, record);" />
                </ext:CommandColumn>
                <ext:Column DataIndex="v_reportname" Header="Nama report" Width="200" />
                <ext:Column DataIndex="c_type" Header="Tipe report" Align="Right" />
                <ext:Column DataIndex="v_filetype" Header="Tipe file" Width="50" Align="Center" />
                <ext:Column DataIndex="v_size" Header="Ukuran" Align="Right" />
                <ext:DateColumn DataIndex="d_entry" Header="Dibuat" Format="dd-MM-yyyy HH:mm:ss"
                  Width="150" Align="Right" />
                <ext:Column DataIndex="v_reportusername" Header="Nama pengenal" Width="200" />
                <ext:Column DataIndex="l_download" Header="Jumlah unduh" />
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
                              <Click Handler="clearFilterGridHeader(#{gridMain}, #{txNamaRptFltr}, #{txDateFltr}, #{cbTipeRptFltr}, #{txNameSignFltr});reloadFilterGrid(#{gridMain});"
                                Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:Button>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNamaRptFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                            <Listeners>
                              <KeyUp Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />                    
                      <ext:HeaderColumn>
                        <Component>
                          <ext:SelectBox ID="cbTipeRptFltr" runat="server" AllowBlank="true" ForceSelection="true" MinChars="3">
                            <Items>
                              <ext:ListItem Text="-" Value="" />
                              <ext:ListItem Text="Xls" Value="xls" />
                              <ext:ListItem Text="PDF" Value="pdf" />
                            </Items>
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="700" Delay="700" />
                            </Listeners>
                          </ext:SelectBox>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                      <ext:HeaderColumn>
                        <Component>
                          <ext:DateField ID="txDateFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:DateField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn>
                        <Component>
                          <ext:TextField ID="txNameSignFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true"
                            AllowBlank="true">
                            <Listeners>
                              <Change Handler="reloadFilterGrid(#{gridMain})" Buffer="300" Delay="300" />
                            </Listeners>
                          </ext:TextField>
                        </Component>
                      </ext:HeaderColumn>
                      <ext:HeaderColumn />
                    </Columns>
                  </ext:HeaderRow>
                </HeaderRows>
              </ext:GridView>
            </View>
            <BottomBar>
              <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
                <Items>
                  <ext:Label runat="server" Text="Page size:" />
                  <ext:ToolbarSpacer runat="server" Width="10" />
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

  <script type="text/javascript" language="javascript">
    var storeSimple = new Ext.data.ArrayStore({
      // store configs
      autoDestroy: true,
      storeId: 'myStore',
      // reader configs
      idIndex: 0,
      idProperty: 'idx',
      fields: [
         { name: 'idx', type: 'int' },
         'report',
         'rptName',
         'userName'
      ]
    });
  </script>

</asp:Content>
