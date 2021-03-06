<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PurchaseOrderPrintCtrl.ascx.cs"
  Inherits="transaksi_penjualan_PurchaseOrderPrintCtrl" %>
<ext:Window ID="winPrintDetail" runat="server" Height="150" Width="460" Hidden="true"
  Maximizable="false" Layout="Fit" Icon="Printer">
  <Items>
    <ext:FormPanel runat="server" Layout="Form" Padding="5" LabelWidth="150">
      <Items>
        <ext:ComboBox ID="cbSuplier" runat="server" DisplayField="v_nama" ValueField="c_nosup"
          Width="260" ItemSelector="tr.search-item" PageSize="10" ListWidth="400" MinChars="3"
          AllowBlank="true" FieldLabel="Suplier"  ForceSelection="false">
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
                <ext:Parameter Name="model" Value="2021" />
                <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                                        ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]"
                  Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_nama" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_nosup" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_nosup" />
                    <ext:RecordField Name="v_nama" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template1" runat="server">
            <Html>
            <table cellpading="0" cellspacing="1" style="width: 400px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
            <tpl for="."><tr class="search-item">
                <td>{c_nosup}</td><td>{v_nama}</td>
            </tr></tpl>
            </table>
            </Html>
          </Template>
        </ext:ComboBox>
        <ext:CompositeField runat="server" FieldLabel="Nomor Purchase Order">
          <Items>
            <ext:TextField ID="poNumber1" runat="server" MaxLength="10" Width="90" />
            <ext:Label runat="server" Text=" - " />
            <ext:TextField ID="poNumber2" runat="server" MaxLength="10" Width="90" />
          </Items>
        </ext:CompositeField>
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" />
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button runat="server" Text="Cetak" Icon="PrinterGo">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate">
          <Confirmation ConfirmRequest="true" Title="Cetak ?" Message="Anda yakin ingin mencetak data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="SuplierID" Value="#{cbSuplier}.getValue()" Mode="Raw" />
            <ext:Parameter Name="PO1" Value="#{poNumber1}.getValue()" Mode="Raw" />
            <ext:Parameter Name="PO2" Value="#{poNumber2}.getValue()" Mode="Raw" />
            <ext:Parameter Name="Async" Value="#{chkAsync}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
