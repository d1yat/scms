<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterItemCtrl.ascx.cs"
  Inherits="master_item_MasterItemCtrl" %>
<ext:Window ID="winDetail" runat="server" Height="425" Width="800" Hidden="true"
  Maximizable="true" MinHeight="425" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfItemNo" runat="server" />
  </Content>
  <Items>
    <ext:FormPanel ID="frmHeaders" runat="server" Height="425" Padding="10">
      <Items>
        <ext:FormPanel ID="FormPanel1" runat="server" Header="false" BodyBorder="false" MonitorPoll="500"
          MonitorValid="true" Padding="0" Width="1024" Height="425" ButtonAlign="Right" Layout="Column">
          <Items>
            <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" ColumnWidth=".3"
              Layout="Form" LabelAlign="Left">
              <Items>
                <ext:Label runat="server" ID="lbKodeItem" FieldLabel="Kode Item"/>
                <ext:Label runat="server" ID="lblItem" FieldLabel="Nama Item"/>
                <ext:Label runat="server" ID="lblPrinsipal" FieldLabel="Nama Prinsipal"/>                
                <ext:Label runat="server" ID="lblMeasure" FieldLabel="Measurement"/>
                <ext:Label runat="server" ID="lblAcronim" FieldLabel="Acronim"/>
                <%--<ext:Label runat="server" ID="lblNoAlkes" FieldLabel="No Alkes"/>--%>
                <ext:TextField runat="server" ID="txNoAlkes" FieldLabel="No Alkes" MaxLength="16" />
                <ext:Label runat="server" ID="lblPrice" FieldLabel="Price" />
                <ext:Label runat="server" ID="lblDisc" FieldLabel="Disc" />
                <%--<ext:Label runat="server" ID="lblBox" FieldLabel="Box" />--%>
                <ext:NumberField runat="server" ID="txBox" AllowNegative="false" FieldLabel="Box" AllowDecimals="true" />
                <ext:Label runat="server" ID="lblQtyKons" FieldLabel="Qty Kons" />
                <ext:Label runat="server" ID="lblMinP" FieldLabel="Min Price" />
                <ext:NumberField runat="server" ID="txMinQ" AllowNegative="false" FieldLabel="Min Qty" AllowBlank="false" MinValue="0" AllowDecimals="true" />
                <ext:NumberField runat="server" ID="txHET" AllowNegative="false" FieldLabel="H E T" AllowDecimals="true" />
                <ext:Label runat="server" ID="lblVia" FieldLabel="Via" Width="100" />              
              </Items>
            </ext:Panel>
            <ext:Panel ID="Panel2" runat="server" Border="false" Header="false" Width="1000" ColumnWidth=".7"
              Layout="Form" LabelAlign="Left">
              <Items>
                <ext:Label runat="server" ID="lblBonus" FieldLabel="Bonus" />
                <ext:Label runat="server" ID="lblPembelian" FieldLabel="Pembelian" />
                <ext:Label runat="server" ID="lblEstimasi" FieldLabel="Estimasi" />
                <ext:Label runat="server" ID="lblTipe" FieldLabel="Tipe" Width="100" />
                <%--<ext:ComboBox ID="cbViaHdr" runat="server" FieldLabel="Via" DisplayField="v_ket"
                    ValueField="c_type" Width="250" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
                    MinChars="3" AllowBlank="false">
                    <Store>
                      <ext:Store ID="Store1" runat="server">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <AutoLoadParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={20}" />
                        </AutoLoadParams>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={10}" />
                          <ext:Parameter Name="model" Value="2001" />
                          <ext:Parameter Name="parameters" Value="[[['c_portal = @0', '3', 'System.Char'],
                                  ['c_notrans = @0', '02', '']]]" Mode="Raw" />
                          <ext:Parameter Name="sort" Value="v_ket" />
                          <ext:Parameter Name="dir" Value="ASC" />
                        </BaseParams>
                        <Reader>
                          <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                            TotalProperty="d.totalRows">
                            <Fields>
                              <ext:RecordField Name="c_type" />
                              <ext:RecordField Name="v_ket" />
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                      </ext:Store>
                    </Store>
                    <Template ID="Template1" runat="server">
                      <Html>
                      <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Oleh</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{v_ket}</td>
                        </tr></tpl>
                        </table>
                      </Html>
                    </Template>
                    <Triggers>
                      <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                    </Triggers>
                    <Listeners>
                      <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                      <Change Handler="clearRelatedComboRecursive(true, #{cbCustomerHdr});" />
                    </Listeners>
                  </ext:ComboBox>
                  <ext:ComboBox ID="cbTipeProduk" runat="server" FieldLabel="Tipe Produk" DisplayField="v_ket"
                    ValueField="c_type" Width="100" ItemSelector="tr.search-item" PageSize="10" ListWidth="250"
                    MinChars="3" AllowBlank="false">
                    <Store>
                      <ext:Store ID="Store2" runat="server">
                        <Proxy>
                          <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                            CallbackParam="soaScmsCallback" />
                        </Proxy>
                        <BaseParams>
                          <ext:Parameter Name="start" Value="={0}" />
                          <ext:Parameter Name="limit" Value="={10}" />
                          <ext:Parameter Name="model" Value="2001" />
                          <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '11', 'System.String'],
                                          ['c_portal = @0', '3', 'System.Char']]" Mode="Raw" />
                          <ext:Parameter Name="sort" Value="c_notrans" />
                          <ext:Parameter Name="dir" Value="ASC" />
                        </BaseParams>
                        <Reader>
                          <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                            TotalProperty="d.totalRows">
                            <Fields>
                              <ext:RecordField Name="c_type" />
                              <ext:RecordField Name="v_ket" />
                            </Fields>
                          </ext:JsonReader>
                        </Reader>
                      </ext:Store>
                    </Store>
                    <Template ID="Template2" runat="server">
                      <Html>
                      <table cellpading="0" cellspacing="1" style="width: 250px">
                      <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                      <tpl for="."><tr class="search-item">
                      <td>{c_type}</td><td>{v_ket}</td>
                      </tr></tpl>
                      </table>
                      </Html>
                    </Template>
                  </ext:ComboBox>--%>
                <ext:Checkbox ID="chkBerat" runat="server" FieldLabel="Berat" />
                <ext:Checkbox ID="chkDinkes" runat="server" FieldLabel="Dinkes" />
                <ext:Checkbox ID="chkMultiP" runat="server" FieldLabel="Multi Price" />
                <ext:Checkbox ID="chkPPNBM" runat="server" FieldLabel="PPN BM" />
                <ext:Checkbox ID="chkCombo" runat="server" FieldLabel="Combo" />
                <ext:Checkbox ID="chkSP" runat="server" FieldLabel="SP Khusus" />
                <ext:TextField ID="txNie" runat="server" FieldLabel="Nomor Ijin Edar (NIE)" />                                  
                <ext:DateField ID="txDateNie" runat="server" FieldLabel="Expired NIE" Format="dd-MM-yyyy" EnableKeyEvents="true" />              
                <ext:Checkbox ID="chkAktif" runat="server" FieldLabel="Aktif" />
                <ext:TextArea ID="taKomposisi" runat="server" FieldLabel="Komposisi" Width="500" AllowBlank="true" />
                <ext:ComboBox ID="cbGolongan" runat="server" FieldLabel="Golongan" ValueField="c_type"
                  DisplayField="v_ket" Width="200" AllowBlank="true">
                  <CustomConfig>
                    <ext:ConfigItem Name="allowBlank" Value="true" />
                  </CustomConfig>
                  <Store>
                    <ext:Store ID="Store1" runat="server">
                      <Proxy>
                        <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                          CallbackParam="soaScmsCallback" />
                      </Proxy>
                      <BaseParams>
                        <ext:Parameter Name="allQuery" Value="true" />
                        <ext:Parameter Name="model" Value="2001" />
                        <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                                  ['c_notrans = @0', '61', '']]" Mode="Raw" />
                        <ext:Parameter Name="sort" Value="c_type" />
                        <ext:Parameter Name="dir" Value="ASC" />
                      </BaseParams>
                      <Reader>
                        <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                          <Fields>
                            <ext:RecordField Name="c_type" />
                            <ext:RecordField Name="v_ket" />
                          </Fields>
                        </ext:JsonReader>
                      </Reader>
                    </ext:Store>
                  </Store>
                  <Triggers>
                    <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                  </Triggers>
                  <Listeners>
                    <%--<Change Handler="changeAsal(this, #{lbCbg}, #{txCabang});" />--%>
                    <Select Handler="this.triggers[0].show();" />
                    <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                    <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); }" />
                  </Listeners>
                </ext:ComboBox>
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button ID="btnSave" runat="server" Icon="Disk" Text="Simpan" Hidden="true">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders},#{gridDetail});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfItemNo}.getValue()" Mode="Raw" />
            <ext:Parameter Name="MinQ" Value="#{txMinQ}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
