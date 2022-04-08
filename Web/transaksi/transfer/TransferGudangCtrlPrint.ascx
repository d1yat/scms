<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TransferGudangCtrlPrint.ascx.cs"
  Inherits="transaksi_transfer_TransferGudangCtrlPrint" %>
<ext:Window ID="winPrintDetail" runat="server" Height="150" Width="390" Hidden="true"
  Maximizable="false" Layout="Fit" Icon="Printer">
  <Content>
    <ext:Hidden ID="hfType" runat="server" />
  </Content>
  <Items>
    <ext:FormPanel runat="server" Layout="Form" Padding="5" LabelWidth="150"> 
      <Items>
        <ext:ComboBox ID="cbGdgPrint" runat="server" FieldLabel="Gudang Tujuan" DisplayField="v_gdgdesc"
          ValueField="c_gdg" Width="175" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
          MinChars="3" AllowBlank="true" ForceSelection="false">
          <CustomConfig>
            <ext:ConfigItem Name="allowBlank" Value="true" />
          </CustomConfig>
          <Store>
            <ext:Store runat="server" RemotePaging="false">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <%--<ext:Parameter Name="start" Value="={0}" />
              <ext:Parameter Name="limit" Value="={10}" />--%>
                <ext:Parameter Name="allQuery" Value="true" />
                <ext:Parameter Name="model" Value="0176" />
                <ext:Parameter Name="parameters" Value="[['c_gdg != @0', #{hfGudang}.getValue(), 'System.Char']]"
                  Mode="Raw" />
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
          <Template ID="Template1" runat="server">
            <Html>
            <table cellpading="0" cellspacing="0" style="width: 200px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
            <tpl for=".">
              <tr class="search-item">
                <td>{c_gdg}</td><td>{v_gdgdesc}</td>
              </tr>
            </tpl>
            </table>
            </Html>
          </Template>
          <Triggers>
            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
          </Triggers>
          <Listeners>
            <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
          </Listeners>
        </ext:ComboBox>
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Nomor Surat Jalan">
          <Items>
            <ext:TextField ID="sjNumber1" runat="server" MaxLength="10" Width="90" />
            <ext:Label ID="Label1" runat="server" Text=" - " />
            <ext:TextField ID="sjNumber2" runat="server" MaxLength="10" Width="90" />
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
            <ext:Parameter Name="GudangID" Value="#{cbGdgPrint}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SJID1" Value="#{sjNumber1}.getValue()" Mode="Raw" />
            <ext:Parameter Name="SJID2" Value="#{sjNumber2}.getValue()" Mode="Raw" />
            <ext:Parameter Name="TypeCode" Value="#{hfType}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Async" Value="#{chkAsync}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
