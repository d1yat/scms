<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FakturEkspedisiPrintCtrl.ascx.cs" Inherits="transaksi_pengiriman_FakturEkspedisiPrintCtrl" %>
<ext:Window ID="winPrintDetail" runat="server" Height="150" Width="390" Hidden="true"
  Maximizable="false" Layout="Fit" Icon="Printer">
  <Items>
    <ext:FormPanel ID="FormPanel1" runat="server" Layout="Form" Padding="5" LabelWidth="150">
      <Items>
        <ext:ComboBox ID="cbEksHdr" runat="server" DisplayField="v_ket" ValueField="c_exp"
            Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="200" MinChars="3"
            FieldLabel="Expedisi" AllowBlank="true">
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
                        <ext:Parameter Name="start" Value="={0}" />
                        <ext:Parameter Name="limit" Value="={10}" />
                        <ext:Parameter Name="model" Value="2081" />
                        <ext:Parameter Name="parameters" Value="[['c_exp != @0', '00', 'System.String'],
                     ['@contains.v_ket.Contains(@0) || @contains.c_exp.Contains(@0)', paramTextGetter(#{cbEksHdr}), '']]"
                            Mode="Raw" />
                        <ext:Parameter Name="sort" Value="v_ket" />
                        <ext:Parameter Name="dir" Value="ASC" />
                    </BaseParams>
                    <Reader>
                        <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                            TotalProperty="d.totalRows">
                            <Fields>
                                <ext:RecordField Name="c_exp" />
                                <ext:RecordField Name="v_ket" />
                            </Fields>
                        </ext:JsonReader>
                    </Reader>
                </ext:Store>
            </Store>
            <Template ID="Template1" runat="server">
                <Html>
                <table cellpading="0" cellspacing="2" style="width: 100">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
            <tpl for="."><tr class="search-item">
            <td>{c_exp}</td><td>{v_ket}</td>
            </tr></tpl>
            </table>
                </Html>
            </Template>
            <Triggers>
                <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
                <ext:FieldTrigger Icon="Search" Qtip="Reload" />
            </Triggers>
            <Listeners>
                <Select Handler="this.triggers[0].show();" />
                <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
                <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
            </Listeners>
        </ext:ComboBox>
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Nomor Faktur Ekspedisi">
          <Items>
            <ext:TextField ID="fakturNumber1" runat="server" MaxLength="10" Width="90" />
            <ext:Label ID="Label1" runat="server" Text=" - " />
            <ext:TextField ID="fakturNumber2" runat="server" MaxLength="10" Width="90" />
          </Items>
        </ext:CompositeField>
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="0"
          AllowBlank="false">
          <Items>
            <ext:ListItem Value="01" Text="PDF" />
            <ext:ListItem Value="02" Text="Excel Data Only" />
            <ext:ListItem Value="03" Text="Excel" />
          </Items>
        </ext:SelectBox>
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" />
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button ID="Button1" runat="server" Text="Cetak" Icon="PrinterGo">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate">
          <Confirmation ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="EkspID" Value="#{cbEksHdr}.getValue()" Mode="Raw" />
            <ext:Parameter Name="FAKTURID1" Value="#{fakturNumber1}.getValue()" Mode="Raw" />
            <ext:Parameter Name="FAKTURID2" Value="#{fakturNumber2}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Async" Value="#{chkAsync}.getValue()" Mode="Raw" />
            <ext:Parameter Name="OutputRpt" Value="#{cbRptTypeOutput}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />