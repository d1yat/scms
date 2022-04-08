<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FakturManual.ascx.cs"
  Inherits="reporting_finance_FakturManual" %>
<ext:Viewport ID="vwGudang" runat="server" Layout="Fit">
  <Items>
    <ext:Panel runat="server">
      <Items>
        <ext:Hidden ID="hidWndDown" runat="server" />
        <ext:Hidden ID="hfStoreID" runat="server" />
        <ext:FormPanel ID="frmReport" runat="server" Padding="5" Frame="True"
          Layout="Form">
          <Items>
            <ext:ComboBox ID="cbPrincipalHdr" runat="server" FieldLabel="Pemasok" DisplayField="v_nama"
              ValueField="c_nosup" Width="250" ItemSelector="tr.search-item" ListWidth="350"
              MinChars="3" AllowBlank="true" ForceSelection="false">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="true" />
              </CustomConfig>
              <Store>
                <ext:Store ID="Store4" runat="server">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="={0}" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="model" Value="14014" />
                    <ext:Parameter Name="parameters" Value="[['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbPrincipalHdr}), '']]"
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
                <table cellpading="0" cellspacing="0" style="width: 350px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_nosup}</td><td>{v_nama}</td>
                </tr></tpl>
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
            <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Periode">
              <Items>
                <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                  AllowBlank="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
                  </CustomConfig>
                </ext:DateField>
                <ext:Label ID="Label1" runat="server" Text="-" />
                <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
                  AllowBlank="false">
                  <CustomConfig>
                    <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" />
                  </CustomConfig>
                </ext:DateField>
              </Items>
            </ext:CompositeField>
            <ext:CompositeField ID="CompositeField3" runat="server" FieldLabel="No. Faktur">
              <Items>
                <ext:TextField ID="txFakturno1" runat="server" />
                <ext:Label ID="Label3" runat="server" Text="-" />
                <ext:TextField ID="txFakturno2" runat="server" />
              </Items>
            </ext:CompositeField>
            <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="1"
              AllowBlank="false">
              <Items>
                <ext:ListItem Value="01" Text="PDF" />
                <ext:ListItem Value="02" Text="Excel Data Only" />
                <ext:ListItem Value="03" Text="Excel" />
              </Items>
            </ext:SelectBox>
            <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="false" />
          </Items>
        </ext:FormPanel>
      </Items>
      <Buttons>
        <ext:Button runat="server" Icon="Report" Text="Generate">
          <DirectEvents>
            <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmReport});">
              <EventMask ShowMask="true" />
            </Click>
          </DirectEvents>
        </ext:Button>
        <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
          <Listeners>
            <Click Handler="initializePanel(#{frmReport}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
          </Listeners>
        </ext:Button>
      </Buttons>
    </ext:Panel>
  </Items>
  <Listeners>
    <AfterRender Handler="initializePanel(#{frmReport}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
  </Listeners>
</ext:Viewport>
<ext:Window ID="wndDown" runat="server" Hidden="true" />
