<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterUserAPJCtrl.ascx.cs"
    Inherits="master_ams_MasterUserAPJCtrl" %>
<ext:Window ID="winDetail" runat="server" Height="350" Width="800" Hidden="true"
    Maximizable="true" MinHeight="300" MinWidth="800" Layout="Fit">
    <Content>
        <ext:Hidden ID="hfNip" runat="server" />
        <ext:Hidden ID="hfCabang" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
        
    </Content>
    <Items>
        <ext:FormPanel ID="frmHeaders" runat="server" Layout="Fit" BodyBorder="false" Frame="false">
            <Items>
                <ext:TabPanel ID="TabPanel1" runat="server" ActiveTabIndex="0" Frame="false" Border="false"
                    DeferredRender="false">
                    <Items>
                        <ext:Panel ID="pnlHeaders1" runat="server" Title="Umum" Height="195" Padding="10"
                            Layout="Column">
                            <Items>
                                <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".5"
                                    Layout="Form" LabelAlign="Left">
                                    <Items>
                                        <ext:TextField runat="server" FieldLabel="Nip" ID="txNip" AllowBlank="false" />
                                        <ext:TextField runat="server" FieldLabel="Nama" ID="txNama" Width="200" />
                                        <%--<ext:TextArea runat="server" FieldLabel="Kode Cabang" ID="txCusNo" AllowBlank="false" />--%>
                                        
                                        <ext:ComboBox ID="cbCustomer" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
                                            ValueField="c_cusno" Width="350" ItemSelector="tr.search-item" PageSize="10"
                                            ListWidth="400" MinChars="3">
                                            <Store>
                                                <ext:Store ID="Store1" runat="server">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="={10}" />
                                                        <ext:Parameter Name="model" Value="2011" />
                                                        <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                      ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomerHdr}), '']]"
                                                            Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="v_cunam" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                                                            TotalProperty="d.totalRows">
                                                            <Fields>
                                                                <ext:RecordField Name="c_cusno" />
                                                                <ext:RecordField Name="v_cunam" />
                                                                <ext:RecordField Name="c_cab" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <Template ID="Template1" runat="server">
                                                <Html>
                                                <table cellpading="0" cellspacing="1" style="width: 400px">
                                                <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                                                <tpl for="."><tr class="search-item">
                                                    <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
                                                </tr></tpl>
                                                </table>
                                                </Html>
                                            </Template>
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                            </Triggers>
                                            <Listeners>
                                                <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                                                <Change Handler="resetEntryWhenChange(#{gridDetail}, #{frmpnlDetailEntry});" />
                                            </Listeners>
                                        </ext:ComboBox>
                                        
                                        <ext:FileUploadField ID="fuImage" runat="server" FieldLabel="Data file" EmptyText="Data file Image..."
                                            ButtonText="Pilih..." Icon="Attach" Width="400" AllowBlank="true" />
                                        <ext:TextField runat="server" FieldLabel="No.SIK" ID="txNoSik" />
                                        <ext:TextField runat="server" FieldLabel="No.Pbf" ID="txNoPbf" AllowBlank="false" />
                                        <ext:TextField runat="server" FieldLabel="Kode Area" ID="txKodeArea" AllowBlank="false" />
                                        <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
                                            ValueField="c_gdg" Width="150" ItemSelector="tr.search-item" PageSize="10" ListWidth="200"
                                            MinChars="3" AllowBlank="false">
                                            <Store>
                                                <ext:Store ID="Store3" runat="server">
                                                    <Proxy>
                                                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                                            CallbackParam="soaScmsCallback" />
                                                    </Proxy>
                                                    <BaseParams>
                                                        <ext:Parameter Name="start" Value="={0}" />
                                                        <ext:Parameter Name="limit" Value="={10}" />
                                                        <ext:Parameter Name="model" Value="2031" />
                                                        <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
                                                        <ext:Parameter Name="sort" Value="v_gdgdesc" />
                                                        <ext:Parameter Name="dir" Value="ASC" />
                                                    </BaseParams>
                                                    <Reader>
                                                        <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                                                            <Fields>
                                                                <ext:RecordField Name="c_gdg" />
                                                                <ext:RecordField Name="v_gdgdesc" />
                                                            </Fields>
                                                        </ext:JsonReader>
                                                    </Reader>
                                                </ext:Store>
                                            </Store>
                                            <Template ID="Template2" runat="server">
                                                <Html>
                                                <table cellpading="0" cellspacing="1" style="width: 200px">
                      <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                      <tpl for="."><tr class="search-item">
                      <td>{c_gdg}</td><td>{v_gdgdesc}</td>
                      </tr></tpl>
                      </table>
                                                </Html>
                                            </Template>
                                            <Triggers>
                                                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                                            </Triggers>
                                        </ext:ComboBox>
                                        
                                    </Items>
                                </ext:Panel>
                            </Items>
                        </ext:Panel>
                    </Items>
                </ext:TabPanel>
            </Items>
        </ext:FormPanel>
    </Items>
    <Buttons>
        <ext:Button runat="server" ID="btnSimpan" Icon="Disk" Text="Simpan">
            <DirectEvents>
                <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
                    <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});" ConfirmRequest="true"
                        Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
                    <EventMask ShowMask="true" />
                    <ExtraParams>
                        <ext:Parameter Name="NumberID" Value="#{hfNip}.getValue()" Mode="Raw" />
                        <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
                    </ExtraParams>
                </Click>
            </DirectEvents>
        </ext:Button>
        <ext:Button ID="btnReload" runat="server" Icon="Reload" Text="Bersihkan">
            <DirectEvents>
                <Click OnEvent="ReloadBtn_Click">
                    <EventMask ShowMask="true" />
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
