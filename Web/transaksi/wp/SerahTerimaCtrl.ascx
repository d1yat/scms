<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaCtrl.ascx.cs" Inherits="transaksi_wp_SerahTerimaCtrl" %>

<ext:Window runat="server" ID="wndIDUser" Title="Validasi User" Height="480" Width="600" Hidden="true"
  MinHeight="480" MinWidth="600" Layout="Fit" Closable="false" >
   <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="190" MaxHeight="180" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Border="false" Padding="10" Height="130">
          <Items>
            
            <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Penerima" >
              <Items>
                <ext:TextField ID="txNipPenerima" runat="server" AllowBlank="false" Width="100" MaxLength="10" Disabled="true">
                  
                </ext:TextField>
                <ext:Label ID="Label3" runat="server" Text="-" />
                <ext:TextField ID="txNamePenerima" runat="server" AllowBlank="false" Disabled="true" />
              </Items>
            </ext:CompositeField>
            <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Penyerah">
              <Items>
                <ext:TextField ID="txNipPenyerah" runat="server" AllowBlank="false" Width="100" MaxLength="10" >
                  <DirectEvents>
                    <Change Buffer="300" OnEvent="text_onchange"></Change>
                  </DirectEvents>
                </ext:TextField>
                <ext:Label ID="Label2" runat="server" Text="-" />
                <ext:TextField ID="txNamePenyerah" runat="server" AllowBlank="false" Disabled="true" />
              </Items>
            </ext:CompositeField>
          </Items>
          <Buttons>
            <ext:Button ID="Button1" runat="server" Text="OK" Icon="Accept">
              <DirectEvents>
                <Click OnEvent="btn_onclick"></Click>
              </DirectEvents>
            </ext:Button>
          </Buttons>
        </ext:FormPanel> 
      </North>
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="Daftar Items" Height="150" Layout="Fit">
          <TopBar>
            <ext:Toolbar ID="Toolbar1" runat="server">
              <Items>
                <ext:FormPanel ID="frmpnlDetailEntry" runat="server" Frame="True" Layout="Table"
                  LabelAlign="Top" Border="false" BaseCls="x-plain" Padding="5">
                  <Items>
                    <ext:TextField ID="txDO" runat="server" Width="100" MaxLength="10" MaxLengthText="100" />
                  </Items>
                </ext:FormPanel>
              </Items>
            </ext:Toolbar>
          </TopBar>
          <Items>
            <ext:GridPanel ID="gridDetail" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <%--<ext:Parameter Name="model" Value="0002" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[['c_plno = @0', #{hfPlNo}.getValue(), 'System.String']]"
                      Mode="Raw" />--%>
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="c_iteno" />
                        <ext:RecordField Name="v_itemdesc" />
                        <ext:RecordField Name="c_sp" />
                        <ext:RecordField Name="c_spc" />
                        <ext:RecordField Name="v_undes" />
                        <ext:RecordField Name="c_batch" />
                        <ext:RecordField Name="n_booked" Type="Float" />
                        <ext:RecordField Name="n_QtyRequest" Type="Float" />
                        <ext:RecordField Name="n_lastqtyRequest" Type="Float" />
                        <ext:RecordField Name="n_sisa" Type="Float" />
                        <ext:RecordField Name="l_new" Type="Boolean" />
                        <ext:RecordField Name="l_void" Type="Boolean" />
                        <ext:RecordField Name="l_modified" Type="Boolean" />
                        <ext:RecordField Name="v_ket" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                  <ext:CommandColumn Width="25">
                    <Commands>
                      <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                      <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                    </Commands>
                    <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfPlNo}.getValue());" />
                  </ext:CommandColumn>
                  <ext:Column DataIndex="c_iteno" Header="Kode" Width="50" />
                  <ext:Column DataIndex="v_itemdesc" Header="Nama Barang" Width="200" />
                  <ext:Column DataIndex="v_undes" Header="Kemasan" />
                  <ext:Column DataIndex="c_spc" Header="SP Cabang" />
                  <ext:Column DataIndex="c_batch" Header="Batch" />
                  <ext:NumberColumn DataIndex="n_booked" Header="Alokasi" Format="0.000,00/i" Width="75" />
                  <ext:NumberColumn DataIndex="n_QtyRequest" Header="Terpenuhi" Format="0.000,00/i"
                    Width="75" >
                    <Editor>
                      <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="false" DecimalPrecision="2" MinValue="0" />
                    </Editor>
                  </ext:NumberColumn>
                  <ext:CheckColumn DataIndex="l_void" Header="Batal" Width="50" />
                </Columns>
              </ColumnModel>
              <Listeners>
                <%--<Command Handler="if(command == 'Delete') { deleteRecordOnGrid(this, record); } else if (command == 'Void') { voidInsertedDataFromStore(record); }" />
                <BeforeEdit Fn="onGridBeforeEdit" />
                <AfterEdit Fn="onGridAfterEdit" />--%>
              </Listeners>
            </ext:GridPanel>
          </Items>
          <Buttons>
            <ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
              <%--<DirectEvents>
                <Click OnEvent="Report_OnGenerate" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                    ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>--%>
            </ext:Button>
            <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan">
              <%--<DirectEvents>
                <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
                  <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
                    ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                  <EventMask ShowMask="true" />
                  <ExtraParams>
                    <ext:Parameter Name="gridValues" Value="saveStoreToServer(#{gridDetail}.getStore())"
                      Mode="Raw" />
                    <ext:Parameter Name="NumberID" Value="#{hfPlNo}.getValue()" Mode="Raw" />
                    <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
                  </ExtraParams>
                </Click>
              </DirectEvents>--%>
            </ext:Button>
            <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
              <DirectEvents>
                <Click OnEvent="ReloadBtn_Click">
                  <EventMask ShowMask="true" />
                </Click>
              </DirectEvents>
            </ext:Button>
            <ext:Button ID="Button2" runat="server" Icon="Cancel" Text="Keluar">
              <Listeners>
                <Click Handler="#{winDetail}.hide();" />
              </Listeners>
            </ext:Button>
          </Buttons>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
   </Items>
</ext:Window>