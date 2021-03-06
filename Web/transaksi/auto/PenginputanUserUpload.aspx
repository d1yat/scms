<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PenginputanUserUpload.aspx.cs" Inherits="transaksi_auto_PenginputanUserUpload"
 MasterPageFile="~/Master.master" %>

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
            <North MinHeight="125">
              <ext:FormPanel ID="frmHeaders" runat="server" Height="135" Padding="5" ButtonAlign="Center"
                Title="Upload Header">
                <Items>
                  <ext:FileUploadField ID="fuImportFB" runat="server" FieldLabel="Data file" EmptyText="Data file Excell..."
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
              <ext:Panel ID="Panel2" runat="server" Title="Details Item" Layout="Fit">
                <Items>
                  <ext:GridPanel ID="gridDetail" runat="server">
                    <LoadMask ShowMask="true" />
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
                              <ext:RecordField Name="No" />
                              <ext:RecordField Name="Nip" />
                              <ext:RecordField Name="Nama" />
                              <ext:RecordField Name="Tgl_Lahir" />
                              <ext:RecordField Name="Gudang" />
                              <ext:RecordField Name="Aktif" />

                             <%-- <ext:RecordField Name="d_date" Type="Date" DateFormat="M$"/>  --%>                    
                            </Fields>
                          </ext:ArrayReader>
                        </Reader>
                      </ext:Store>
                    </Store>
                    <ColumnModel>
                      <Columns>
                        <ext:Column DataIndex="No" Header="No" />
                        <ext:Column DataIndex="Nip" Header="NIP" />
                        <ext:Column DataIndex="Nama" Header="NAMA Karyawan" />
                        <ext:Column DataIndex="Tgl_Lahir" Header="Tanggal Lahir" />                        
                        <ext:Column DataIndex="Gudang" Header="Gudang" />
                        <ext:Column DataIndex="Aktif" Header="Aktif" /> 
                       <%-- <ext:DateColumn DataIndex="d_date" Header="Tanggal"  Format="dd-MM-yyyy"/>--%>
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
      </ext:Panel>
     </Items>
  </ext:Viewport>
</asp:Content>