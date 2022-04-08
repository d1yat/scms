<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DOSTTCtrlPrint.ascx.cs" Inherits="transaksi_DO_DOSTTCtrlPrint" %>

<ext:Window ID="winPrintDetail" runat="server" Height="150" Width="390" Hidden="true"
  Maximizable="false" Layout="Fit" Icon="Printer">
  <Items>
    <ext:FormPanel ID="FormPanel1" runat="server" Layout="Form" Padding="5" LabelWidth="150">
      <Items>
        <ext:ComboBox ID="cbCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
          Width="190" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
          llowBlank="true" ForceSelection="false" FieldLabel="Cabang">
          <Store>
            <ext:Store runat="server">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="true" />
              </CustomConfig>
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2011" />
                <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), '']]"
                  Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_cunam" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_cusno" />
                    <ext:RecordField Name="c_cab" />
                    <ext:RecordField Name="v_cunam" />
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
            <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
          </Triggers>
          <Listeners>
            <Select Handler="this.triggers[0].show();" />
            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
            <TriggerClick Handler="if (index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
          </Listeners>
        </ext:ComboBox>
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Nomor DO">
          <Items>
            <ext:TextField ID="doNumber1" runat="server" MaxLength="10" Width="90" />
            <ext:Label ID="Label1" runat="server" Text=" - " />
            <ext:TextField ID="doNumber2" runat="server" MaxLength="10" Width="90" />
          </Items>
        </ext:CompositeField>
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
            <ext:Parameter Name="CustomeID" Value="#{cbCustomer}.getValue()" Mode="Raw" />
            <ext:Parameter Name="DOID1" Value="#{doNumber1}.getValue()" Mode="Raw" />
            <ext:Parameter Name="DOID2" Value="#{doNumber2}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Async" Value="#{chkAsync}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />