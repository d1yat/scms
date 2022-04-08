<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="FakturBeliProses.aspx.cs" Inherits="keuangan_faktur_FakturBeliProses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script language="javascript" type="text/javascript">
    var onSuccessProcess = function() {
      ;
    }
    var pagingSelected = function(pg, val) {
      pg.pageSize = parseInt(val);
      pg.doLoad();
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport runat="server" Layout="Fit">
    <Items>
      <ext:Panel runat="server" Layout="Fit" Border="false" AutoDoLayout="true">
        <Items>
          <ext:BorderLayout ID="bllayout" runat="server">
            <North Collapsible="false" MinHeight="135" Split="false">
              <ext:FormPanel ID="frmHeaders" runat="server" Height="135" Padding="5" ButtonAlign="Center"
                Title="Faktur Header">
                <Items>
                  <ext:FileUploadField ID="fuImportFB" runat="server" FieldLabel="Data file" EmptyText="Data file FB..."
                    ButtonText="Pilih..." Icon="Attach" Width="400" AllowBlank="false" />
                  <ext:Checkbox ID="chkVerify" runat="server" FieldLabel="Verifikasi" Checked="true" />
                </Items>
                <Listeners>
                  <ClientValidation Handler="#{btnProses}.setDisabled(!valid);" />
                </Listeners>
                <Buttons>
                  <ext:Button ID="btnProses" runat="server" Text="Proses" Icon="CogStart">
                    <DirectEvents>
                      <Click OnEvent="Proses_OnClick" Before="return validasiProses(#{frmHeaders});" Success="onSuccessProcess();">
                        <Confirmation BeforeConfirm="return validasiProses(#{frmHeaders});" ConfirmRequest="true"
                          Title="Simpan ?" Message="Anda yakin ingin memproses data ini." />
                        <EventMask ShowMask="true" />
                      </Click>
                    </DirectEvents>
                  </ext:Button>
                </Buttons>
              </ext:FormPanel>
            </North>
            <Center MinHeight="125">
              <ext:Panel runat="server" Title="Details Item" Layout="Fit">
                <Items>
                  <ext:GridPanel ID="gridDetail" runat="server">
                    <LoadMask ShowMask="true" />
                    <Listeners>
                      <Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); }" />
                    </Listeners>
                    <SelectionModel>
                      <ext:RowSelectionModel SingleSelect="true" />
                    </SelectionModel>
                    <Store>
                      <ext:Store runat="server" SkinID="OriginalExtStore" RemoteSort="false" RemotePaging="false"
                        RemoteGroup="false" AutoLoad="false">
                        <AutoLoadParams>
                          <ext:Parameter Name="start" Value="0" />
                          <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                        </AutoLoadParams>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="0" />
                          <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                        </BaseParams>
                        <Reader>
                          <%--<ext:ArrayReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">--%>
                          <%--<ext:ArrayReader TotalProperty="totalRows" Root="data" SuccessProperty="success">--%>
                          <ext:ArrayReader>
                            <Fields>
                              <ext:RecordField Name="Gudang" />
                              <ext:RecordField Name="RnNo" />
                              <ext:RecordField Name="DoNo" />
                              <ext:RecordField Name="Supplier" />
                              <ext:RecordField Name="Keterangan" />
                            </Fields>
                          </ext:ArrayReader>
                        </Reader>
                      </ext:Store>
                    </Store>
                    <ColumnModel>
                      <Columns>
                        <ext:Column DataIndex="Gudang" Header="Gudang" Width="50" />
                        <ext:Column DataIndex="RnNo" Header="No. RN" />
                        <ext:Column DataIndex="DoNo" Header="No. DO" />
                        <ext:Column DataIndex="Keterangan" Header="Keterangan" Width="200" />
                      </Columns>
                    </ColumnModel>
                    <BottomBar>
                      <ext:PagingToolbar ID="gmPagingBB" runat="server" PageSize="20" HideRefresh="true">
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
                              <Select Handler="pagingSelected(#{gmPagingBB}, this.getValue());" />
                            </Listeners>
                          </ext:ComboBox>
                        </Items>
                      </ext:PagingToolbar>
                    </BottomBar>
                  </ext:GridPanel>
                </Items>
              </ext:Panel>
            </Center>
          </ext:BorderLayout>
        </Items>
      </ext:Panel>
    </Items>
  </ext:Viewport>

  <script type="text/javascript" language="javascript">
    Ext.ux.data.PagingStore.override({
      loadData: function(o, append) {
        var options = {},
                r;
        //this.fireEvent("beforeload", this, options);
        var cbPaging = Ext.getCmp('<%= cbGmPagingBB.ClientID %>');

        var newParams = Ext.apply({},
        [
          {
            'start': 0,
            'limit': parseInt(cbPaging.getValue())
          }
        ], null);

        //          [
        //            {'start': 0},
        //            {'limit': parseInt(<%= cbGmPagingBB.ClientID %>.getValue())}
        //          ];
        this.isPaging(Ext.apply({}, this.lastOptions && !Ext.isEmptyObj(this.lastOptions) ? this.lastOptions.params : options.params, newParams[0]));
        r = this.reader.readRecords(o);
        this.loadRecords(r, Ext.apply({ add: append }, this.lastOptions || {}), true);
      }
    });
  </script>

</asp:Content>
