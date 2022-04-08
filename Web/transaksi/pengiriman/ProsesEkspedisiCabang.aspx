<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProsesEkspedisiCabang.aspx.cs" 
Inherits="transaksi_pengiriman_ProsesEkspedisiCabang" MasterPageFile="~/Master.master" %>

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
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="Panel1" runat="server" Layout="Fit" Border="false" AutoDoLayout="true">
        <Items>
          <ext:BorderLayout ID="bllayout" runat="server">
            <North Collapsible="false" MinHeight="135" Split="false">
              <ext:FormPanel ID="frmHeaders" runat="server" Height="135" Padding="5" ButtonAlign="Center"
                Title="Ekspedisi Header">
                <Items>
                  <ext:FileUploadField ID="fuImportExp" runat="server" FieldLabel="Data file" EmptyText="Data file Exp..."
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
                        <ExtraParams>
                          <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                            Mode="Raw" />
                        </ExtraParams>
                      </Click>
                    </DirectEvents>
                  </ext:Button>
                </Buttons>
              </ext:FormPanel>
            </North>
            <Center MinHeight="125">
              <ext:Panel ID="Panel2" runat="server" Title="Details Item" Layout="Fit">
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
                      <ext:Store ID="Store1" runat="server" SkinID="OriginalExtStore" RemoteSort="false" RemotePaging="false"
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
                          <ext:ArrayReader>
                            <Fields>
                              <ext:RecordField Name="expno" />
                              <ext:RecordField Name="resino" />
                              <ext:RecordField Name="tglresi" />
                              <ext:RecordField Name="tglexpcab" />
                              <ext:RecordField Name="wktexpcab" />
                              <ext:RecordField Name="Dtglresi" Type="Date" DateFormat="M$" />
                              <ext:RecordField Name="Dtglexpcab" Type="Date" DateFormat="M$" />
                              <ext:RecordField Name="Twktexpcab" Type="Date" />
                              <ext:RecordField Name="v_ket" />
                              <ext:RecordField Name="c_type" />
                              <ext:RecordField Name="l_new" />
                            </Fields>
                          </ext:ArrayReader>
                        </Reader>
                        <SortInfo Field="expno" Direction="ASC" />
                      </ext:Store>
                    </Store>
                    <ColumnModel>
                      <Columns>
                        <ext:CommandColumn Width="25">
                          <Commands>
                            <ext:GridCommand CommandName="Delete" Icon="Delete" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                          </Commands>
                        </ext:CommandColumn>
                        <ext:Column DataIndex="expno" Header="No Expdisi" Width="100" />
                        <ext:DateColumn DataIndex="Dtglresi" Header="Tgl Resi" Format="dd-MM-yyyy"
                          Editable="true">
                          <Editor>
                            <ext:DateField runat="server" Format="dd-MM-yyyy" />
                          </Editor>
                        </ext:DateColumn>
                        <ext:DateColumn DataIndex="Dtglexpcab" Header="Tgl Expedisi Cabang" Width="140" Format="dd-MM-yyyy">
                          <Editor>
                            <ext:DateField runat="server" Format="dd-MM-yyyy" />
                          </Editor>
                        </ext:DateColumn>
                        <ext:Column DataIndex="wktexpcab" Header="Waktu Expedisi Cabang" Width="140">
                          <Editor>
                            <ext:TimeField runat="server" Format="HH:mm:ss"/>
                          </Editor>
                        </ext:Column>
                        <ext:Column DataIndex="v_ket" Header="Keterangan" Width="175" />
                        <ext:Column DataIndex="l_new" Header="Exp Id" Hidden="true" Width="50" />
                      </Columns>
                    </ColumnModel>
                    <BottomBar>
                      <ext:PagingToolbar ID="gmPagingBB" runat="server" PageSize="20" HideRefresh="true">
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
        <Buttons>
        <%--<ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Simpan">
          <DirectEvents>
            <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
              <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
                ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                  Mode="Raw" />
              </ExtraParams>
            </Click>
          </DirectEvents>
        </ext:Button>--%>
      </Buttons>
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