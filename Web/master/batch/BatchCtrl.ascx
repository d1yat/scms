<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BatchCtrl.ascx.cs" Inherits="master_batch_BatchCtrl" %>

<script type="text/javascript" language="javascript">
  var onSelectGridChanged = function(i, tx, dt) {
    var r = i.getSelected();

    if (!Ext.isEmpty(r)) {
      var bat = (r.get('c_batch') || '').trim();
      var dat = (r.get('d_expired') || new Date());

      tx.setValue(bat);
      dt.setValue(dat);
    }
  }
</script>

<ext:Window ID="winDetail" runat="server" Height="400" Width="600" Hidden="true"
  Maximizable="true" MinHeight="400" MinWidth="600" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfItemNo" runat="server" />
  </Content>
  <Items>
    <ext:Panel ID="pnlGridDetail" runat="server" Layout="Fit">
      <TopBar>
        <ext:Toolbar ID="Toolbar1" runat="server">
          <Items>
            <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
              LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
              <Items>
                <ext:TextField ID="txBatchID" runat="server" FieldLabel="Batch" AllowBlank="false"
                  MaxLength="15" />
                <ext:DateField ID="txDateExpired" runat="server" FieldLabel="Berlaku" AllowBlank="false"
                  Format="dd-MM-yyyy" MinDate="1900-01-01" MaxDate="2099-12-31" />
                <ext:Button ID="btnSimpan" runat="server" FieldLabel="&nbsp;" LabelSeparator=" "
                  ToolTip="Simpan" Icon="Disk">
                  <DirectEvents>
                    <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmpnlDetailEntry});">
                      <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmpnlDetailEntry});"
                        ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                      <EventMask ShowMask="true" />
                      <ExtraParams>
                        <ext:Parameter Name="ItemID" Value="#{hfItemNo}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="NumberID" Value="#{txBatchID}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="ExpiredDate" Value="#{txDateExpired}.getValue()" Mode="Raw" />
                      </ExtraParams>
                    </Click>
                  </DirectEvents>
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
          <LoadMask ShowMask="true" />
          <SaveMask ShowMask="true" />
          <SelectionModel>
            <ext:RowSelectionModel SingleSelect="true">
              <Listeners>
                <SelectionChange Handler="onSelectGridChanged(item, #{txBatchID}, #{txDateExpired});" />
              </Listeners>
            </ext:RowSelectionModel>
          </SelectionModel>
          <Store>
            <ext:Store ID="Store2" runat="server" RemoteSort="true" AutoLoad="false" SkinID="OriginalExtStore">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="0" />
                <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                <ext:Parameter Name="model" Value="0140" />
                <ext:Parameter Name="parameters" Value="[['noItem', #{hfItemNo}.getValue(), 'System.String'],
                  ['@contains.c_batch.Contains(@0)', paramValueGetter(#{txBatchFltr}), 'System.String'],
                  ['d_expired = @0', paramRawValueGetter(#{txBatchEDFltr}), 'System.DateTime']]"
                  Mode="Raw" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_batch" TotalProperty="d.totalRows" Root="d.records"
                  SuccessProperty="d.success">
                  <Fields>
                    <ext:RecordField Name="c_batch" />
                    <ext:RecordField Name="d_expired" Type="Date" DateFormat="M$" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
              <SortInfo Field="d_expired" Direction="DESC" />
            </ext:Store>
          </Store>
          <ColumnModel>
            <Columns>
              <%--<ext:CommandColumn Width="25">
                <Commands>
                  <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                </Commands>
              </ext:CommandColumn>--%>
              <ext:Column DataIndex="c_batch" Header="Batch" Width="175" />
              <ext:DateColumn DataIndex="d_expired" Header="Masa berlaku" Format="dd-MM-yyyy" />
            </Columns>
          </ColumnModel>
          <View>
            <ext:GridView ID="GridView1" runat="server" StandardHeaderRow="true">
              <HeaderRows>
                <ext:HeaderRow>
                  <Columns>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:TextField ID="txBatchFltr" runat="server" EnableKeyEvents="true" AllowBlank="true">
                          <Listeners>
                            <KeyUp Handler="reloadFilterGrid(#{gridDetail})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:TextField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn>
                      <Component>
                        <ext:DateField ID="txBatchEDFltr" runat="server" Format="dd-MM-Y" EnableKeyEvents="true" AllowBlank="true">
                          <Listeners>
                            <Change Handler="reloadFilterGrid(#{gridDetail})" Buffer="300" Delay="300" />
                          </Listeners>
                        </ext:DateField>
                      </Component>
                    </ext:HeaderColumn>
                    <ext:HeaderColumn />
                  </Columns>
                </ext:HeaderRow>
              </HeaderRows>
            </ext:GridView>
          </View>
          <BottomBar>
            <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="10">
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
                  <SelectedItem Value="100" />
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
</ext:Window>
