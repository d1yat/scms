<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaGoodCtrl.ascx.cs" Inherits="transaksi_retur_SerahTerimaGoodCtrl" %>

<ext:Window ID="winDetail" runat="server" Height="510" Width="875" Hidden="true" Maximizable="true" MinHeight="510" MinWidth="850" Layout="Fit">
    <Content>
        <ext:Hidden ID="hfgudang" runat="server" />
        <ext:Hidden ID="hfSTno" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
    </Content>
    <Items>
        <ext:BorderLayout ID="bllayout" runat="server">
          <North MinHeight="210" MaxHeight="210" Collapsible="false">
            <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="210" Padding="10">
              <Items>
                <ext:TextField ID="txSTno" runat="server" FieldLabel="Nomor ST" Width="400" ReadOnly="true" />
                <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Tanggal Proses ST">
                    <Items>
                        <ext:DateField ID="dfTanggal" runat="server" Width="400" ReadOnly="true" Format="dd-MM-yyyy" EnableKeyEvents="true" />
                    </Items>
                </ext:CompositeField>            
                <ext:TextField ID="txCab" runat="server" FieldLabel="Cabang" Width="400" ReadOnly="true" />
                <ext:TextField ID="txRCno" runat="server" FieldLabel="Nomor RC" Width="400" ReadOnly="true" />
                <ext:TextField ID="txPBBRno" runat="server" FieldLabel="Nomor PBB" Width="400" ReadOnly="true" />
                <ext:Checkbox ID="chkConfirm" runat="server" FieldLabel="Confirm" Width="400" />
              </Items>
            </ext:FormPanel>
          </North>
          <Center MinHeight="150">
            <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
                <Items>
                    <ext:GridPanel ID="griddetail" runat="server">
                        <SelectionModel>
                            <ext:RowSelectionModel SingleSelect="true" />
                        </SelectionModel>
                        <Store>
                            <ext:Store runat="server" RemotePaging="false" RemoteSort="false">
                                <Proxy>
                                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000" CallbackParam="soaScmsCallback" />
                                </Proxy>
                                <BaseParams>
                                    <ext:Parameter Name="start" Value="0" />
                                    <ext:Parameter Name="limit" Value="-1" />
                                    <ext:Parameter Name="model" Value="0362" />
                                    <ext:Parameter Name="parameters" Value="[['v_stno = @0', #{hfSTno}.getValue(), 'System.String']]" Mode="Raw" />
                                </BaseParams>
                                <Reader>
                                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                                        <Fields>
                                            <ext:RecordField Name="c_iteno" />
                                            <ext:RecordField Name="v_itnam" />
                                            <ext:RecordField Name="c_batch" />
                                            <ext:RecordField Name="n_qty" />
                                            <ext:RecordField Name="n_qtyterima" />
                                            <ext:RecordField Name="n_qtyreject" />
                                        </Fields>
                                    </ext:JsonReader>
                                </Reader>
                            </ext:Store>
                        </Store>
                        <ColumnModel>
                            <Columns>
                                <ext:CommandColumn Width="50" Resizable="false"></ext:CommandColumn>
                                <ext:Column ColumnID="iteno" DataIndex="c_iteno" Header="Kode Item" />
                                <ext:Column ColumnID="itnam" DataIndex="v_itnam" Header="Nama Item" />
                                <ext:Column ColumnID="batch" DataIndex="c_batch" Header="Batch" />
                                <ext:NumberColumn ColumnID="qty" DataIndex="n_qty" Header="Qty Serah Terima" Format="0.000,00/i"/>
                                <ext:NumberColumn ColumnID="qtyterima" DataIndex="n_qtyterima" Header="Qty Terima" Format="0.000,00/i">
                                    <Editor>
                                        <ext:NumberField ID="NumberField1" runat="server" AllowDecimals="true" AllowNegative="false" MinValue="0" DecimalPrecision="2" />
                                    </Editor>
                                </ext:NumberColumn>
                                <ext:NumberColumn ColumnID="qtyreject" DataIndex="n_qtyreject" Header="Qty Reject" Format="0.000,00/i"/>
                            </Columns>
                        </ColumnModel>
                    </ext:GridPanel>
                </Items>
            </ext:Panel>
          </Center>
        </ext:BorderLayout>
    </Items>
    <Buttons>
        <ext:Button ID="btnsave" Text="Save" runat="server" Icon="Disk">
            <DirectEvents>
                <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});" ConfirmRequest="true" Title="Save" Message="Anda yakin ingin simpan data ini?" />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{griddetail}.getStore())"
                      Mode="Raw" />
                    <ext:parameter Name="stno" Value="#{txSTno}.getValue()" Mode="raw"/>
                    <ext:Parameter Name="confirm" Value="#{chkConfirm}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
            </DirectEvents>
        </ext:Button>
        <ext:Button ID="Button1" runat="server" Icon="Cancel" Text="Keluar">
            <Listeners>
                <Click Handler="#{winDetail}.hide();" />
            </Listeners>
        </ext:Button>
    </Buttons>
</ext:Window>