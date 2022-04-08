<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="FakturBeliReturProses.aspx.cs" Inherits="keuangan_faktur_FakturBeliReturProses" %>

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
                  <ext:FileUploadField ID="fuImportRS" runat="server" FieldLabel="Data file" EmptyText="Data file RS..."
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
                              <ext:RecordField Name="c_gdg" />
                              <ext:RecordField Name="c_rsno" />
                              <ext:RecordField Name="c_iteno" />
                              <ext:RecordField Name="v_itnam" />
                              <ext:RecordField Name="c_batch" />
                              <ext:RecordField Name="c_fb" />
                              <ext:RecordField Name="c_taxno" />
                              <ext:RecordField Name="d_taxdate" Type="Date" DateFormat="M$" />
                              <ext:RecordField Name="n_disc" Type="Float" />
                              <ext:RecordField Name="n_salpri" Type="Float" />
                              <ext:RecordField Name="n_gsisa" Type="Float" />
                              <ext:RecordField Name="n_bsisa" Type="Float" />
                              <ext:RecordField Name="c_up_exfaktur" />
                              <ext:RecordField Name="c_up_taxno" />
                              <ext:RecordField Name="d_up_taxdate" Type="Date" DateFormat="M$" />
                              <ext:RecordField Name="n_up_salpri" />
                              <ext:RecordField Name="n_up_disc" />
                              <ext:RecordField Name="n_up_gsisa" />
                              <ext:RecordField Name="n_up_bsisa" />
                              <ext:RecordField Name="l_void" Type="Boolean" />
                              <ext:RecordField Name="v_ket" />
                            </Fields>
                          </ext:ArrayReader>
                        </Reader>
                        <SortInfo Field="v_itnam" Direction="ASC" />
                      </ext:Store>
                    </Store>
                    <ColumnModel>
                      <Columns>
                        <ext:CommandColumn Width="25">
                          <Commands>
                            <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                          </Commands>
                        </ext:CommandColumn>
                        <ext:Column DataIndex="c_rsno" Header="Nomor" />
                        <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                        <ext:Column DataIndex="v_itnam" Header="Nama" Width="250" />
                        <ext:Column DataIndex="c_batch" Header="Batch" />
                        <%--<ext:Column DataIndex="c_fb" Header="Ex. Faktur" />
                        <ext:Column DataIndex="c_taxno" Header="Pajak" Width="150" />
                        <ext:DateColumn DataIndex="d_taxdate" Header="Tgl. Pajak" Format="dd-MM-yyyy" />--%>
                        <ext:NumberColumn DataIndex="n_disc" Header="Disc" Format="0.000,00/i" Width="75" />
                        <ext:NumberColumn DataIndex="n_salpri" Header="Harga" Format="0.000,00/i" />
                        <ext:NumberColumn DataIndex="n_gsisa" Header="G. Sisa" Format="0.000,00/i" />
                        <ext:NumberColumn DataIndex="n_bsisa" Header="B. Sisa" Format="0.000,00/i" />
                        <ext:Column DataIndex="c_up_exfaktur" Header="Imp. Ex. Faktur" Editable="true">
                          <Editor>
                            <ext:TextField runat="server" MaxLength="20" />
                          </Editor>
                        </ext:Column>
                        <ext:Column DataIndex="c_up_taxno" Header="Imp. Pajak" Width="150" Editable="true">
                          <Editor>
                            <ext:TextField runat="server" MaxLength="20" />
                          </Editor>
                        </ext:Column>
                        <ext:DateColumn DataIndex="d_up_taxdate" Header="Imp. Tgl. Pajak" Format="dd-MM-yyyy"
                          Editable="true">
                          <Editor>
                            <ext:DateField runat="server" Format="dd-MM-yyyy" />
                          </Editor>
                        </ext:DateColumn>
                        <ext:NumberColumn DataIndex="n_up_salpri" Header="Imp. Harga" Format="0.000,00/i">
                          <Editor>
                            <ext:NumberField runat="server" AllowNegative="false" />
                          </Editor>
                        </ext:NumberColumn>
                        <ext:NumberColumn DataIndex="n_up_disc" Header="Imp. Disc" Format="0.000,00/i" Width="75">
                          <Editor>
                            <ext:NumberField runat="server" AllowNegative="false" />
                          </Editor>
                        </ext:NumberColumn>
                        <ext:NumberColumn DataIndex="n_up_gsisa" Header="Imp. G. Sisa" Format="0.000,00/i">
                          <Editor>
                            <ext:NumberField runat="server" AllowNegative="false" />
                          </Editor>
                        </ext:NumberColumn>
                        <ext:NumberColumn DataIndex="n_up_bsisa" Header="Imp. B. Sisa" Format="0.000,00/i">
                          <Editor>
                            <ext:NumberField runat="server" AllowNegative="false" />
                          </Editor>
                        </ext:NumberColumn>
                        <ext:Column DataIndex="v_ket" Header="Keterangan" />
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
