<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MemoPemusnahanCtrlPrint.ascx.cs" 
Inherits="memo_pemusnahan_MemoPemusnahanCtrlPrint" %>
<ext:Window ID="winPrintDetail" runat="server" Height="150" Width="390" Hidden="true"
  Maximizable="false" Layout="Fit" Icon="Printer">
  <Content>
    <ext:Hidden ID="hfType" runat="server" />
  </Content>
  <Items>
    <ext:FormPanel runat="server" Layout="Form" Padding="5" LabelWidth="150">
      <Items>
        <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" DisplayField="v_gdgdesc"
        ValueField="c_gdg" Width="200" PageSize="10" ListWidth="200" ItemSelector="tr.search-item"
        MinChars="3" AllowBlank="false" ForceSelection="false">
        <CustomConfig>
          <ext:ConfigItem Name="allowBlank" Value="false" />
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
              <ext:Parameter Name="model" Value="2031" />
              <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
              <ext:Parameter Name="sort" Value="v_gdgdesc" />
              <ext:Parameter Name="dir" Value="ASC" />
            </BaseParams>
            <Reader>
              <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success"
                TotalProperty="d.totalRows">
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
          <table cellpading="0" cellspacing="0" style="width: 350px">
          <tr><td class="body-panel">Kode</td><td class="body-panel">TO</td></tr>
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
      <ext:CompositeField runat="server" FieldLabel="Nomor Memo">
        <Items>
          <ext:TextField ID="pmNumber1" runat="server" MaxLength="10" Width="90" />
          <ext:Label runat="server" Text=" - " />
          <ext:TextField ID="pmNumber2" runat="server" MaxLength="10" Width="90" />
        </Items>
      </ext:CompositeField>
      <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" />
      </Items>
      <Buttons>
        <ext:Button runat="server" Text="Cetak" Icon="PrinterGo">
          <DirectEvents>
            <Click OnEvent="Report_OnGenerate">
              <Confirmation ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="GudangID" Value="#{cbGudang}.getValue()" Mode="Raw" />
                <ext:Parameter Name="PMID1" Value="#{pmNumber1}.getValue()" Mode="Raw" />
                <ext:Parameter Name="PMID2" Value="#{pmNumber2}.getValue()" Mode="Raw" />
                <ext:Parameter Name="Async" Value="#{chkAsync}.getValue()" Mode="Raw" />
              </ExtraParams>
            </Click>
          </DirectEvents>
        </ext:Button>
      </Buttons>
    </ext:FormPanel>
  </Items>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />

